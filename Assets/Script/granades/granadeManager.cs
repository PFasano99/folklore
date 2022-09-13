using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class granadeManager : MonoBehaviour
{
    public enum GranadeType { frag, smoke, fire };
    public GranadeType granadeType;

    public Rigidbody rigidbody;
    public float thrust = 10f;

    public int maxForType = 2;

    public bool inPlayerPossesion = false;
    public bool beenThrown = false;

    public GameObject effectGO;
    public float effectDuration, startOffset;
    private Coroutine effectCoroutine = null;

    public AudioClip floorHitAudio, effectAudio;
    public AudioSource granadeAudioSource;
    public float volume = 1f;
    public void playEffect()
    {   
        if(granadeType == GranadeType.smoke)
        effectCoroutine = StartCoroutine(smokeTimeTick(startOffset, effectDuration));

        if (granadeType == GranadeType.frag)
        {

        }
    }

    bool doOnce = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            if (beenThrown && !doOnce)
            {
                doOnce = true;
                granadeAudioSource.PlayOneShot(floorHitAudio, volume);
                playEffect();
            }
        }
    }

    IEnumerator smokeTimeTick(float startAfter, float duration)
    {
        while (true)
        {
            yield return new WaitForSeconds(startAfter);
            granadeAudioSource.PlayOneShot(effectAudio, volume);
            yield return new WaitForSeconds(0.05f);
            GameObject toSpawn = (GameObject)Instantiate(effectGO, transform.position, transform.rotation);
            toSpawn.transform.parent = transform.parent;
            yield return new WaitForSeconds(duration);
            granadeAudioSource.Stop();
            toSpawn.GetComponent<VisualEffect>().Stop();
            yield return new WaitForSeconds(60f);
            StopCoroutine(effectCoroutine);
            Destroy(toSpawn);
            Destroy(this);
            yield return null;
        }
    }

}
