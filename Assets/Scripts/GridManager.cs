using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 20;
    public int height = 15;
    public float nodeSize = 1f;
    public LayerMask obstacleMask;

    private Grid grid;
    
    public Grid GetGrid() => grid;

    void Awake()
    {
        // Create the grid using regular C# constructor
        grid = new Grid(width, height);
        ScanForObstacles();
    }

    void ScanForObstacles()
    {
        Vector2 worldBottomLeft = (Vector2)transform.position - 
                                 Vector2.right * width * nodeSize / 2 - 
                                 Vector2.up * height * nodeSize / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPoint = worldBottomLeft + 
                    Vector2.right * (x * nodeSize + nodeSize / 2) + 
                    Vector2.up * (y * nodeSize + nodeSize / 2);
                
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeSize / 2, obstacleMask);
                grid.SetWalkable(x, y, walkable);
            }
        }
    }

    public Node WorldToNode(Vector3 position)
    {
        Vector2 localPos = (Vector2)position - (Vector2)transform.position;
        int x = Mathf.RoundToInt(localPos.x / nodeSize + width / 2);
        int y = Mathf.RoundToInt(localPos.y / nodeSize + height / 2);
        
        return grid.GetNode(x, y);
    }

    public Vector2 NodeToWorld(Node node)
    {
        if (node == null) return Vector2.zero;
        
        return new Vector2(
            (node.GetX() - width / 2) * nodeSize,
            (node.GetY() - height / 2) * nodeSize
        ) + (Vector2)transform.position;
    }

    void OnDrawGizmos()
    {
        Vector2 worldBottomLeft = (Vector2)transform.position - 
                                Vector2.right * width * nodeSize / 2 - 
                                Vector2.up * height * nodeSize / 2;

        Gizmos.color = Color.gray;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPoint = worldBottomLeft + 
                    Vector2.right * (x * nodeSize + nodeSize / 2) + 
                    Vector2.up * (y * nodeSize + nodeSize / 2);
                
                Gizmos.DrawWireCube(worldPoint, new Vector3(nodeSize, nodeSize, 0));
                
                // Visualize unwalkable nodes
                if (grid != null && grid.GetNode(x, y) != null && !grid.GetNode(x, y).IsWalkable())
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(worldPoint, new Vector3(nodeSize, nodeSize, 0));
                    Gizmos.color = Color.gray;
                }
            }
        }
    }
}