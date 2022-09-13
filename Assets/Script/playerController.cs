using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float speed = 12.0f;
    public float gravity = -9.81f;
    public float jumpHaight = 3.0f;

    public Vector3 originalCameraPosition;
    public Transform screenCenter;

    public CharacterController controller;

    Vector3 velocity;

    public bool isGround;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;


    public Transform gunHold;
    public Transform gunAdditional;
    public Transform granadeHold;
    public Transform otherHoldPoint;

    [Space]
    public Vector3 backRotation = new Vector3(90,90,180);
    [Space]

    public bool isRightHandFull = false;
    public bool isAiming = false;
    public bool hasHammer = false;
    public bool otherHoldFull = false;

    private gunManager gunMenager = null;
    private gunManager gunMenager2 = null;
    private gadJet gadJet1 = null, gadJet2 = null;

    private granadeManager granadeManager = null;
    public GameObject[] granadeArray;
    [Space]
    public int numberOfGranade;

    public InventoryManager inventory;
    [Space]
    public HUDManager mng = null;

    // Start is called before the first frame update
    private void Start()
    {
        inventory = GetComponent<InventoryManager>();
        originalCameraPosition = Camera.main.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);


        if (isGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (inventory.weaponsNumber > 0)
                aim();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {          
            if (isRightHandFull)
                if (inventory.equipedWeapons[0])
                {
                    inventory.equipedWeapons[0].transform.localPosition = inventory.equipedWeapons[0].GetComponent<gunManager>().holdPosition;
                    Camera.main.fieldOfView = 60;
                    Camera.main.transform.parent = gameObject.transform;
                    Camera.main.transform.localPosition = originalCameraPosition;
                    isAiming = false;
                }
        }


        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            velocity.y = Mathf.Sqrt(jumpHaight * -2f * gravity);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            pickUp();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            throwGranade();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            dropItem();
        }

        if (Input.GetKeyDown(KeyCode.Q))// || Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            invertWeapons();
        }
    }

    /*
     * the method pickUp creates a raycast thati if hits a GO in the items layer and the hand is not full will place the GO in the holding position
     */
    private void pickUp()
    {
        Debug.DrawRay(Camera.main.transform.position, transform.TransformDirection(Vector3.forward) * 30f, Color.magenta, 10f, false);
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, LayerMask.GetMask("items")))
        {
            Debug.DrawRay(Camera.main.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow, 10f, false);

            if (hit.collider != null)
            {
                //  Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
                GameObject hold = hit.collider.gameObject;

                if (hit.collider.gameObject.CompareTag("Item"))
                {
                    if (hold.gameObject.GetComponent<ObjectsManager>().objectType == ObjectsManager.ObjectType.ammo)
                        addAmmo(hold);
                    else if (hold.gameObject.GetComponent<ObjectsManager>().objectType == ObjectsManager.ObjectType.backpack)
                        inventory.changeBackpack(hold.gameObject.GetComponent<ObjectsManager>());
                    else if (inventory.addItemToInventory(hold.GetComponent<ObjectsManager>()))
                        Debug.Log("Inventory full");
                }
                else if (hit.collider.gameObject.CompareTag("gun") )
                {
                    if (!isRightHandFull)
                    {
                        isRightHandFull = true;                       
                        gunMenager = hold.GetComponent<gunManager>();
                        gunMenager.isHold = true;
                        hold.transform.parent = gunHold.gameObject.transform;
                        hold.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        hold.transform.localPosition = hold.GetComponent<gunManager>().holdPosition;
                        hold.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                        hold.gameObject.transform.localEulerAngles = hold.GetComponent<gunManager>().holdOffsetRotation;
                       
                    }
                    else if (inventory.weaponsNumber < inventory.maxWeapons)
                    {                  
                        gunMenager2 = hold.GetComponent<itemsMenager>().GetComponent<gunManager>();
                        gunMenager2.gameObject.transform.parent = gunAdditional.gameObject.transform;
                        gunMenager2.gameObject.transform.localEulerAngles = backRotation;
                        gunMenager2.gameObject.transform.localPosition = gunAdditional.gameObject.transform.position;
                        gunMenager2.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    }

                    GetComponent<InventoryManager>().addActiveWeapon(hold.GetComponent<ObjectsManager>());

                }
                else if (hit.collider.gameObject.CompareTag("gadjet"))
                {
                    if (inventory.equipedWeapons[0] != null && hold.GetComponent<gunManager>() && gadJet1 == null)
                    {
                        gadJet1 = hit.collider.gameObject.GetComponent<gadJet>();
                        attachItem(gadJet1);
                    }
                    else if (inventory.equipedWeapons[0] != null && hold.GetComponent<gunManager>() && gadJet2 == null)
                    {
                        gadJet2 = hit.collider.gameObject.GetComponent<gadJet>();
                        attachItem(gadJet2);
                    }
                }
                else if (hit.collider.gameObject.CompareTag("granade"))
                {
                    Debug.Log("hit in granada tag " + hit.collider.gameObject);
                    if (granadeManager != null)
                    {
                        if (granadeManager.granadeType == hit.collider.gameObject.GetComponent<granadeManager>().granadeType && granadeManager.maxForType > numberOfGranade)
                        {
                            if (!hit.collider.gameObject.GetComponent<granadeManager>().inPlayerPossesion)
                            {
                                granadeManager = hit.collider.gameObject.GetComponent<granadeManager>();
                                granadeArray[numberOfGranade] = hit.collider.gameObject;
                                numberOfGranade++;
                                setGranadePositionOnBelt();
                            }
                        }
                    }
                    else
                    {
                        if (numberOfGranade > 0)
                        {
                            for (int i = 0; i < numberOfGranade; i++)
                            {
                                granadeArray[i].transform.parent = null;
                                granadeArray[i].transform.position = hit.collider.gameObject.transform.position;
                            }
                        }



                        granadeManager = hit.collider.gameObject.GetComponent<granadeManager>();
                        granadeArray[numberOfGranade] = hit.collider.gameObject;
                        setGranadePositionOnBelt();

                        numberOfGranade = 1;

                    }
                }
                else if (hit.collider.gameObject.CompareTag("hammer"))
                {
                    hasHammer = true;
                    hit.collider.gameObject.transform.parent = gunAdditional.gameObject.transform;
                    hit.collider.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                }
                else if (hit.collider.gameObject.CompareTag("plank"))
                {
                    if (hit.collider.gameObject.GetComponent<plankBuilding>())
                    {
                        if (!otherHoldFull && !hit.collider.gameObject.GetComponent<plankBuilding>().isBuilt)
                        {
                            hit.collider.gameObject.GetComponent<plankBuilding>().isHold = true;
                            hit.collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                            otherHoldFull = true;
                            hit.collider.gameObject.transform.parent = otherHoldPoint;
                            hit.collider.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                            hit.collider.gameObject.transform.localRotation = Quaternion.Euler(90f, -90f, 90f);

                        }
                    }


                }
            }

        }

        void setGranadePositionOnBelt()
        {
            granadeManager.inPlayerPossesion = true;
            granadeManager.rigidbody.isKinematic = true;

            granadeManager.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            granadeManager.transform.parent = granadeHold.gameObject.transform;
            granadeManager.transform.localPosition = new Vector3(0f, 0f, 0f);
            granadeManager.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }


    }

    /*
     * this method "attachItem" is used to place a gadjet on the right spot on a weapon To REWORK
     */
    private void attachItem(gadJet g)
    {
        if (g.gadjetType.ToString() == "flashLight")
            g.transform.parent = inventory.equipedWeapons[0].gameObject.GetComponent<attachmentMenager>().barrelTransform;
        else if (g.gadjetType.ToString() == "longRangeScope" || g.gadjetType.ToString() == "scope")
        {
            g.transform.parent = inventory.equipedWeapons[0].gameObject.GetComponent<attachmentMenager>().scopeTransform;
            inventory.equipedWeapons[0].gameObject.GetComponent<gunManager>().hasScope = true;
        }

        g.GetComponent<gadJet>().isHold = true;
        g.transform.localPosition = new Vector3(0f, 0f, 0f);
        g.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        g.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    private void dropItem()
    {
        if (inventory.equipedWeapons[0] != null)
        {
            isRightHandFull = false;

            if (inventory.equipedWeapons[0].GetComponent<gunManager>())
            {
                inventory.equipedWeapons[0].GetComponent<gunManager>().isHold = false;
                foreach(gunManager g in GetComponent<InventoryManager>().equipedWeapons)
                {
                    if(g.id == inventory.equipedWeapons[0].GetComponent<gunManager>().id)
                    {
                        inventory.weaponsNumber--;
                        inventory.equipedWeapons.Remove(g);
                       
                        if (inventory.weaponsNumber == 0)
                        {
                            inventory.activeAmmos.Clear();                         
                        }
                        
                        break;
                    }
                }
            }


            inventory.equipedWeapons[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            inventory.equipedWeapons[0].transform.parent = null;
            inventory.equipedWeapons[0].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            inventory.equipedWeapons[0].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            
            if (inventory.weaponsNumber <= 0)
            {
                inventory.equipedWeapons[0] = null;
            }
            else
            {
                //sposta arma di scorta in mano
            }
            
        }

        if (gadJet1 != null)
            gadJet1 = null;

        if (gadJet2 != null)
            gadJet2 = null;

        if (otherHoldFull)
        {
            otherHoldFull = false;
            otherHoldPoint.gameObject.GetComponentInChildren<plankBuilding>().isHold = false;
            otherHoldPoint.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = false;
            otherHoldPoint.gameObject.GetComponentInChildren<MeshRenderer>().material = otherHoldPoint.gameObject.GetComponentInChildren<plankBuilding>().originalMaterial;
            otherHoldPoint.gameObject.GetComponentInChildren<plankBuilding>().gameObject.transform.localScale = otherHoldPoint.gameObject.GetComponentInChildren<plankBuilding>().originalScale;
            otherHoldPoint.gameObject.GetComponentInChildren<plankBuilding>().gameObject.transform.parent = null;
        }
    }

    private void aim()
    {

        if (isRightHandFull)
            if (inventory.equipedWeapons[0])
            {
                //gunHold.transform.localPosition = new Vector3(0f, -0.3f, 0.7f);

                if (inventory.equipedWeapons[0].gameObject.GetComponent<gunManager>().hasScope)
                {
                    inventory.equipedWeapons[0].transform.localPosition = inventory.equipedWeapons[0].aimScopeOffsetPosition;
                }
                else
                {
                    inventory.equipedWeapons[0].transform.localPosition = inventory.equipedWeapons[0].aimOffsetPosition;                  
                }
                inventory.equipedWeapons[0].transform.localRotation = Quaternion.Euler(inventory.equipedWeapons[0].aimOffsetRotation);
                //item.transform.localRotation = Quaternion.Euler(0, -0.5f, 0);

                //if (!isAiming)
                Camera.main.fieldOfView += inventory.equipedWeapons[0].GetComponent<gunManager>().zoomOnAim;
                Camera.main.transform.parent = inventory.equipedWeapons[0].GetComponent<attachmentMenager>().scopeGO.GetComponent<scopeManager>().scopeCenter;
                Camera.main.transform.localPosition = new Vector3(0,0,0);
                isAiming = true;
            }
            else inventory.equipedWeapons[0].transform.position = gunHold.position;
    }

    private void throwGranade()
    {

        if (numberOfGranade > 0)
        {
            granadeArray[numberOfGranade - 1].GetComponent<granadeManager>().transform.parent = null;
            granadeArray[numberOfGranade - 1].GetComponent<granadeManager>().inPlayerPossesion = false;
            granadeArray[numberOfGranade - 1].GetComponent<granadeManager>().beenThrown = true;
            granadeArray[numberOfGranade - 1].GetComponent<granadeManager>().rigidbody.isKinematic = false;
            granadeArray[numberOfGranade - 1].transform.position = gunHold.position;
            granadeArray[numberOfGranade - 1].transform.rotation = gameObject.transform.rotation;
            granadeArray[numberOfGranade - 1].GetComponent<granadeManager>().rigidbody.AddForce((granadeArray[numberOfGranade - 1].transform.up + granadeArray[numberOfGranade - 1].transform.forward) * granadeManager.thrust, ForceMode.Impulse);
            granadeArray[numberOfGranade - 1] = null;
            numberOfGranade--;
        }
        if (numberOfGranade <= 0)
            granadeManager = null;

    }

    private void addAmmo(GameObject ammo)
    {
        if (!gameObject.GetComponent<InventoryManager>().addItemToInventory(ammo.GetComponent<ObjectsManager>()))
            Debug.Log("Inventory full");
    }

    private void invertWeapons()
    {
        if (inventory.weaponsNumber > 1)
        {
            gunManager flag = inventory.equipedWeapons[0];
            inventory.equipedWeapons[0] = inventory.equipedWeapons[1];
            inventory.equipedWeapons[1] = flag;

            invertGameobjectPosition(inventory.equipedWeapons[0].gameObject, inventory.equipedWeapons[1].gameObject);


            inventory.equipedWeapons[0].isHold = true;
            inventory.equipedWeapons[1].isHold = false;

            inventory.equipedWeapons[0].transform.parent = gunHold.gameObject.transform;
            inventory.equipedWeapons[1].transform.parent = gunAdditional.gameObject.transform;

            inventory.equipedWeapons[0].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            inventory.equipedWeapons[1].transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            inventory.equipedWeapons[0].transform.localPosition = inventory.equipedWeapons[0].holdPosition;           
            inventory.equipedWeapons[1].gameObject.transform.localPosition = gunAdditional.gameObject.transform.position;

            inventory.equipedWeapons[0].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            inventory.equipedWeapons[1].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            inventory.equipedWeapons[0].gameObject.transform.localEulerAngles = inventory.equipedWeapons[0].holdOffsetRotation;
            inventory.equipedWeapons[1].gameObject.transform.localEulerAngles = backRotation;

            mng.updateText(mng.magazine, inventory.equipedWeapons[0].ammoInMagazine.ToString());
            mng.updateText(mng.ammo, "/" + inventory.equipedWeapons[0].ammoQuantity.ToString());
            
            if (inventory.equipedWeapons[0].isAutomatic)
            {
                mng.updateText(mng.fireMode, "|||");
            }
            else if(inventory.equipedWeapons[0].isSemi)
            {
                mng.updateText(mng.fireMode, "||");
            }
            else
            {
                mng.updateText(mng.fireMode, "|");

            }
        }
    }

    private void invertGameobjectPosition(GameObject g, GameObject f)
    {
        Vector3 tempPosition = g.transform.position;      
        g.transform.position = f.transform.position;
        f.transform.position = tempPosition;
    }
}
