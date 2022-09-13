using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gadJet : MonoBehaviour
{
    public enum GadjetType { scope, longRangeScope, flashLight, magazine};
    public GadjetType gadjetType;

    public bool isHold;

    public Transform scopeCenter = null;

    private void Start()
    {
        if (GadjetType.scope == gadjetType || GadjetType.longRangeScope == gadjetType)
        {
            scopeCenter = gameObject.GetComponentInChildren<Transform>().GetChild(0).gameObject.transform;
        }
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q) && GetComponentInParent<Transform>().gameObject.GetComponentInParent<gunManager>() && isHold)
        {
            if(GetComponentInParent<Transform>().gameObject.GetComponentInParent<gunManager>().isHold)
            useGadjet();
        }
    }

    void useGadjet()
    {
        if (GadjetType.flashLight == gadjetType)
        {
            GameObject light = gameObject.transform.GetChild(0).gameObject;
            light.SetActive(!light.activeSelf);
        }
    }
}
