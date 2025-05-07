using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public int WaypointAmount() 
    {
        return transform.childCount;
    }

    public Transform WaypointAtIndex (int index)
    {
        if (index >= 0 && index < WaypointAmount())
        {
            return transform.GetChild(index);
        }
        else return null;
    }
}
