using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel3 : MonoBehaviour
{
    public static FinishLevel3 Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && LevelManager.Instance.HasAllKillCodes && LevelManager.Instance.RemainingTime <=0)
        {
            DialogManager.Instance.EndLevel3();
        }
    }
}