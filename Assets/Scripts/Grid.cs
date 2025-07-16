using System.Collections.Generic;

public class Grid
{
    private int width;
    private int height;
    private Node[,] grid;

    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;
        grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Node(x, y);
            }
        }
    }

    public int GetWidth() => width;
    public int GetHeight() => height;

    public Node GetNode(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return grid[x, y];
        return null;
    }

    public void SetWalkable(int x, int y, bool walkable)
    {
        Node node = GetNode(x, y);
        if (node != null) node.SetWalkable(walkable);
    }

    public bool IsWalkable(int x, int y)
    {
        Node node = GetNode(x, y);
        return node != null && node.IsWalkable();
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int[,] directions = {
            {-1, 0}, {1, 0}, {0, -1}, {0, 1},
            {-1, -1}, {-1, 1}, {1, -1}, {1, 1}
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int nx = node.GetX() + directions[i, 0];
            int ny = node.GetY() + directions[i, 1];
            Node neighbor = GetNode(nx, ny);
            if (neighbor != null && neighbor.IsWalkable())
                neighbors.Add(neighbor);
        }
        return neighbors;
    }

    public List<Node> GetAllNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int[,] directions = {
            {-1, 0}, {1, 0}, {0, -1}, {0, 1},
            {-1, -1}, {-1, 1}, {1, -1}, {1, 1}
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int nx = node.GetX() + directions[i, 0];
            int ny = node.GetY() + directions[i, 1];
            
            // Add the neighbor regardless of walkability, as long as it's on the grid.
            Node neighbor = GetNode(nx, ny);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }
}
