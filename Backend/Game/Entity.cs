
using System.Collections.Generic;
using Common.Data;
using GeometRi;
using UnityEngine;

namespace Backend.Game
{
    public class Entity : MyObject
    {
        private static int sequence = 1;

        public EntityType entityType;
        public int entityID;
        public V3 pos;
        public V4 rot;
        public bool forClone;
        public string name;
        public bool update = false;

        private int parentID = 0;
        private Point3d m_pos = new Point3d();
        private Dictionary<int, Entity> m_children = new Dictionary<int, Entity>();

        public Dictionary<int, Entity> Children { get { return m_children; } }
        public Entity Parent { get { return World.Instance().GetEntity(parentID); } }

        public Entity()
        {
            entityID = sequence++;
            World.Instance().AddEntity(entityID, this);
        }

        ~Entity()
        {
            World.Instance().RemoveEntity(entityID);
        }

        virtual public void AddEntity(Entity entity)
        {
            entity.parentID = this.entityID;
            Children.Add(entity.entityID, entity);
        }

        virtual public bool RemoveEntity(int id, out Entity entity)
        {
            bool ret = Children.Remove(id, out entity);
            if (ret)
            {
                entity.parentID = 0;
            }
            return ret;
        }

        virtual public bool FindEntity(int id, out Entity entity)
        {
            return Children.TryGetValue(id, out entity);
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
            entity.entityID = entityID;
            entity.name = name;
            entity.pos = pos;
            entity.rot = rot;
            entity.forClone = forClone;
            entity.type = (int)entityType;
            return entity;
        }

        virtual public void FromDEntity(DEntity entity)
        {
            name = entity.name;
            //id = entity.id; not assign here
            pos = entity.pos;
            rot = entity.rot;
            forClone = entity.forClone;
            entityType = (EntityType)entity.type;
        }

        virtual public void Broundcast(Message message, bool exclude = false)
        {
            World.Instance().Broundcast(message, GetScene(), this, 100, exclude ? entityID : 0);
        }

        public Scene GetScene()
        {
            Entity parent = Parent;
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
