// PathfindingManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance;

    [Header("Grid Settings")]
    public int gridWidth = 200;  // Match your map size (10 tiles * 20 units)
    public int gridHeight = 200;
    public LayerMask collisionLayer;
    private float lastUpdateTime;
    public float dynamicUpdateInterval = 2f;
    private Grid pathfindingGrid;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Update dynamic obstacles at fixed intervals
        if (Time.time > lastUpdateTime + dynamicUpdateInterval)
        {
            UpdateDynamicObstacles();
            lastUpdateTime = Time.time;
        }
    }

    public void InitializeGrid()
    {
        pathfindingGrid = new Grid(gridWidth, gridHeight);

        // Scan for static obstacles (walls)
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPos = GridToWorldPosition(x, y);
                bool hasWall = Physics2D.OverlapPoint(worldPos, collisionLayer);
                pathfindingGrid.SetWalkable(x, y, !hasWall);
            }
        }

        UpdateDynamicObstacles();
    }

    public List<Node> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        // Get JPS instance from pool
        JPS jps = new JPS(pathfindingGrid);  // Always create new JPS
        List<Node> path = jps.FindPath(WorldToNode(startPos), WorldToNode(targetPos));
        Debug.Log($"JPS pathfinding from {startPos} to {targetPos} at time {Time.time}");


        return path;
    }

    private Node WorldToNode(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x);
        int y = Mathf.FloorToInt(worldPos.y);
        return pathfindingGrid.GetNode(x, y);
    }

    public Vector2 GridToWorldPosition(int x, int y)
    {
        return new Vector2(x + 0.5f, y + 0.5f);
    }

    public void UpdateDynamicObstacles()
    {
        // Find all dynamic obstacles
        GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");

        foreach (var obj in boxes.Concat(traps))
        {
            Vector2 pos = obj.transform.position;
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);

            // Only update if position is walkable in base grid
            if (pathfindingGrid.IsWalkable(x, y))
            {
                pathfindingGrid.SetWalkable(x, y, false);
            }
        }
    }
}