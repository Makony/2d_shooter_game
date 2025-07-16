using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinishLevel1 : MonoBehaviour
{
    private bool hasBeenTriggered = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (hasBeenTriggered) return;

            hasBeenTriggered = true;
            StartCoroutine(LoadNextLevelSequence());
        }
    }

    private IEnumerator LoadNextLevelSequence()
    {
        LevelManager.Instance.SavePlayerStats();
        SoundManager.Instance.ButtonClickSound();
        
        yield return new WaitForSeconds(3f);
        
        SceneManager.LoadScene("Level2");
    }
}