using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel1 : MonoBehaviour
{
    // This is called when another collider enters the trigger zone.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it was the player.
        if (other.CompareTag("Player"))
        {
            // Tell the current LevelManager to save the player's stats.
            LevelManager.Instance.SavePlayerStats();

            // Load Level 2.
            SceneManager.LoadScene("Level2");
        }
    }
}