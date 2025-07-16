using System;
using UnityEngine;

public class Level3TriggerForFight : MonoBehaviour
{
    public static Level3TriggerForFight Instance { get; private set; }

    private Boolean IsFirsttime = true;
    private Boolean isPlayerTouching = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }



    void Update()
    {
        if (isPlayerTouching && LevelManager.Instance.HasAllKillCodes)
        {
            doomDay();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("3");
            isPlayerTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("2");
            isPlayerTouching = false;
        }
    }



    private void doomDay()
    {
        if (!IsFirsttime) return;
        IsFirsttime = false;
        LevelManager.Instance.YouAreDoomeD();
        
        if (TryGetComponent<BoxCollider2D>(out var boxCollider))
        {
            boxCollider.enabled = false;
        }
    }
}
