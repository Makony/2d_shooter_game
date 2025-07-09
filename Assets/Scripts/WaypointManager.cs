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

    public Transform WaypointAtIndex(int index)
    {
        if (index >= 0 && index < WaypointAmount())
        {
            return transform.GetChild(index);
        }
        else return null;
    }
    
    public void AddWaypoint(Vector2 position)
    {
        GameObject waypoint = new GameObject("Waypoint");
        waypoint.transform.position = position;
        waypoint.transform.parent = transform;
    }
}
