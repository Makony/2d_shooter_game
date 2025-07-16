using UnityEngine;

public class BoxManager : MonoBehaviour
{
    public static BoxManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }
}
