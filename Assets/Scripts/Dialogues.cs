using System;
using UnityEngine;

public class Dialogues : MonoBehaviour
{
    public static Dialogues Instance { get; private set; }

    private int index;
    private string[] messages;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

}
