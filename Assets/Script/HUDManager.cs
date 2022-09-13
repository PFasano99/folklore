using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HUDManager : MonoBehaviour
{
    private GameObject player;

    public TMP_Text magazine, ammo, fireMode;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    public void updateText(TMP_Text TMP_t, string newText)
    {
        TMP_t.text = newText;
    }
}
