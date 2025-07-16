using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class CCTVManager : MonoBehaviour
{
    public static CCTVManager Instance;
    public List<GameObject> cctvs = new List<GameObject>();

    private Boolean IsFirstTime = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCCTV(GameObject cctv)
    {
        cctvs.Add(cctv);
    }

   public void Detected()
    {
        if(!IsFirstTime){ return; }
        IsFirstTime = false;

        foreach (GameObject cctv in cctvs)
        {
            if (cctv != null)
            {
                Transform eye = cctv.transform.Find("eye");

                if (eye != null)
                {
                    // Get the SpriteRenderer component from the "eye".
                    eye.GetComponent<SpriteRenderer>().color = Color.red; ;
                }
                else
                {
                    Debug.Log("no eye :O");
                }

                PolygonCollider2D CCTVcollider = cctv.GetComponent<PolygonCollider2D>();

                if (CCTVcollider != null)
                {
                    CCTVcollider.enabled = false;
                }
                else
                {
                    Debug.LogWarning("The CCTV " + cctv.name + " is missing a SecurityCamera script.", cctv);
                }
            }
        }
    }
}

