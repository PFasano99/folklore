using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : MonoBehaviour
{
    public int id = -1;

    public int spaceOccupied = 1;
    public int spaceAdded = 0;
    public int quantity = 1;
    public int maxQuantity = 1;
    public enum ObjectType { weapon, ammo, attachment, backpack};
    public ObjectType objectType;

    public object obj;
    // Start is called before the first frame update
    void Start()
    {
        if (objectType == ObjectType.ammo)
        {
            quantity = gameObject.GetComponent<ammoManager>().ammoQuantity;
            maxQuantity = gameObject.GetComponent<ammoManager>().maxForType;
        }
        else if (objectType == ObjectType.weapon)
        {
            obj = new gunManager();
            obj = gameObject.GetComponent<gunManager>();
        }
    }



}
