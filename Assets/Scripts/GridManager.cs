using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Debugging")]
    public bool showDebugGizmos = false;

    private Grid pathfindingGrid;
    private Vector3Int gridOrigin;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }
    
    public Node FindNearestWalkableNode(Node startNode)
    {
        if (startNode == null) return null;
        if (startNode.IsWalkable()) return startNode;

        Queue<Node> queue = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();

            foreach (Node neighbor in pathfindingGrid.GetAllNeighbors(currentNode))
            {
                if (!visited.Contains(neighbor))
                {
                    if (neighbor.IsWalkable())
                    {
                        return neighbor;
                    }
                    
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        return null; 
    }


    /* ain't know way I can fix it in time :D
    
    public void UpdateGridObstaclesForBounds(Bounds worldBounds, bool isWalkable)
    {
        if (pathfindingGrid == null) return;

        Node minNode = WorldToNode(worldBounds.min);
        Node maxNode = WorldToNode(worldBounds.max);

        if (minNode == null || maxNode == null) return;

        for (int x = minNode.GetX(); x <= maxNode.GetX(); x++)
        {
            for (int y = minNode.GetY(); y <= maxNode.GetY(); y++)
            {
                pathfindingGrid.GetNode(x, y)?.SetWalkable(isWalkable);
            }
        }
    }

    */
    
    public void CreateGridFromTilemaps(Tilemap wallTilemap, Tilemap floorTilemap)
    {
        wallTilemap.CompressBounds();
        BoundsInt bounds = wallTilemap.cellBounds;
        gridOrigin = bounds.min;
        pathfindingGrid = new Grid(bounds.size.x, bounds.size.y);
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int tilemapPosition = new Vector3Int(x + gridOrigin.x, y + gridOrigin.y, 0);
                bool hasFloor = floorTilemap.HasTile(tilemapPosition);
                bool hasWall = wallTilemap.HasTile(tilemapPosition);
                if (!hasFloor || hasWall)
                {
                    pathfindingGrid.GetNode(x, y)?.SetWalkable(false);
                    continue;
                }
                bool isTooCloseToBoundary = false;
                int[] dx = { 0, 0, 1, -1 };
                int[] dy = { 1, -1, 0, 0 };
                for (int i = 0; i < 4; i++)
                {
                    Vector3Int neighborPos = new Vector3Int(tilemapPosition.x + dx[i], tilemapPosition.y + dy[i], 0);
                    if (wallTilemap.HasTile(neighborPos) || !floorTilemap.HasTile(neighborPos))
                    {
                        isTooCloseToBoundary = true;
                        break;
                    }
                }
                pathfindingGrid.GetNode(x, y)?.SetWalkable(!isTooCloseToBoundary);
            }
        }
    }

    public Grid GetGrid()
    {
        return pathfindingGrid;
    }

    public Node WorldToNode(Vector3 worldPosition)
    {
        if (pathfindingGrid == null) return null;
        var tilemapGrid = GlobalWallTilemap.Instance.GetComponentInParent<UnityEngine.Grid>();
        Vector3Int cellPosition = tilemapGrid.WorldToCell(worldPosition);
        int gridX = cellPosition.x - gridOrigin.x;
        int gridY = cellPosition.y - gridOrigin.y;
        return pathfindingGrid.GetNode(gridX, gridY);
    }

    public Vector2 NodeToWorld(Node node)
    {
        if (pathfindingGrid == null || node == null) return Vector2.zero;
        Vector3Int cellPosition = new Vector3Int(node.GetX() + gridOrigin.x, node.GetY() + gridOrigin.y, 0);
        var tilemapGrid = GlobalWallTilemap.Instance.GetComponentInParent<UnityEngine.Grid>();
        return tilemapGrid.GetCellCenterWorld(cellPosition);
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugGizmos)
        {
            return;
        }
        if (pathfindingGrid == null) return;
        for (int x = 0; x < pathfindingGrid.GetWidth(); x++)
        {
            for (int y = 0; y < pathfindingGrid.GetHeight(); y++)
            {
                Node node = pathfindingGrid.GetNode(x, y);
                if (node != null)
                {
                    Gizmos.color = node.IsWalkable() ? new Color(0, 1, 0, 0.1f) : new Color(1, 0, 0, 0.2f);
                    Gizmos.DrawCube(NodeToWorld(node), Vector3.one * 0.8f);
                }
            }
        }
    }
}