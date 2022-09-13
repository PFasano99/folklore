using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class InventoryManager : MonoBehaviour
{
    public List<ObjectsManager> inventory;

    public ObjectsManager backpack;
    public int freeSpace = 0;
    public int occupiedSpace = 0;
    [Space]
    public Vector3 tpPoint = new Vector3(0, -100, 0);
    private int lastId = 0;

    [Space]
    public List<ammoList> AmmoList = new List<ammoList>();
    public int maxWeapons;
    public int weaponsNumber;
    public List<gunManager> equipedWeapons;
    public List<bulletType.AmmoType> activeAmmos;
    
    [Space]
    public HUDManager mng = null;

    private void Start()
    {
        freeSpace = backpack.spaceAdded;
    }

    public bool addItemToInventory(ObjectsManager item)
    {      
        if (freeSpace - item.spaceOccupied >= 0)
        {
            item.id = lastId;          
            
            if (item.quantity < item.maxQuantity)
            {                
               item = collapseItems(item);
            }

            if (item != null)
            {
                lastId += 1;
                inventory.Add(item);
                item.gameObject.transform.position = tpPoint;
                if (item.gameObject.GetComponent<ammoManager>())
                {
                    updateAmmolist(item);
                }

                freeSpace -= item.spaceOccupied;
            }                       
           
            return true;
        }
        else
            return false;
    }

    public void changeBackpack(ObjectsManager newBackpack)
    {
        if (newBackpack.spaceAdded > backpack.spaceAdded)
        {
            newBackpack.gameObject.transform.position = tpPoint;
            freeSpace = newBackpack.spaceAdded - backpack.spaceAdded;

            if (!addItemToInventory(backpack))
            {
                instancieteItem(backpack);
            }
            
            backpack = newBackpack;
            
        } 
        else if (!addItemToInventory(newBackpack))
        {
            Debug.Log("inventario pieno");
        }
    }

    public void dropItem(ObjectsManager item, bool delate=false)
    {
        int i = 0;
        foreach(ObjectsManager obj in inventory)
        {
            if (obj.id == item.id)
            {
                occupiedSpace -= obj.spaceOccupied;
                inventory.RemoveAt(i);

                if(!delate)
                    instancieteItem(obj);

                break;
            }
            else
                i++;
        }
    }

    public ObjectsManager collapseItems(ObjectsManager item)
    {

        foreach (ObjectsManager o in inventory)
        {
            if (item.objectType == o.objectType )
            {
                if(item.objectType == ObjectsManager.ObjectType.ammo)
                {
                    if(item.gameObject.GetComponent<ammoManager>().ammoType == o.gameObject.GetComponent<ammoManager>().ammoType)
                    {
                        if (o.quantity < o.maxQuantity)
                        {
                            int flagAdd = o.maxQuantity - o.quantity;

                            if (item.quantity <= flagAdd)
                            {
                                o.quantity += item.quantity;
                                item.quantity = 0;
                                updateAmmolist(item);
                                Destroy(item.gameObject);
                                return null;
                            }
                            else
                            {
                                o.quantity += flagAdd;
                                item.quantity -= flagAdd;
                                updateAmmolist(item);
                                updateAmmolist(item);
                            }
                        }
                    }
                }
                else
                {
                    if (o.quantity < o.maxQuantity)
                    {
                        int flagAdd = o.maxQuantity - o.quantity;

                        if (item.quantity <= flagAdd)
                        {
                            o.quantity += item.quantity;
                            item.quantity = 0;
                            Destroy(item.gameObject);
                            return null;
                        }
                        else
                        {
                            o.quantity += flagAdd;
                            item.quantity -= flagAdd;
                        }
                    }
                }             
            }
        }

        return item;
    }

    //the idea is to have the objects in the inventory instanciated somewhere in the map and "teleport" them in front of the player when they are removed but not delated 
    public void instancieteItem(ObjectsManager item)
    {
        item.gameObject.transform.position = new Vector3( gameObject.transform.position.x+1, 1, gameObject.transform.position.z + 1);
    }

    public void changeItemQuantity(ObjectsManager item, int valueChange)
    {
        int i = 0;
        foreach (ObjectsManager obj in inventory)
        {
            if (obj.id == item.id)
            {
                
                if(inventory[i].quantity + valueChange <= inventory[i].maxQuantity && inventory[i].quantity + valueChange > 0)
                {
                    inventory[i].quantity += valueChange;
                }
                else if (inventory[i].quantity + valueChange <= 0)
                {
                    dropItem(item, true);
                }
                else if (inventory[i].quantity + valueChange > inventory[i].maxQuantity)
                {
                    inventory[i].quantity = inventory[i].maxQuantity;
                    ObjectsManager toDrop = inventory[i];
                    toDrop.quantity = (inventory[i].quantity + valueChange) - inventory[i].maxQuantity;

                    if (!addItemToInventory(toDrop))
                        dropItem(toDrop);
                }

                break;
            }
            else
                i++;
        }
    }

    //this method returns a list of all ammo objects in the inventory by type, to use when a certain qunatity of ammo is fired
    public List<ObjectsManager> getAmmo(bulletType.AmmoType ammo)
    {
        List<ObjectsManager> allAmmo = new List<ObjectsManager>();

        foreach (ObjectsManager obj in inventory)
        {
            if (obj.GetComponent<ammoManager>())
            {
                if (obj.GetComponent<ammoManager>().ammoType.ToString() == ammo.ToString())
                    allAmmo.Add(obj);
            }
        }

        return allAmmo;
    }

    public void addActiveWeapon(ObjectsManager item)
    {
        if (weaponsNumber < maxWeapons)
        {
            weaponsNumber++;
            equipedWeapons.Add(item.GetComponent<gunManager>());
            if(activeAmmos==null)
            {
                activeAmmos.Add(item.gameObject.GetComponent<gunManager>().ammoType);
            }
            else
            {
                bool found = false;
                foreach (bulletType.AmmoType t in activeAmmos)
                {
                    if (t == item.gameObject.GetComponent<gunManager>().ammoType)
                    {
                        found = true;
                        updateWeaponsAmmo();
                        break;
                    }                   
                }
                if (!found)
                {
                    activeAmmos.Add(item.gameObject.GetComponent<gunManager>().ammoType);
                    updateWeaponsAmmo();
                }
                    
            }                  
        }
        else if (!addItemToInventory(item))
            Debug.Log("inventario pieno");
            
    }

    //this method updates the list of all ammo adding the ones that are not present and updating the quantity for the ones present
    public void updateAmmolist(ObjectsManager item)
    {
        
        if (AmmoList == null)
        {
            AmmoList.Add(new ammoList(item.gameObject.GetComponent<ammoManager>().ammoType, item.gameObject.GetComponent<ammoManager>().ammoQuantity));           
        }
        else
        {
            int i = 0;
            bool found = false;
            foreach (ammoList stored in AmmoList)
            {
                if (stored.AmmoType.ToString() == item.gameObject.GetComponent<ammoManager>().ammoType.ToString())
                {
                    AmmoList[i] = new ammoList(item.gameObject.GetComponent<ammoManager>().ammoType, stored.Quantity + item.gameObject.GetComponent<ammoManager>().ammoQuantity);
                    found = true;
                    break;
                }
                i++;
            }
            if (!found)
            {
                AmmoList.Add(new ammoList(item.gameObject.GetComponent<ammoManager>().ammoType, item.gameObject.GetComponent<ammoManager>().ammoQuantity));
            }
        }
        updateWeaponsAmmo();


    }

    //this method updates the quantity of ammo present in the active gun
    public void updateWeaponsAmmo()
    {     
        if (weaponsNumber > 0 && AmmoList.Count > 0)
        {        
            foreach (gunManager g in equipedWeapons)
            {
                foreach (ammoList a in AmmoList)
                {
                    if (a.AmmoType.ToString() == g.ammoType.ToString())
                    {
                        g.ammoQuantity = a.Quantity;
                        mng.updateText(mng.ammo, "/" + g.ammoQuantity.ToString()); //we update the ammo quantity on the UI for the active gun
                        break;
                    }

                }
                              
            }         
        }
    }

    //this method updates the quantity of ammo present in the list of all ammo only if that kind of ammo already exists, used to reload weapons
    public void updateWeaponsAmmo(int quantity, gunManager g)
    {
        int i = 0;
        foreach (ammoList a in AmmoList)
        {
            if (a.AmmoType.ToString() == g.ammoType.ToString())
            {
                AmmoList[i] = new ammoList(a.AmmoType, a.Quantity+quantity); 
                break;
            }
            i++;

        }
    }

    [System.Serializable]
    public struct ammoList
    {
        public ammoList(bulletType.AmmoType ammoType, int quantity)
        {
            AmmoType = ammoType;
            Quantity = quantity;
        }

        public bulletType.AmmoType AmmoType { get; set; }
        public int Quantity { get; set; }
    }
 
}


