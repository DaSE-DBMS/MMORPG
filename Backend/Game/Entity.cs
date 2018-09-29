
using System.Collections.Generic;
using Common.Data;
using GeometRi;
using UnityEngine;

namespace Backend.Game
{
    public class Entity : MyObject
    {
        private static int sequence = 1;
        private int parent = 0;
        public Dictionary<int, Entity> children = new Dictionary<int, Entity>();
        public int id;
        public V3 pos;
        public V4 rot;
        public Vector3d vector3d = new Vector3d();
        public bool canClone;
        public string name;
        public bool update = false;

        private Point3d m_pos = new Point3d();

        public Entity()
        {
            id = sequence++;
            World.Instance().AddEntity(id, this);
        }

        public Entity GetParent()
        {
            return World.Instance().GetEntity(parent);
        }

        ~Entity()
        {
            World.Instance().RemoveEntity(id);
        }

        virtual public void AddEntity(Entity entity)
        {
            entity.parent = this.id;
            children.Add(entity.id, entity);
        }

        virtual public bool RemoveEntity(int id, out Entity entity)
        {
            bool ret = children.Remove(id, out entity);
            if (ret)
            {
                entity.parent = 0;
            }
            return ret;
        }

        virtual public bool FindEntity(int id, out Entity entity)
        {
            return children.TryGetValue(id, out entity);
        }

        virtual public void Update()
        {

        }

        virtual public void Spawn()
        {

        }

        virtual public void Vanish()
        {

        }

        virtual public DEntity ToDEntity()
        {
            DEntity entity = new DEntity();
            entity.id = id;
            entity.name = name;
            entity.pos = pos;
            entity.rot = rot;
            entity.canClone = canClone;
            return entity;
        }

        virtual public void FromDEntity(DEntity entity)
        {
            name = entity.name;
            //id = entity.id; not assign here
            pos = entity.pos;
            rot = entity.rot;
            canClone = entity.canClone;
        }

        virtual public void Broundcast(Message message, bool exclude = false)
        {
            World.Instance().Broundcast(message, GetScene(), this, 100, exclude ? id : 0);
        }

        public Scene GetScene()
        {
            Entity parent = GetParent();
            if (parent != null)
            {
                if (parent.GetType() == typeof(Scene))
                {
                    return (Scene)parent;
                }
                else
                {
                    return parent.GetScene();
                }
            }
            else
            {
                return null;
            }

        }

        public float Distance(Entity entity)
        {
            V3ToPoint3D();
            entity.V3ToPoint3D();
            return (float)m_pos.DistanceTo(entity.m_pos);
        }

        void V3ToPoint3D()
        {
            m_pos.X = pos.x;
            m_pos.Y = pos.y;
            m_pos.Z = pos.z;
        }
    }
}
