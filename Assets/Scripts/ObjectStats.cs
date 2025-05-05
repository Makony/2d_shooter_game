using System;
using UnityEngine;

public class ObjectStats : MonoBehaviour
{
    public float Health = 100f; 
    public float Ammo = 100f;       //03.05 by A: I thought maybe it is better to have Ammo, Magazin Numbers and etc. in Shooting script instead, so we don't need to call this scrip that much.
    //Minimum 0 
    public float Armour = 1f; //Minimum 1. It can be as high as you much but don't forget (real dmg = dmg/Armour)
    public float HealthGenerator = 0f; //I guess it can be negative if you kill objects that you shouldn't destroy (?). Like hostages (?)

    //public Boolean enemy = true;    //03.05, A: I had this before but I don't know why anymore. It is really not needed because Enemys got "Enemy" tag anyway.
}
