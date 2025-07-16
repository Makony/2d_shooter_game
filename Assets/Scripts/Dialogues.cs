using System;
using UnityEngine;
using System.Collections;

public class Dialogues : MonoBehaviour
{
    public static Dialogues Instance { get; private set; }

    private Boolean IsDetectedAfter = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    void Update()
    {
        if (!IsDetectedAfter && LevelManager.Instance.IsDetected)
        {
            string message = "ALARM?! They found you! \n Just... gotta lick my fingers clean... wait a second";
            DialogManager.Instance.ShowDialogWithTimer(message);
            StartCoroutine(SendDialogueCorCoroutine(10f));

            message = "Sending it to your goggles now \n any seconds now \n Turn them on. DON'T PANIC :)";
            LevelManager.Instance.IsAllowedToSeePath = true;
        }
    }

    public void notdetected()
    {
        StartCoroutine(SendDialogueCorCoroutine(60f));
        if (!LevelManager.Instance.IsDetected)
        {
            IsDetectedAfter = true;
            string message = "Okay, that was a good exercise. Yummy, very Salty. \n Anyway, where were we? Ah, right, the path. Sending it to your goggles now \n any seconds now \n Turn them on. Don't be shy :)";
            DialogManager.Instance.ShowDialogWithTimer(message);
            LevelManager.Instance.IsAllowedToSeePath = true;
        }
    }

    private IEnumerator SendDialogueCorCoroutine(float t)
    {
        yield return new WaitForSeconds(t);
    }
}
