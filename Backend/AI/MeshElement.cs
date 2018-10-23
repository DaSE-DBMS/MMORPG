using System;
using System.Collections.Generic;
using System.Text;
using GeometRi;
using Common;

namespace Backend.AI
{
    public class MeshElement
    {
        public MeshElement[] neighbor = new MeshElement[3];
        public float[] deviation = new float[3];
        public RTree.Rectangle bounding;
        public int meshId;
        public Triangle triangle;

        const int A = 0;
        const int B = 1;
        const int C = 2;
        const int AB = A + B - 1;
        const int AC = A + C - 1;
        const int BC = B + C - 1;


        static int id = 0;

        Point3d[] m_point = new Point3d[3];
        Dictionary<int, int> m_equalPoint = new Dictionary<int, int>();


        public MeshElement(V3[] p)
        {
            meshId = id++;

            for (int i = 0; i < 3; i++)
            {
                m_point[i] = new Point3d();
                m_point[i].X = p[i].x;
                m_point[i].Y = p[i].y;
                m_point[i].Z = p[i].z;
                deviation[i] = float.MaxValue;
            }

            triangle = new Triangle(m_point[AB], m_point[BC], m_point[AC]);
            Box3d box = triangle.BoundingBox();
            bounding = new RTree.Rectangle(
                (float)box.P1.X,
                (float)box.P1.Y,
                (float)box.P1.Z,
                (float)box.P7.X,
                (float)box.P7.Y,
                (float)box.P7.Z);
        }

        public void SetNeighbor(MeshElement meshTriangle)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    float d1 = (float)m_point[i].DistanceTo(meshTriangle.m_point[(j + 1) % 3]);
                    float d2 = (float)m_point[(i + 1) % 3].DistanceTo(meshTriangle.m_point[j]);
                    if (d1 < 0.1f && d2 < 0.1f)
                    {
                        int edge = i + (i + 1) % 3 - 1;
                        float dev = MathF.Sqrt(d1 * d1 + d2 * d2);
                        if (deviation[edge] > dev)
                        {
                            neighbor[edge] = meshTriangle;
                            deviation[edge] = dev;
                        }
                    }
                }
            }
        }
    }
}
