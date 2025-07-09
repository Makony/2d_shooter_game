using UnityEngine;

public class GlobalFloorTilemap : MonoBehaviour
{
    public static GlobalFloorTilemap Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }
}
