using System;
using UnityEngine;

public class ObjectStats : MonoBehaviour
{
    public float Health = 100f; //Minimum 0 
    public float Armour = 1f; //Minimum 1. It can be as high as you much but don't forget (real dmg = dmg/Armour)
    public float HealthGenerator = 0f; //I guess it can be negative if you kill objects that you shouldn't destroy (?). Like hostages (?)

    public Boolean enemy = true;    //03.05, A: I had this before but I don't know why anymore. It is really not needed because Enemys got "Enemy" tag anyway.



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
