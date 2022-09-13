using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitRange : MonoBehaviour
{

    public GameObject[] inLimitGo;
    public GameObject[] outLimitGo;

    private void OnTriggerEnter(Collider other)
    {  
        if(other.gameObject.CompareTag("Player"))
        {
            foreach (GameObject g in inLimitGo)
            {
                g.SetActive(false);
            }

            foreach (GameObject g in outLimitGo)
            {
                g.SetActive(true);
            }

            if (GetComponentInParent<targetRangeManager>())
                GetComponentInParent<targetRangeManager>().exitRange();
        }
        
    }
}
