using UnityEngine;

public class SpawnpointManager : MonoBehaviour
{
    public static SpawnpointManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public int SpawnpointAmount()
    {
        return transform.childCount;
    }

    public Transform SpawnpointAtIndex(int index)
    {
        if (index >= 0 && index < SpawnpointAmount())
        {
            return transform.GetChild(index);
        }
        else return null;
    }
}
