﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Common.Data;
using GeometRi;
using System.Spatial;

namespace Backend.AI
{
    public class PathFinding : Singleton<PathFinding>
    {
        string m_path;

        RTree.RTree<MeshElement> m_rtree = new RTree.RTree<MeshElement>();
        Dictionary<int, MeshElement> mesh = new Dictionary<int, MeshElement>();

        class Step : IComparable<Step>
        {
            public Step(MeshElement e)
            {
                element = e;
            }

            // the cost from start position to end position by use path crossing current position
            public float costS2E;

            // the cost form start position to current position
            public float costS2C;

            public MeshElement element;

            public Step prev;

            public int CompareTo(Step other)
            {
                float diff = this.costS2E - other.costS2E;
                if (Math.Abs(diff) < float.Epsilon)
                {
                    return this.element.meshId - other.element.meshId;
                }
                else
                {
                    return diff > 0 ? 1 : -1;
                }
            }
        }

        Dictionary<int, Step> m_open = new Dictionary<int, Step>();
        SortedSet<int> m_close = new SortedSet<int>();
        SortedSet<Step> m_openSorted = new SortedSet<Step>();

        void ClearState()
        {
            m_open.Clear();
            m_openSorted.Clear();
            m_close.Clear();
        }

        public void LoadNavMesh()
        {
            //JsonReader reader = new JsonReader();
            XmlSerializer serializer = new XmlSerializer(typeof(Common.Data.NavM));
            StreamReader sm = new StreamReader("E:/Users/ybbh/workspace/MMORPG/Assets/navmesh/navmesh_Level1.xml");

            Common.Data.NavM navM = (Common.Data.NavM)serializer.Deserialize(sm);
            foreach (Common.Data.Tngl tr in navM.mesh)
            {
                MeshElement me = new MeshElement(tr.p);
                AddElement(me);
            }

            foreach (KeyValuePair<int, MeshElement> kv in mesh)
            {
                List<MeshElement> meshList = m_rtree.Intersects(kv.Value.bounding);
                foreach (MeshElement element in meshList)
                {
                    if (element.meshId != kv.Value.meshId)
                    {
                        kv.Value.SetNeighbor(element);
                    }
                }
                int count = 0;
                foreach (MeshElement n in kv.Value.neighbor)
                {
                    if (n != null)
                        count++;
                }
                if (count == 0)
                {
                    m_rtree.Delete(kv.Value.bounding, kv.Value);
                    continue;
                }
            }
        }

        void AddElement(MeshElement element)
        {
            mesh.Add(element.meshId, element);

            m_rtree.Add(element.bounding, element);
        }

        public bool FindPath(Pos p1, Pos p2, out List<Pos> path)
        {
            bool ret = false;
            RTree.Point point = new RTree.Point(p1.x, p1.y, p1.z);
            List<MeshElement> list = m_rtree.Nearest(point, 10.0f);
            path = null;
            MeshElement nearest = null;
            Point3d start = new Point3d();
            start.X = p1.x;
            start.Y = p1.y;
            start.Z = p1.z;
            float minDis = float.MaxValue;
            foreach (MeshElement e in list)
            {
                float dis = (float)e.triangle.Centroid.DistanceTo(start);
                if (dis < minDis)
                {
                    nearest = e;
                    minDis = dis;
                }

            }
            if (nearest == null)
            {
                return false;
            }
            ret = FindPath(nearest, p2, out path);
            ClearState();
            return ret;
        }


        bool FindPath(MeshElement start, Pos pos, out List<Pos> path)
        {
            Point3d end = new Point3d();
            Point3d currentPos = start.triangle.Centroid.Copy();
            end.X = pos.x;
            end.Y = pos.y;
            end.Z = pos.z;
            float distance = CostEstimate(currentPos, end);
            Step step = new Step(start);
            step.costS2C = 0;
            step.costS2E = distance;
            m_open.Add(start.meshId, step);
            m_openSorted.Add(step);
            path = null;
            while (m_openSorted.Count != 0)
            {
                IEnumerator<Step> e = m_openSorted.GetEnumerator();
                if (!e.MoveNext())
                {
                    return false;
                }
                Step currentStep = e.Current;
                int id = currentStep.element.meshId;
                m_openSorted.Remove(currentStep);
                m_open.Remove(id);
                MeshElement element = currentStep.element;
                if (TriangleContainPoint(element.triangle, end))
                {
                    // Success ...
                    ConstructPath(currentStep, out path);
                    return true;
                }
                m_close.Add(id);
                foreach (MeshElement neighbor in element.neighbor)
                {
                    if (neighbor == null)
                    {
                        continue;
                    }
                    if (m_close.Contains(neighbor.meshId))
                    {
                        continue;
                    }

                    // start position to this neighbor piece cost
                    float costS2C = currentStep.costS2C + Distance(neighbor.triangle.Centroid, end);
                    if (m_open.ContainsKey(neighbor.meshId))
                    {
                        Step s = m_open.GetValueOrDefault(neighbor.meshId);

                        if (s != null && costS2C < s.costS2C)
                        {
                            m_openSorted.Remove(s);
                            s.costS2C = costS2C;
                            s.prev = currentStep;
                            m_openSorted.Add(s);
                        }
                    }
                    else
                    {
                        Step nextStep = new Step(neighbor);
                        nextStep.costS2C = costS2C;
                        nextStep.costS2E = costS2C + CostEstimate(neighbor.triangle.Centroid, end);
                        m_open.Add(neighbor.meshId, nextStep);
                        m_openSorted.Add(nextStep);
                        nextStep.prev = currentStep;
                    }
                }
            }
            return false;
        }

        void ConstructPath(Step lastStep, out List<Pos> path)
        {
            Step step = lastStep;
            path = new List<Pos>();
            while (step.prev != null)
            {
                Point3d point = step.element.triangle.Centroid;
                Pos pos = new Pos();
                pos.x = (float)point.X;
                pos.y = (float)point.Y;
                pos.z = (float)point.Z;
                path.Add(pos);
                step = step.prev;
            }
        }

        float CostEstimate(Point3d p1, Point3d p2)
        {
            float distance = (float)p1.DistanceTo(p2);
            return distance;
        }

        float Distance(Point3d p1, Point3d p2)
        {
            float distance = (float)p1.DistanceTo(p2);
            return distance;
        }

        bool TriangleContainPoint(Triangle triangle, Point3d point)
        {

            // P = A + x * (C - A) + y * (B - A)
            // P - A = x * (C - A) + y * (B - A)
            // set v0 = C - A, v1 = B - A  and v2 = P - A
            // then we have v2 = x * v0 + y * v1
            // x = ((v1*v1)(v2*v0)-(v1*v0)(v2*v1)) / ((v0*v0)(v1*v1) -(v0*v1)(v1*v0))
            // y = ((v0*v0)(v2*v1)-(v0*v1)(v2*v0)) / ((v0*v0)(v1*v1) -(v0•v1)(v1*v0))

            Point3d v0 = triangle.C.Subtract(triangle.A);
            Point3d v1 = triangle.B.Subtract(triangle.A);
            Point3d v2 = point.Subtract(triangle.A);

            float d00 = (float)(v0.X * v0.X + v0.Y * v0.Y + v0.Z * v0.Z);
            float d01 = (float)(v0.X * v1.X + v0.Y * v1.Y + v0.Z * v1.Z);
            float d02 = (float)(v0.X * v2.X + v0.Y * v2.Y + v0.Z * v2.Z);
            float d11 = (float)(v1.X * v1.X + v1.Y * v1.Y + v1.Z * v1.Z);
            float d12 = (float)(v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z);

            float x = (d11 * d02 - d01 * d12) / (d00 * d11 - d01 * d01);
            if (x < 0 || x > 1)
            {
                return false;
            }

            float y = (d00 * d12 - d01 * d02) / (d00 * d11 - d01 * d01);
            if (y < 0 || y > 1)
            {
                return false;
            }

            return x + y <= 1;
        }
    }
}
