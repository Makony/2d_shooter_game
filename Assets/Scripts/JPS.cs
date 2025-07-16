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
        
        // Reset the start node for the new search.
        start.ResetPathData();
        start.SetG(0);
        start.SetF(Heuristic(start, goal));
        openList.Enqueue(start, start.GetF());
        gScores[start] = 0;

        while (openList.Count > 0)
        {
            Node current = openList.Dequeue();

            if (current.Equals(goal))
            {
                // Path found, reconstruct and return it.
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
                    // Discovered a new or better path to the successor.
                    gScores[successor] = tentativeG;
                    successor.SetParent(current);
                    successor.SetG(tentativeG);
                    successor.SetF(tentativeG + Heuristic(successor, goal));

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

        return new List<Node>();
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
        //  Determine the next position to check
        int nextX = current.GetX() + dx;
        int nextY = current.GetY() + dy;

        // Check for boundaries and obstacles
        // If the next node is off the grid or not walkable, this path is a dead end.
        if (!grid.IsWalkable(nextX, nextY))
        {
            return null;
        }

        Node nextNode = grid.GetNode(nextX, nextY);

        // Goal Check
        // If we've reached the goal, we've found a jump point.
        if (nextNode.Equals(goal))
        {
            return nextNode;
        }

        // Forced Neighbor Checks
        if (dx != 0 && dy != 0) // DIAGONAL Movement
        {
            // For diagonal moves, we check for jump points by recursively jumping
            // horizontally and vertically from the next node. If either finds a
            // jump point, then our current 'nextNode' is also a jump point.
            if (Jump(nextNode, goal, dx, 0) != null || Jump(nextNode, goal, 0, dy) != null)
            {
                return nextNode;
            }
        }
        else // STRAIGHT Movement (Horizontal or Vertical)
        {
            if (dx != 0) // Horizontal
            {
                // Check for an obstacle directly above or below the 'current' node that
                // would force a diagonal move.
                // Example: Moving right, obstacle at (current.x, current.y + 1)
                // and the space at (current.x + dx, current.y + 1) is open.
                if ((!grid.IsWalkable(current.GetX(), nextY + 1) && grid.IsWalkable(nextX, nextY + 1)) ||
                    (!grid.IsWalkable(current.GetX(), nextY - 1) && grid.IsWalkable(nextX, nextY - 1)))
                {
                    return nextNode;
                }
            }
            else // Vertical (dy != 0)
            {
                // Check for an obstacle directly to the left or right of the 'current' node.
                // Example: Moving up, obstacle at (current.x + 1, current.y)
                // and the space at (current.x + 1, current.y + dy) is open.
                if ((!grid.IsWalkable(nextX + 1, current.GetY()) && grid.IsWalkable(nextX + 1, nextY)) ||
                    (!grid.IsWalkable(nextX - 1, current.GetY()) && grid.IsWalkable(nextX - 1, nextY)))
                {
                    return nextNode;
                }
            }
        }

        // Recursive Jump
        // If no jump point has been found, continue jumping in the same direction.
        return Jump(nextNode, goal, dx, dy);
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
                
                List<Node> intermediateNodes = CompletePath(parent, current);
                for (int i = intermediateNodes.Count - 1; i >= 0; i--)
                {
                    path.AddFirst(intermediateNodes[i]);
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
