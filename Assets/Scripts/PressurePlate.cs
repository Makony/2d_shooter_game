using UnityEngine;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour
{
    public float triggerRange = 15f;   // Range within which bows will be triggered.
    private List<GameObject> bows;     // Store all active bows from BowTrapsManager.
    private List<ArrowShooter> bowsInRange = new List<ArrowShooter>(); //list of nearby ArrowShooters.

    private bool playerIsOnPlate = false;

    public void CalculateBowsInRange()
    {
        ArrowShooter[] allBowsInScene = FindObjectsOfType<ArrowShooter>();

        foreach (ArrowShooter bow in allBowsInScene)
        {
            float distance = Vector2.Distance(transform.position, bow.transform.position);

            if (distance <= triggerRange)
            {
                bowsInRange.Add(bow);
            }
        }
    }

    void Update()
    {
        if (playerIsOnPlate)
        {
            TriggerBows();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player")) 
        {
            playerIsOnPlate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOnPlate = false;
        }
    }

    private void TriggerBows()
    {
        foreach (ArrowShooter arrowShooter in bowsInRange)
        {
            arrowShooter.ShootArrow(transform.position);
        }

        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject);
    }
}