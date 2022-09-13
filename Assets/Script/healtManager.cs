using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healtManager : MonoBehaviour
{
    public float healt;


    // Update is called once per frame
    void Update()
    {
        if (healt <= 0)
            beforeDestroy();
    }

    void beforeDestroy()
    {
        if (GetComponentInParent<objectBuilding>())
        {
            GetComponentInParent<objectBuilding>().isBuilt = false;
            GetComponentInParent<objectBuilding>().buildingTime = 0;            
        }

        Destroy(gameObject);
    }
}
