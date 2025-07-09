using UnityEngine;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour
{
    public float triggerRange = 30f;   // Range within which bows will be triggered.
    private List<GameObject> bows;     // Store all active bows from BowTrapsManager.
    private List<ArrowShooter> bowsInRange = new List<ArrowShooter>(); // Filtered list of nearby ArrowShooters.

    void Start()
    {
        // Get centralized list of all active "Bow" objects from BowTrapsManager.
        bows = BowTrapsManager.Instance.bows;

        CalculateBowsInRange(); // Precompute nearby ArrowShooters at startup.
    }

    private void CalculateBowsInRange()
    {
        foreach (GameObject bow in bows)
        {
            float distance = Vector2.Distance(transform.position, bow.transform.position);

            if (distance <= triggerRange) // Check if within range.
            {
                ArrowShooter arrowShooter = bow.GetComponent<ArrowShooter>();
                if (arrowShooter != null)
                {
                    bowsInRange.Add(arrowShooter); // Add valid ArrowShooters to filtered list.
                }
            }
        }

        Debug.Log($"Found {bowsInRange.Count} ArrowShooters within range.");
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player")) 
        {
            TriggerBows(); 
        }
    }

    private void TriggerBows()
    {
        foreach (ArrowShooter arrowShooter in bowsInRange)
        {
            arrowShooter.ShootArrow(transform.position); // Fire toward pressure plate's position.
            Debug.Log($"{arrowShooter.name} fired an arrow!");
        }
        
        Debug.Log($"Triggered {bowsInRange.Count} nearby ArrowShooters.");
    }
}