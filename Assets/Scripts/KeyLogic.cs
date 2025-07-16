using UnityEngine;

public class KeyLogic : MonoBehaviour
{
    private bool isPlayerTouching = false;

    void Update()
    {
        if (isPlayerTouching && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerTouching = false;
        }
    }

    private void PickUp()
    {
        switch (gameObject.tag)
        {
            case "Key1":
                LevelManager.Instance.Key1 = true;
                break;
            case "Key2":
                LevelManager.Instance.Key2 = true;
                break;
            case "Key3":
                LevelManager.Instance.Key3 = true;
                break;
            case "Key4":
                LevelManager.Instance.Key4 = true;
                break;
            case "KillCode":
                LevelManager.Instance.HasAllKillCodes = true;
                break;
        }
        LevelManager.Instance.KillCodeStat();
        LevelManager.Instance.KeysStat();
        SoundManager.Instance.KeyCollectSound();
        Destroy(gameObject);
    }
}