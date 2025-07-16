using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines

public class FinishLevel3 : MonoBehaviour
{
    public static FinishLevel3 Instance { get; private set; }

    private bool hasBeenTriggered = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenTriggered) return;

        if (other.CompareTag("Player") && LevelManager.Instance.HasAllKillCodes && LevelManager.Instance.RemainingTime <= 0)
        {
            hasBeenTriggered = true;
            StartCoroutine(EndLevelSequence());
        }
    }
    private IEnumerator EndLevelSequence()
    {
        SoundManager.Instance.ButtonClickSound();
        
        yield return new WaitForSeconds(1f);
        
        DialogManager.Instance.EndLevel3();
    }
}