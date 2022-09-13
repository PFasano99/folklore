using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plateMenager : MonoBehaviour
{

    public AudioClip hitClipAudio;
    public AudioSource hitSourceAudio;
    public float volume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("bullet"))
        {
            hitSourceAudio.PlayOneShot(hitClipAudio, volume);
        }
    }
}
