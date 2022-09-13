using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletMenager : MonoBehaviour
{
    public int damage;
    public int bulletSpeed;
    public Rigidbody rigidbody;

    public float decondBeforeDestroy;


    public bool hasHit = false;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody.GetComponent<Rigidbody>();
        destroyAfterSec(decondBeforeDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasHit)
        {
            rigidbody.AddForce(transform.TransformDirection(Vector3.forward * bulletSpeed) * 10, ForceMode.Impulse);
            //rigidbody.velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);      
          
        }
      
            

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("gun") == false)
        {
            hasHit = true;
        }
 
    }

    Coroutine destroyRoutine = null;

    private void destroyAfterSec(float sec)
    {
        destroyRoutine = StartCoroutine(growTimeTick(sec));
    }

    IEnumerator growTimeTick(float second)
    {
        while (true)
        {
            yield return new WaitForSeconds(second);
            Destroy(gameObject);
        }
    }
}
