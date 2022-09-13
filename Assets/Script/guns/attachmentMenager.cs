using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attachmentMenager : MonoBehaviour
{

    public Transform scopeTransform, magazineTransform, barrelTransform, sRailTransform, eRailTransform, oRailTransform, bulletShellTransform;
    public GameObject scopeGO, magazineGO, barrelGO, sRailGO, eRailGO, oRailGO;

    public float damage, reloadSpeed, recoilMultiplier, zoomOnAim;
    public int magazineSpace;

    private void Start()
    {
        if(scopeGO != null)
            changeAttachment(scopeGO.GetComponent<ObjectsManager>());

        if(magazineGO != null)        
            changeAttachment(magazineGO.GetComponent<ObjectsManager>());
                 
        if(barrelGO != null)
            changeAttachment(barrelGO.GetComponent<ObjectsManager>());

        if(sRailGO != null)
            changeAttachment(sRailGO.GetComponent<ObjectsManager>());

        if (eRailGO != null)
            changeAttachment(eRailGO.GetComponent<ObjectsManager>());

        if (oRailGO != null)
            changeAttachment(oRailGO.GetComponent<ObjectsManager>());
    }

    //thism ethod changes the attachment from the current one to a new one changing the stats as well
    public void changeAttachment(ObjectsManager item)
    {
        if(item.gameObject.GetComponent<attachment>())
        {
            attachment att = item.gameObject.GetComponent<attachment>();
            switch (att.attachmentTypes)
            {
                case attachment.AttachmentTypes.scope:
                    if (scopeGO != null)
                    {
                        detachAttachment(scopeGO);                       
                    }
                   
                    addAttachment(item.gameObject, scopeTransform);
                    
                    break;

                case attachment.AttachmentTypes.magazine:
                    if (magazineGO != null)
                    {
                        detachAttachment(magazineGO);
                    }
                    addAttachment(item.gameObject, magazineTransform);
                    updateStats(att);               
                    magazineSpace = att.magazineSpace; 
                    this.GetComponent<gunManager>().ammoType = magazineGO.GetComponent<attachment>().ammoType;
                    this.GetComponent<gunManager>().bulletShellGo = att.bulletShell;
                    this.GetComponent<gunManager>().bulletGo = att.bulletShell;
                    this.GetComponent<gunManager>().magazineSpace = magazineSpace;

                    break;

                case attachment.AttachmentTypes.barrel:
                    if (barrelGO != null)
                    {
                        detachAttachment(barrelGO);
                    }
                    addAttachment(item.gameObject, barrelTransform);
                    updateStats(att,false);
                    break;

                case attachment.AttachmentTypes.eRail:
                    if (eRailGO != null)
                    {
                        detachAttachment(eRailGO);
                    }
                    addAttachment(item.gameObject, eRailTransform);
                    updateStats(att, false);
                    break;

                case attachment.AttachmentTypes.sRail:
                    if (sRailGO != null)
                    {
                        detachAttachment(sRailGO);
                    }
                    addAttachment(item.gameObject, sRailTransform);
                    updateStats(att, false);
                    break;

                case attachment.AttachmentTypes.oRail:
                    if (oRailGO != null)
                    {
                        detachAttachment(oRailGO);
                    }
                    addAttachment(item.gameObject, oRailTransform);
                    updateStats(att, false);
                    break;
                default:

                    break;
            }
        }
        
    }

    public void addAttachment(GameObject g, Transform newPosition)
    {      
        g.transform.parent = newPosition.transform;
        g.transform.localPosition = new Vector3(0f, 0f, 0f);
        g.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);   
    }

    public void addAttachment(GameObject g, Transform newPosition, Vector3 rescaleVec, bool rescale = false)
    {
        addAttachment(g, newPosition);
        if (rescale)
            g.transform.localScale = rescaleVec;
    }

    public void updateStats(attachment gj, bool emplace = true)
    {
        if (emplace)
        {
            damage = gj.GetComponent<attachment>().damage;
            reloadSpeed = gj.GetComponent<attachment>().reloadSpeed;
            recoilMultiplier = gj.GetComponent<attachment>().recoilMultiplier;           
        }
        else
        {
            damage += gj.GetComponent<attachment>().damage;
            reloadSpeed += gj.GetComponent<attachment>().reloadSpeed;
            recoilMultiplier += gj.GetComponent<attachment>().recoilMultiplier;
        }
       
    }

    public void detachAttachment(GameObject item)
    {
        item.transform.SetParent(null);
        if (GetComponentInParent<InventoryManager>())
        {
            GetComponentInParent<InventoryManager>().addItemToInventory(item.GetComponent<ObjectsManager>());
        }        
    }
}
