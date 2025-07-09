using UnityEngine;
using System.Collections.Generic;

public class BowTrapsManager : MonoBehaviour
{
    public static BowTrapsManager Instance;
    public List<GameObject> bows = new List<GameObject>();

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
    
    public void AddBow(GameObject bow)
    {
        bows.Add(bow);
    }
}
