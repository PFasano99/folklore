using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plankBuilding : MonoBehaviour
{
    public bool isHold = false;
    public bool isBuilt = false;
    public bool isColliding = false;
    public Material originalMaterial, noBuildMaterial, toBuildMaterial;

    public GameObject leftCollider, centerCollider, rightCollider, bodyCollider;
    public bool leftCollision, centerCollision, rightCollision;

    public Vector3 originalScale;

    [Header("the audio for hammering")]
    public AudioClip hammerAudio;
    public AudioSource hammerAudioSource;
    public float volume = .2f;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        originalMaterial = GetComponent<MeshRenderer>().material;
        if (isHold)
            GetComponent<MeshRenderer>().material = noBuildMaterial;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isBuilt)
        {           
            if (isHold)
            {
                
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
                    {
                        transform.localPosition +=  Vector3.forward * Time.deltaTime * 2;
                        transform.localScale = originalScale;
                    }
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                    {
                        transform.localPosition -= Vector3.forward * Time.deltaTime * 2;
                        transform.localScale = originalScale;
                    }
                }
                else if (Input.GetAxis("Mouse ScrollWheel") != 0f) // upwards
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, (transform.localPosition.y + Input.GetAxis("Mouse ScrollWheel")), transform.localPosition.z);
                    transform.localScale = originalScale;
                }

                leftCollision = leftCollider.GetComponent<checkCollision>().isColliding;
                centerCollision = centerCollider.GetComponent<checkCollision>().isColliding;
                rightCollision = rightCollider.GetComponent<checkCollision>().isColliding;
                isColliding = bodyCollider.GetComponent<checkCollision>().isColliding;

                if ((centerCollision || leftCollision && rightCollision) && !isColliding)
                {
                    GetComponent<MeshRenderer>().material = toBuildMaterial;
                }
                else
                {
                    GetComponent<MeshRenderer>().material = noBuildMaterial;
                }

                if (Input.GetKeyDown(KeyCode.B) && !isColliding)
                {
                    if (GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<playerController>().hasHammer)
                    {
                        hammerAudioSource.PlayOneShot(hammerAudio, volume);
                        isHold = false;
                        isBuilt = true;
                        GetComponent<MeshRenderer>().material = originalMaterial;
                        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<playerController>().otherHoldFull = false;
                        transform.parent = null;
                        Destroy(leftCollider);
                        Destroy(rightCollider);
                        Destroy(centerCollider);
                        Destroy(bodyCollider);

                    }
                }
            }
        }
        
    }

}
