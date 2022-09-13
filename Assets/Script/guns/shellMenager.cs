using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shellMenager : MonoBehaviour
{

    public AudioClip bulletAudio;

    public AudioSource bulletShotAudio;

    public float volume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(reloadTimeTick(3));      
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("gun") == false)
            bulletShotAudio.PlayOneShot(bulletAudio, volume);      
    }

    IEnumerator reloadTimeTick(float second)
    {
        while (true)
        {         
            yield return new WaitForSeconds(second);
            Destroy(gameObject);
            yield return null;
        }
    }
}
