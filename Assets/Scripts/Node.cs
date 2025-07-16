using System;
using UnityEngine;

public class Node : IComparable<Node>
{
        private int x;
        private int y;
        private bool walkable = true;

        private Node parent;
        private double g;
        private double f;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int GetX() => x;
        public int GetY() => y;

        public bool IsWalkable() => walkable;
        public void SetWalkable(bool value) => walkable = value;

        public Node GetParent() => parent;
        public void SetParent(Node value) => parent = value;

        public double GetG() => g;
        public void SetG(double value) => g = value;

        public double GetF() => f;
        public void SetF(double value) => f = value;

        public override bool Equals(object obj)
        {
            if (obj is Node node)
            {
                return x == node.x && y == node.y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y); // .NET built-in hash combiner
        }

        public int CompareTo(Node other)
        {
            return f.CompareTo(other.f);
        }

    public void ResetPathData()
    {
        SetParent(null);
        SetG(0);
        SetF(0);
    }
}
