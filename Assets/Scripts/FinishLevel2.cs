using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinishLevel2 : MonoBehaviour
{
    private bool hasBeenTriggered = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenTriggered) return;
        if (other.CompareTag("Player"))
        {
            hasBeenTriggered = true;
            StartCoroutine(LoadNextLevelSequence());
        }
    }
    
    private IEnumerator LoadNextLevelSequence()
    {
        LevelManager.Instance.SavePlayerStats();
        SoundManager.Instance.ButtonClickSound();
        
        yield return new WaitForSeconds(2f);
        
       SceneManager.LoadScene("Level3");
    }
}