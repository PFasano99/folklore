using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public enum TargetType { friend, foe };
    public TargetType targetType;
    public int healt = 1;
    public float points = 10f;
    [Space]

    public GameObject targetBody;
    public Material friendMaterial;
    public Material foeMaterial;   

    [Space]

    public AudioClip hitClipAudio;
    public AudioSource hitSourceAudio;
    public float volume = 1f;

    [Space]
    public Vector3 startRotation;
    public Vector3 fallRotation;
    [Space]
    public bool isRandom = false;
    public bool isHit = false;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        startRotation = gameObject.transform.rotation.eulerAngles;

        if (isRandom)
            assignRandomTarget();

        assignMaterial();

    }

    private void assignMaterial()
    {
        if(targetType == TargetType.friend)
        {
            targetBody.GetComponent<MeshRenderer>().material = friendMaterial;
            
        }
        else
            targetBody.GetComponent<MeshRenderer>().material = foeMaterial;


    }
    private void assignRandomTarget()
    {
        int random = Random.Range(0, 5);
        if (random == 0)
        {
            targetType = TargetType.friend;
        }
        else
            targetType = TargetType.foe;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet") && !isHit)
        {
            hitSourceAudio.PlayOneShot(hitClipAudio, volume);
            healt--;
            if (healt <= 0)
                fall();
        }
    }

    private void fall()
    {
        gameObject.transform.rotation = Quaternion.Euler(fallRotation);
        isHit = true;
        if (targetType == TargetType.friend)
        {
            GetComponentInParent<targetRangeManager>().friendHit++;           
            points *= -1f;
        }
        else
        {
            GetComponentInParent<targetRangeManager>().foeHit++;
        }
        GetComponentInParent<targetRangeManager>().points += points;
    }
}
