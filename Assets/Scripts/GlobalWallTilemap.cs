using UnityEngine;

public class GlobalWallTilemap : MonoBehaviour
{
    public static GlobalWallTilemap Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }
    
}
