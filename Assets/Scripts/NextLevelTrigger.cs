using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTrigger : MonoBehaviour
{
    private bool isLoading = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isLoading)
        {
            isLoading = true;

            LevelManager.Instance.SavePlayerStats();

            SceneManager.LoadScene("Level2");
        }
    }
}