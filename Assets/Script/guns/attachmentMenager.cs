using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class attachmentMenager : MonoBehaviour
{       
    public Transform scopeTransform, magazineTransform, barrelTransform, sRailTransform, eRailTransform, stockTransform, bulletShellTransform;
    public GameObject scopeGO, magazineGO, barrelGO, sRailGO, eRailGO, stockGO;

    public float damage, reloadSpeed, recoilMultiplier, zoomOnAim, mobility, fireRateo, range;
    public int magazineSpace;

    [Space]
    public int maxQuickInventory = 2;
    [Space]
    public ObjectsManager[] scopeQuick;
    public ObjectsManager[] magazineQuick;
    public ObjectsManager[] barrellQuick;
    public ObjectsManager[] sRailQuick;
    public ObjectsManager[] eRailQuick;
    public ObjectsManager[] stockQuick;

    [Space]
    [Header("Hud quick attachment")]
    public Image weaponHud;
    public bool isShown = false;
    public Button[] scopeBtn;
    public Button[] magazineBtn;
    public Button[] barrellBtn;
    public Button[] sRailBtn;
    public Button[] eRailBtn;
    public Button[] stockBtn;

    private void Start()
    {
        weaponHud.gameObject.SetActive(false);

        if (scopeTransform.childCount > 0)
        {
            scopeGO = scopeTransform.GetChild(0).gameObject;
            changeAttachment(scopeGO.GetComponent<ObjectsManager>());
        }

        if (magazineTransform.childCount > 0)
        {
            magazineGO = magazineTransform.GetChild(0).gameObject;
            changeAttachment(magazineGO.GetComponent<ObjectsManager>());
        }

        if (barrelTransform.childCount > 0)
        {
            barrelGO = barrelTransform.GetChild(0).gameObject;
            changeAttachment(barrelGO.GetComponent<ObjectsManager>());
        }

        if (sRailTransform.childCount > 0)
        {
            sRailGO = sRailTransform.GetChild(0).gameObject;
            changeAttachment(sRailGO.GetComponent<ObjectsManager>());
        }

        if (eRailTransform.childCount > 0)
        {
            eRailGO = eRailTransform.GetChild(0).gameObject;
            changeAttachment(eRailGO.GetComponent<ObjectsManager>());
        }

        if (stockTransform.childCount > 0)
        {
            stockGO = stockTransform.GetChild(0).gameObject;
            changeAttachment(stockGO.GetComponent<ObjectsManager>());
        }     

        hudStart();

        scopeBtn[0].GetComponent<Button>();
        scopeBtn[0].onClick.AddListener(delegate {
            ObjectsManager toChange = scopeGO.GetComponent<ObjectsManager>();
            changeAttachment(scopeQuick[0]);
            scopeQuick[0] = toChange.GetComponent<ObjectsManager>(); 
        });
        
        scopeBtn[1].GetComponent<Button>();
        scopeBtn[1].onClick.AddListener(delegate {
            ObjectsManager toChange = scopeGO.GetComponent<ObjectsManager>();
            changeAttachment(scopeQuick[1]);
            scopeQuick[1] = toChange.GetComponent<ObjectsManager>(); 
        });

        scopeBtn[2].GetComponent<Button>();
        scopeBtn[2].onClick.AddListener(delegate {
            ObjectsManager toChange = scopeGO.GetComponent<ObjectsManager>();
            changeAttachment(scopeQuick[2]);
            scopeQuick[2] = toChange.GetComponent<ObjectsManager>();
        });


        magazineBtn[0].GetComponent<Button>();
        magazineBtn[0].onClick.AddListener(delegate {
            ObjectsManager toChange = magazineGO.GetComponent<ObjectsManager>();
            changeAttachment(magazineQuick[0]);
            magazineQuick[0] = toChange.GetComponent<ObjectsManager>();
        });

        magazineBtn[1].GetComponent<Button>();
        magazineBtn[1].onClick.AddListener(delegate {
            ObjectsManager toChange = magazineGO.GetComponent<ObjectsManager>();
            changeAttachment(magazineQuick[1]);
            magazineQuick[1] = toChange.GetComponent<ObjectsManager>();
        });

        magazineBtn[2].GetComponent<Button>();
        magazineBtn[2].onClick.AddListener(delegate {
            ObjectsManager toChange = magazineGO.GetComponent<ObjectsManager>();
            changeAttachment(magazineQuick[2]);
            magazineQuick[2] = toChange.GetComponent<ObjectsManager>();
        });


    }

    //this method changes the attachment from the current one to a new one changing the stats as well
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
                    scopeGO = item.gameObject;
                    break;

                case attachment.AttachmentTypes.magazine:
                    if (magazineGO != null)
                    {
                        detachAttachment(magazineGO);
                    }
                    addAttachment(item.gameObject, magazineTransform);
                    updateStats(att);               
                    magazineSpace = att.magazineSpace;
                    magazineGO.GetComponent<ObjectsManager>().quantity = GetComponent<gunManager>().ammoInMagazine;
                    magazineGO = item.gameObject;
                    this.GetComponent<gunManager>().ammoType = magazineGO.GetComponent<attachment>().ammoType;
                    this.GetComponent<gunManager>().bulletShellGo = att.bulletShell;
                    this.GetComponent<gunManager>().bulletGo = att.bulletShell;
                    this.GetComponent<gunManager>().magazineSpace = magazineSpace;

                    if (GetComponentInParent<InventoryManager>())
                    {                      
                        GetComponentInParent<InventoryManager>().updateAmmolist(item);                       
                        GetComponent<gunManager>().ammoInMagazine = magazineGO.GetComponent<ObjectsManager>().quantity;
                        GetComponentInParent<InventoryManager>().mng.updateText(GetComponentInParent<InventoryManager>().mng.magazine, ""+GetComponent<gunManager>().ammoInMagazine);
                        GetComponentInParent<InventoryManager>().addActiveAmmo(GetComponent<gunManager>().gameObject.GetComponent<ObjectsManager>());
                    }
                        

                    break;

                case attachment.AttachmentTypes.barrel:
                    if (barrelGO != null)
                    {
                        detachAttachment(barrelGO);
                    }
                    addAttachment(item.gameObject, barrelTransform);
                    barrelGO = item.gameObject;
                    updateStats(att,false);

                    break;

                case attachment.AttachmentTypes.eRail:
                    if (eRailGO != null)
                    {
                        detachAttachment(eRailGO);
                    }
                    addAttachment(item.gameObject, eRailTransform);
                    eRailGO = item.gameObject;
                    updateStats(att, false);
                    break;

                case attachment.AttachmentTypes.sRail:
                    if (sRailGO != null)
                    {
                        detachAttachment(sRailGO);
                    }
                    addAttachment(item.gameObject, sRailTransform);
                    sRailGO = item.gameObject;
                    updateStats(att, false);
                    break;

                case attachment.AttachmentTypes.stock:
                    if (stockGO != null)
                    {
                        detachAttachment(stockGO);
                    }
                    addAttachment(item.gameObject, stockTransform);
                    stockGO = item.gameObject;
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
        else
        {
            item.gameObject.transform.position = new Vector3(gameObject.transform.position.x + 1, 1, gameObject.transform.position.z + 1);
        }
    }

    //this method shows the panel for the quick inventory attachments changes
    public void panelShow()
    {
        if (isShown)
        {
            weaponHud.gameObject.SetActive(false);
            isShown = false;
            Cursor.lockState = CursorLockMode.Locked;
            transform.localPosition = GetComponent<gunManager>().holdPosition;          
            gameObject.transform.localEulerAngles = GetComponent<gunManager>().holdOffsetRotation;
            GetComponentInParent<playerRotation>().canRotate = true;

        }
        else
        {
            weaponHud.gameObject.SetActive(true);
            isShown = true;
            Cursor.lockState = CursorLockMode.None;
            gameObject.transform.localPosition = new Vector3(.3f,0,.90f);           
            gameObject.transform.localEulerAngles = new Vector3(0f,-90f,0f);
            GetComponentInParent<playerRotation>().canRotate = false;
        }
    }

    private void hudStart()
    {
        if (maxQuickInventory == 0)
        {
            for(int i = 0; i < scopeBtn.Length; i++)
            {
                scopeBtn[i].gameObject.SetActive(false);
                magazineBtn[i].gameObject.SetActive(false);
                barrellBtn[i].gameObject.SetActive(false);
                sRailBtn[i].gameObject.SetActive(false);
                eRailBtn[i].gameObject.SetActive(false);
                stockBtn[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = scopeBtn.Length-1; i >= maxQuickInventory; i--)
            {
                if (scopeBtn[i] != null)
                    scopeBtn[i].gameObject.SetActive(false);
                if (magazineBtn[i] != null)
                    magazineBtn[i].gameObject.SetActive(false);
                if (barrellBtn[i] != null)
                    barrellBtn[i].gameObject.SetActive(false);
                if (sRailBtn[i] != null)
                    sRailBtn[i].gameObject.SetActive(false);
                if (eRailBtn[i] != null)
                    eRailBtn[i].gameObject.SetActive(false);
                if (stockBtn[i] != null)
                    stockBtn[i].gameObject.SetActive(false);
            }
        }
    }
}
