using UnityEngine;

public class FinishingRoom : MonoBehaviour
{
    public static FinishingRoom Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }
}
