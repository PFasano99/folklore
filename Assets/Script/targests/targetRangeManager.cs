using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class targetRangeManager : MonoBehaviour
{
    
    public int friendHit, foeHit;
    public int friendTot, foeTot;

    public GameObject Targets;
    public GameObject[] target;

    public GameObject[] inLimitGo;
    public GameObject[] outLimitGo;

    [Space]
    [Header("Score variables")]
    public float recordTime = 9999999f; //seconds
    public float time = 9999999f;

    public float recordPoints = 0f;
    public float points = 0;

    public TextMeshProUGUI recordTimeText, timeText, recordPointText, pointText;

    public Coroutine timer = null;
    // Start is called before the first frame update
    void Start()
    {
        int index = 0;        
        foreach (Transform t in Targets.transform.GetComponentsInChildren<Transform>())
        {          
            if(t.gameObject.CompareTag("target"))
            {
                target[index] = t.gameObject;
                

                if(target[index].GetComponent<TargetManager>().targetType == TargetManager.TargetType.friend)
                {
                    friendTot++;
                }
                else
                {
                    foeTot++;
                }

                index++;
            }
        }
    }

    public void resetTargets()
    {
        foreach (GameObject g in target) 
        {
            if (g.GetComponent<TargetManager>())
            {
               g.transform.rotation = Quaternion.Euler(g.GetComponent<TargetManager>().startRotation);
               g.GetComponent<TargetManager>().isHit = false;
            }
        }
    }

    public void setRangeStats()
    {
        points += (friendTot - friendHit) * 10 + (foeTot - foeHit ) * 10 * -1; 
        pointText.text = points.ToString();
        writeTime(timeText);

        if (points >= recordPoints && time <= recordTime)
        {
            recordPoints = points;
            recordTime = time;

            recordPointText.text= points.ToString();
            writeTime(recordTimeText); 
        }
        
    }

    public void writeTime(TextMeshProUGUI tText)
    {
        int hour = 0;
        int min = (int)time / 60;
        int sec = (int)time - (min * 60);

        if (min > 60)
        {
            hour = min / 60;
            min = min - hour * 60;
        }

        if(sec > 9 && min > 9)
        {
            tText.text = hour+":"+min+":"+sec;
        }
        else if (sec < 9 && min > 9)
        {
            tText.text = hour + ":" + min + ":0" + sec;
        }
        else if (sec > 9 && min < 9)
        {
            tText.text = hour + ":0" + min + ":" + sec;
        }
        else
        {
            tText.text = hour + ":0" + min + ":0" + sec;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            points = 0;
            foreach (GameObject g in inLimitGo)
            {
                g.SetActive(true);
            }

            foreach (GameObject g in outLimitGo)
            {
                g.SetActive(false);
            }

            time = 0;
            timer = StartCoroutine(timerTimeTick());
        }
           
    }

    IEnumerator timerTimeTick()
    {
        while (true)
        {           
            yield return new WaitForSeconds(1f);
            time++;
        }
    }

    public void exitRange()
    {
        StopCoroutine(timer);
        timer = null; 
        setRangeStats();
        points = 0;
        time = 0;
        resetTargets();
    }
}
