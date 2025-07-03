using UnityEngine;
using System.Collections.Generic;
using System;

public class JPS
{
    private Grid grid;

    public JPS(Grid grid)
    {
        this.grid = grid;
    }

    public List<Node> FindPath(Node start, Node goal)
    {
        PriorityQueue<Node> openList = new PriorityQueue<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        Dictionary<Node, double> gScores = new Dictionary<Node, double>();

        start.SetG(0);
        start.SetF(Heuristic(start, goal));
        openList.Enqueue(start, start.GetF());
        gScores[start] = 0;

        while (openList.Count > 0)
        {
            Node current = openList.Dequeue();

            if (current.Equals(goal))
            {
                return ReconstructPath(goal);
            }

            closedList.Add(current);

            foreach (Node successor in IdentifySuccessors(current, goal))
            {
                if (closedList.Contains(successor)) continue;

                double tentativeG = gScores.GetValueOrDefault(current, double.MaxValue) +
                                    Distance(current, successor);

                if (!gScores.ContainsKey(successor) || tentativeG < gScores[successor])
                {
                    gScores[successor] = tentativeG;
                    successor.SetG(tentativeG);
                    successor.SetF(tentativeG + Heuristic(successor, goal));
                    successor.SetParent(current);

                    if (!openList.Contains(successor))
                    {
                        openList.Enqueue(successor, successor.GetF());
                    }
                    else
                    {
                        openList.UpdatePriority(successor, successor.GetF());
                    }
                }
            }
        }

        return new List<Node>(); // No path found
    }

    private List<Node> IdentifySuccessors(Node current, Node goal)
    {
        List<Node> successors = new List<Node>();
        Node parent = current.GetParent();
        int dx = 0, dy = 0;

        if (parent != null)
        {
            dx = Math.Sign(current.GetX() - parent.GetX());
            dy = Math.Sign(current.GetY() - parent.GetY());
        }

        var directions = PruneDirections(dx, dy);


        foreach (int[] dir in directions)
        {
            Node jumpPoint = Jump(current, goal, dir[0], dir[1]);
            if (jumpPoint != null)
            {
                successors.Add(jumpPoint);

            }
        }

        return successors;
    }


    /*private Node Jump(Node current, Node goal, int dx, int dy)
    {
        int x = current.GetX() + dx;
        int y = current.GetY() + dy;

        if (!grid.IsWalkable(x, y))
            return null;

        Node node = grid.GetNode(x, y);
        if (node == null)
            return null;

        if (node.Equals(goal))
            return node;

        // Check for "No Corner Cutting"
        if (dx != 0 && dy != 0)
        {
            if (!grid.IsWalkable(x, current.GetY()) &&
                !grid.IsWalkable(current.GetX(), y))
            {
                return null;
            }
        }

        // Forced neighbor checks
        if (dx != 0 && dy != 0)  // Diagonal movement
        {
            if ((!grid.IsWalkable(x - dx, y) && grid.IsWalkable(x - dx, y + dy)) ||
                (!grid.IsWalkable(x, y - dy) && grid.IsWalkable(x + dx, y - dy)))
            {
                return node;
            }
        }
        else if (dx != 0)  // Horizontal movement
        {
            if ((!grid.IsWalkable(x, y + 1) && grid.IsWalkable(x + dx, y + 1)) ||
                (!grid.IsWalkable(x, y - 1) && grid.IsWalkable(x + dx, y - 1)))
            {
                return node;
            }
        }
        else if (dy != 0)  // Vertical movement
        {
            if ((!grid.IsWalkable(x + 1, y) && grid.IsWalkable(x + 1, y + dy)) ||
                (!grid.IsWalkable(x - 1, y) && grid.IsWalkable(x - 1, y + dy)))
            {
                return node;
            }
        }

        // Recursive jumps with boundary checks
        if (dx != 0 && dy != 0)
        {
            // Check horizontal and vertical jumps
            if (Jump(node, goal, dx, 0) != null || Jump(node, goal, 0, dy) != null)
                return node;
        }


        return Jump(node, goal, dx, dy);
    }*/

    private Node Jump(Node current, Node goal, int dx, int dy)
{
    int x = current.GetX() + dx;
    int y = current.GetY() + dy;

    // Check if next position is within grid bounds
    if (x < 0 || x >= grid.GetWidth() || y < 0 || y >= grid.GetHeight())
        return null;

    Node node = grid.GetNode(x, y);
    
    // Add null checks
    if (node == null || !node.IsWalkable())
        return null;

    if (node.Equals(goal))
        return node;

    // Check for "No Corner Cutting" with boundary checks
    if (dx != 0 && dy != 0)
    {
        if ((x - dx >= 0 && x - dx < grid.GetWidth() && 
             !grid.IsWalkable(x, current.GetY())) &&
            (y - dy >= 0 && y - dy < grid.GetHeight() && 
             !grid.IsWalkable(current.GetX(), y)))
        {
            return null;
        }
    }

    // Forced neighbor checks with boundary safety
    if (dx != 0 && dy != 0)  // Diagonal movement
    {
        // Check left neighbor
        if (x - dx >= 0 && !grid.IsWalkable(x - dx, y) && 
            x + dx < grid.GetWidth() && grid.IsWalkable(x + dx, y))
        {
            return node;
        }
        // Check bottom neighbor
        if (y - dy >= 0 && !grid.IsWalkable(x, y - dy) && 
            y + dy < grid.GetHeight() && grid.IsWalkable(x, y + dy))
        {
            return node;
        }
    }
    else if (dx != 0)  // Horizontal movement
    {
        // Check top neighbor
        if (y + 1 < grid.GetHeight() && !grid.IsWalkable(x, y + 1) && 
            x + dx < grid.GetWidth() && grid.IsWalkable(x + dx, y + 1))
        {
            return node;
        }
        // Check bottom neighbor
        if (y - 1 >= 0 && !grid.IsWalkable(x, y - 1) && 
            x + dx < grid.GetWidth() && grid.IsWalkable(x + dx, y - 1))
        {
            return node;
        }
    }
    else if (dy != 0)  // Vertical movement
    {
        // Check right neighbor
        if (x + 1 < grid.GetWidth() && !grid.IsWalkable(x + 1, y) && 
            y + dy < grid.GetHeight() && grid.IsWalkable(x + 1, y + dy))
        {
            return node;
        }
        // Check left neighbor
        if (x - 1 >= 0 && !grid.IsWalkable(x - 1, y) && 
            y + dy < grid.GetHeight() && grid.IsWalkable(x - 1, y + dy))
        {
            return node;
        }
    }

    // Recursive jumps with boundary checks
    if (dx != 0 && dy != 0)
    {
        // Check horizontal and vertical jumps only if within bounds
        if ((dx != 0 && Jump(node, goal, dx, 0) != null) ||
            (dy != 0 && Jump(node, goal, 0, dy) != null))
        {
            return node;
        }
    }

    // Only continue if next position is within bounds
    if (x + dx >= 0 && x + dx < grid.GetWidth() && 
        y + dy >= 0 && y + dy < grid.GetHeight())
    {
        return Jump(node, goal, dx, dy);
    }
    
    return null;
}

    private List<int[]> PruneDirections(int dx, int dy)
    {
        if (dx == 0 && dy == 0)
        {
            //  all 8 directions for start node
            return new List<int[]>
        {
            new[] { 1, 0 }, new[] { -1, 0 }, new[] { 0, 1 }, new[] { 0, -1 },
            new[] { 1, 1 }, new[] { -1, 1 }, new[] { 1, -1 }, new[] { -1, -1 }
        };
        }

        List<int[]> dirs = new List<int[]>();

        if (dx != 0 && dy != 0)
        {
            // Diagonal
            dirs.Add(new[] { dx, dy });
            dirs.Add(new[] { dx, 0 });
            dirs.Add(new[] { 0, dy });
        }
        else if (dx != 0)
        {
            // Horizontal
            dirs.Add(new[] { dx, 0 });
            dirs.Add(new[] { dx, 1 });
            dirs.Add(new[] { dx, -1 });
        }
        else if (dy != 0)
        {
            // Vertical
            dirs.Add(new[] { 0, dy });
            dirs.Add(new[] { 1, dy });
            dirs.Add(new[] { -1, dy });
        }

        return dirs;
    }


    private double Heuristic(Node a, Node b)
    {
        double dx = Mathf.Abs(a.GetX() - b.GetX());
        double dy = Mathf.Abs(a.GetY() - b.GetY());
        return (dx + dy) + (Mathf.Sqrt(2) - 2) * Mathf.Min((float)dx, (float)dy);
    }

    private double Distance(Node a, Node b)
    {
        return Math.Sqrt(Math.Pow(a.GetX() - b.GetX(), 2) + Math.Pow(a.GetY() - b.GetY(), 2));
    }

    private List<Node> ReconstructPath(Node goal)
    {
        LinkedList<Node> path = new LinkedList<Node>();
        Node current = goal;

        while (current != null)
        {
            Node parent = current.GetParent();
            if (parent != null)
            {
                path.AddFirst(current);
                foreach (var n in CompletePath(parent, current))
                {
                    path.AddFirst(n);
                }
            }
            else
            {
                path.AddFirst(current);
            }
            current = parent;
        }

        return new List<Node>(path);
    }

    private List<Node> CompletePath(Node from, Node to)
    {
        List<Node> path = new List<Node>();
        int x0 = from.GetX();
        int y0 = from.GetY();
        int x1 = to.GetX();
        int y1 = to.GetY();

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }

            // Stop before reaching target node
            if (x0 == to.GetX() && y0 == to.GetY())
                break;

            Node node = grid.GetNode(x0, y0);
            if (node != null)
            {
                path.Add(node);
            }
        }

        return path;
    }
}
