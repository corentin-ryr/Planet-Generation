using System;
using UnityEngine;

namespace Geometry
{
    public struct Edge
    {
        public int p1;
        public int p2;

        public Edge(int p1, int p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Edge edge = (Edge)obj;
                return ((p1 == edge.p1) && (p2 == edge.p2)) || ((p2 == edge.p1) && (p1 == edge.p2)) || edge.p1 == edge.p2;
            }
        }

        public override string ToString() => $"({p1}, {p2})";
    }

    public struct TriangleIndices
    {
        public int v1;
        public int v2;
        public int v3;
        public TriangleIndices(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

    }

}