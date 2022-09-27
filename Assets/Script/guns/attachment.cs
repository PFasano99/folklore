using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attachment : MonoBehaviour
{
    public enum AttachmentTypes { scope, magazine, barrel, sRail, eRail, stock };
    public AttachmentTypes attachmentTypes;

    public float damage, reloadSpeed, recoilMultiplier, zoomOnAim, mobility, fireRateo, range;
    public int magazineSpace;

    public bulletType.AmmoType ammoType;
    public GameObject bulletShell;    

}
