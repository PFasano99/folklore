using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class gunManager : MonoBehaviour
{
    public int id = -1;
    [Space]

    public Transform setRotation;
    public GameObject bulletShellGo, bulletGo;

    [Header("is the player holding the gun")]
    public bool isHold;
    [Header("is the gun reloading ")]
    public bool isReloading = false;
    [Header("gun fire settings")]
    public bool isAutomatic = true;    
    public bool isSemi = false;
    public bool isBurst = false;
    public bool canBeAutomatic = true;
    public bool canBeSemi = false;
    public bool canBeBurst = true;
    public int burst = 3;
    public float burstMultiplier = 0.5f;
    public bool isBursting = false;

    [Header("the audio for firing")]
    public AudioClip bulletAudio;
    public AudioSource bulletShotAudio;
    public float volume = 1f;

    [Header("Magazine variable and reload")]
    public int magazineSpace;
    public int ammoInMagazine;
    public float reloadTime;
    public bool hasMagazine;
    public bulletType.AmmoType ammoType= new bulletType.AmmoType();
    public int ammoQuantity = 0;

    [Header("Visula effects")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("bullet caratteristics")]
    public float rangeFall;
    public float bulletSpeed;
    public Transform lastBulletPosition;
    public float rangeOffset;
    public float bulletDamage = 10f;

    [Space]
    public float fireRate;
    private float nextTimeToFire = 0f;

   
    public enum ResourceTypes { handgun, shotgun, rifle, sniperRifle, machineGun};
    public ResourceTypes resourceTypes;

    [Space]
    [Header("the coordinates and rotation the gun has when picked up")]
    public Vector3 holdOffsetRotation;
    public Vector3 holdPosition = new Vector3(0.3f, -0.3f, 0f);

    [Space]
    [Header("the coordinates and rotation the gun has when picked up and Aiming")]
    public Vector3 aimOffsetPosition;
    public Vector3 aimOffsetRotation;

    [Space]
    public bool hasScope = false;
    public Vector3 aimScopeOffsetPosition;
    [Space]
    public Transform aimCenter;
    public float zoomOnAim = -20f;

    [Header("the following are the automatic fire offset")]
    public int bulletsFiredNonstop = 0;
    public int recoilAfterBullets = 5;
    public float recoilMultiplier = 0.010f;

    private int contromisura;
    private Coroutine reloadCoroutine = null;
    private Coroutine burstCoroutine = null;
    private Transform fireingStartPoint;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        fireingStartPoint = player.GetComponent<playerController>().screenCenter; // bullet is center of screen for player
        aimCenter = GetComponent<attachmentMenager>().scopeGO.GetComponentInChildren<scopeManager>().scopeCenter;
    }

    // Update is called once per frame
    void Update()
    {
        if(isHold)
        {            
            if(Input.GetKeyDown(KeyCode.Mouse0)  && Time.time >= nextTimeToFire-0.01f)
            {

                nextTimeToFire = Time.time + 1f / fireRate;
                if(isSemi || isAutomatic)
                    fire();    
                else if(isBurst && !isBursting)
                {                  
                    burstCoroutine = StartCoroutine(burstTimeTick());   
                }
            }

            if (Input.GetKey(KeyCode.Mouse0) && !isSemi && !isBurst && isAutomatic && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                fire();
               
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                bulletsFiredNonstop = 0;
            }

            if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            {
                if (ammoInMagazine <= magazineSpace && ammoQuantity > 0)
                    reloadCoroutine = StartCoroutine( reloadTimeTick(reloadTime));               
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                HUDManager mng = player.GetComponentInChildren<HUDManager>();
                if (canBeSemi && !isSemi && isAutomatic)
                {
                    isSemi = !isSemi;
                    isBurst = false;
                    isAutomatic = false;
                    mng.updateText(mng.fireMode, "|"); //we update the fire mode on the UI for the active gun
                }
                else if (canBeBurst && !isBurst )
                {
                    isBurst = !isBurst;
                    isSemi = false;
                    isAutomatic = false;
                    mng.updateText(mng.fireMode, "||");
                }
                else if(canBeAutomatic)
                {
                    isAutomatic = true;
                    isBurst = false;
                    isSemi = false;
                    mng.updateText(mng.fireMode, "|||");
                }
                    
                
                
            }          
        }
    
    }

    /*
     * the method fire plays the sound for the bullet, create the shall of the bullet that get expelled from the side of gunAdditional
     * then the method create a raycast to the range before the bullet fall, if doesn't hit anything the method checkHitAtRange
     * is called to start the calculation for the bullet fall 
     */
    public void fire()
    {
        if(isReloading)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            isReloading = false;
            gameObject.transform.localRotation = Quaternion.Euler(holdOffsetRotation.x , gameObject.transform.localRotation.y, 0);

        }
        else
        {
            if (ammoInMagazine < 1 )
            {
                if(ammoQuantity > 0)
                    reloadCoroutine = StartCoroutine(reloadTimeTick(reloadTime));
                //else mettere rumore clip vuota
            }
            else
            {               
                bulletShotAudio.PlayOneShot(bulletAudio, volume);
                ammoInMagazine -= 1;
                bulletsFiredNonstop++;
                muzzleFlash.Play();

                lastBulletPosition.position = new Vector3(0, 0, 0);

                HUDManager mng = player.GetComponentInChildren<HUDManager>();
                mng.updateText(mng.magazine, ammoInMagazine.ToString());


                /*
                 * the next three line are used to drop a bullet shell from the side of the gun
                 */
                GameObject actualShell = (GameObject)Instantiate(bulletShellGo, GetComponent<attachmentMenager>().bulletShellTransform.position, GetComponent<attachmentMenager>().bulletShellTransform.rotation);
                actualShell.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * 10 * Time.deltaTime, ForceMode.Impulse);
                actualShell.gameObject.GetComponent<Rigidbody>().AddForce(transform.right * 10 * Time.deltaTime, ForceMode.Impulse);
                //actualShell.gameObject.GetComponent<Rigidbody>().velocity = (transform.up * 2);

                if (resourceTypes != ResourceTypes.shotgun)
                    normalFire();
                else
                    shotgunFire();

            }
        }
        
       
              
    }

    private void normalFire()
    {
        /*
         * the following lines genrates a offset for the weapons that are automatic after a certain ammount of bullets fired
         * if the player is aiming the offset will be lower
         */
        Vector3 offsetBullet = Vector3.zero;
        if (bulletsFiredNonstop >= recoilAfterBullets)
        {
            float flag = 0.05f + (bulletsFiredNonstop * recoilMultiplier);
            if (player.GetComponentInChildren<playerController>().isAiming && flag >= .1f)
            {
                flag -= 0.1f;
            }
            offsetBullet = new Vector3(Random.Range(-flag, flag), Random.Range(-flag, flag));
        }
        
        //the shot get fired with the offset
        RaycastHit hit;

        if (Physics.Raycast(fireingStartPoint.position + offsetBullet, fireingStartPoint.TransformDirection(Vector3.forward), out hit, rangeFall))
        {
            Debug.DrawRay(fireingStartPoint.position + offsetBullet, fireingStartPoint.TransformDirection(Vector3.forward) * rangeFall, Color.green, 1f, false);
            GameObject actualBullet = (GameObject)Instantiate(bulletShellGo, hit.point, fireingStartPoint.rotation);
            actualBullet.transform.rotation = Quaternion.Euler(fireingStartPoint.rotation.x, setRotation.transform.eulerAngles.y, fireingStartPoint.rotation.z);

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * bulletSpeed * 10);
            }

            GameObject impactEffectGO = (GameObject)Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactEffectGO, 2f);

            if (hit.transform.gameObject.GetComponent<healtManager>())
            {
                //hit.transform.gameObject.GetComponent<healtManager>().healt -= damage;
            }
        }
        else
        {
            Debug.DrawRay(fireingStartPoint.position + offsetBullet, fireingStartPoint.transform.TransformDirection(Vector3.forward) * rangeFall, Color.blue, 10f, false);

            lastBulletPosition.position = fireingStartPoint.transform.position + fireingStartPoint.transform.TransformDirection(Vector3.forward) * rangeFall;

            GameObject actualBullet = (GameObject)Instantiate(bulletShellGo, lastBulletPosition.position, fireingStartPoint.rotation);
            actualBullet.transform.rotation = Quaternion.Euler(fireingStartPoint.rotation.x, setRotation.transform.eulerAngles.y, fireingStartPoint.rotation.z);

            checkHitAtRange(lastBulletPosition);
        }
    }
    private void shotgunFire()
    {
        for (int pallets = 0; pallets < 7; pallets++)
        {
            Vector3 offsetBullet = new Vector3(Random.Range(-.2f,.2f), Random.Range(-.3f, .3f) + 0f) ;
            RaycastHit hit;

            if (Physics.Raycast(fireingStartPoint.transform.position + offsetBullet, fireingStartPoint.transform.TransformDirection(Vector3.forward), out hit, rangeFall))
            {
                GameObject actualBullet = (GameObject)Instantiate(bulletShellGo, hit.point, fireingStartPoint.rotation);
                actualBullet.transform.rotation = Quaternion.Euler(fireingStartPoint.rotation.x, setRotation.transform.eulerAngles.y, fireingStartPoint.rotation.z);

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * bulletSpeed * 10);
                }

                Debug.DrawRay(fireingStartPoint.transform.position + offsetBullet, fireingStartPoint.transform.TransformDirection(Vector3.forward) * rangeFall, Color.green, 1f, false);

                GameObject impactEffectGO = (GameObject)Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactEffectGO, 2f);

                if (hit.transform.gameObject.GetComponent<healtManager>())
                {
                   // hit.transform.gameObject.GetComponent<healtManager>().healt -= damage;
                }
            }
            else
            {
                Debug.DrawRay(fireingStartPoint.transform.position + offsetBullet, fireingStartPoint.transform.TransformDirection(Vector3.forward) * rangeFall, Color.blue, 10f, false);

                lastBulletPosition.position = fireingStartPoint.transform.position + fireingStartPoint.transform.TransformDirection(Vector3.forward) * rangeFall;

                GameObject actualBullet = (GameObject)Instantiate(bulletShellGo, lastBulletPosition.position, fireingStartPoint.rotation);
                actualBullet.transform.rotation = Quaternion.Euler(fireingStartPoint.rotation.x, setRotation.transform.eulerAngles.y, fireingStartPoint.rotation.z);

                checkHitAtRange(lastBulletPosition);
            }
        }
    }

    IEnumerator reloadTimeTick(float second)
    {
        while (true)
        {
            HUDManager mng = player.GetComponentInChildren<HUDManager>();
            isReloading = true;
            int flagAmmo = ammoInMagazine;
            this.gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localRotation.x, gameObject.transform.localRotation.y, gameObject.transform.localRotation.z - 30f);

            if (hasMagazine)
            {
                bool addOne = false;
                if (ammoInMagazine > 0)
                {
                    ammoQuantity += ammoInMagazine - 1;
                    ammoInMagazine = 1;
                    addOne = true;

                }
                    
               
                yield return new WaitForSeconds(second);               

                if(ammoQuantity - magazineSpace > 0)
                {
                    ammoInMagazine = magazineSpace;
                    player.GetComponent<InventoryManager>().updateWeaponsAmmo(-(ammoInMagazine - flagAmmo), this);
                    ammoQuantity -= magazineSpace;                  
                }
                else
                {
                    ammoInMagazine = ammoQuantity;
                    ammoQuantity = 0;
                }

                if (addOne)
                    ammoInMagazine += 1;
         
                mng.updateText(mng.magazine, ammoInMagazine.ToString()); //we update the ammo in magazine on the UI for the active gun
                mng.updateText(mng.ammo, "/" + ammoQuantity.ToString()); //we update the ammo quantity on the UI for the active gun

            }
            else
            {
                while (ammoInMagazine != magazineSpace)
                {
                    yield return new WaitForSeconds(second);
                    ammoInMagazine++;
                    ammoQuantity--;
                  
                    mng.updateText(mng.magazine, ammoInMagazine.ToString());
                    mng.updateText(mng.ammo, "/"+ammoQuantity.ToString());
                }
            }



            this.gameObject.transform.localRotation = Quaternion.Euler(holdOffsetRotation.x, holdOffsetRotation.y, holdOffsetRotation.z);

            GameObject magazzineToDrop = (GameObject)Instantiate(GetComponent<attachmentMenager>().magazineGO, GetComponent<attachmentMenager>().bulletShellTransform.position, GetComponent<attachmentMenager>().bulletShellTransform.rotation);
            magazzineToDrop.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -10 * Time.deltaTime, ForceMode.Impulse);
           

            isReloading = false;
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            yield return null;
        }
    }

    /*
     * the method checkHitAtRange create raycasts from the point the bullet starts to fall down, lowering the transform position for which the raycast is created
     * for a max of 500 times, an arbitrary number to stop if from looping
     * 
     */
    private void checkHitAtRange(Transform positionCheck)
    {       
        RaycastHit hit;
        do
        {
            positionCheck.position += (positionCheck.transform.TransformDirection(Vector3.down * 0.05f + Vector3.forward * rangeOffset));
            if (Physics.Raycast(positionCheck.position, positionCheck.transform.TransformDirection(Vector3.forward), out hit, rangeOffset))
            {
                Debug.DrawRay(positionCheck.position, positionCheck.transform.TransformDirection(Vector3.forward) * rangeOffset, Color.magenta, 10f, false);
                GameObject actualBullet = (GameObject)Instantiate(bulletShellGo, hit.point, GetComponent<attachmentMenager>().bulletShellTransform.rotation);

                actualBullet.transform.rotation = Quaternion.Euler(GetComponent<attachmentMenager>().bulletShellTransform.rotation.x, setRotation.transform.eulerAngles.y , GetComponent<attachmentMenager>().bulletShellTransform.rotation.z);
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * bulletSpeed * 10);
                }

                if (hit.transform.gameObject.GetComponent<healtManager>())
                {
                    //hit.transform.gameObject.GetComponent<healtManager>().healt -= damage;
                }
                contromisura = 500;
            }
            else
            {               
                Debug.DrawRay(positionCheck.transform.position, positionCheck.transform.TransformDirection(Vector3.forward) * rangeOffset, Color.yellow, 10f, false);
            }
            contromisura++;
        } while (contromisura <= 500);

        lastBulletPosition.position = new Vector3(0, 0, 0);
        contromisura = 0;            
    }
 
    
    IEnumerator burstTimeTick()
    {
        while (true)
        {
            isBursting = true;
            for (int brt = 0; brt < burst; brt++)
            {
                fire();
                yield return new WaitForSeconds( 1f / (fireRate*burstMultiplier));
            }

            isBursting = false;
            StopCoroutine(burstCoroutine);
            yield return null;
        }
    }



}
