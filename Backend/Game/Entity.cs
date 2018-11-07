using System;
using System.Collections.Generic;
using GeometRi;
using Common;

namespace Backend.Game
{

    public class Entity : MyObject
    {
        public delegate void OnTimer();
        private static int sequence = 1;
        public EntityType entityType;
        public int entityId;
        public bool forClone;
        public string name;
        private bool m_update = false;
        private int m_parentID = 0;
        private Point3d m_position = new Point3d();
        private Quaternion m_rotation = new Quaternion();
        private Dictionary<int, Entity> m_children = new Dictionary<int, Entity>();

        private DEntity m_entityData; // for reset ...
        private Queue<KeyValuePair<DateTime, OnTimer>> m_timers = new Queue<KeyValuePair<DateTime, OnTimer>>();


        public Dictionary<int, Entity> Children { get { return m_children; } }
        public Entity Parent { get { return World.Instance().GetEntity(m_parentID); } }
        public DEntity DefaultData { get { return m_entityData; } }
        public Point3d Position
        {
            get
            {
                return new Point3d(m_position.X, m_position.Y, m_position.Z);
            }
            set
            {
                m_position.X = value.X;
                m_position.Y = value.Y;
                m_position.Z = value.Z;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return new Quaternion(m_rotation.X, m_rotation.Y, m_rotation.Z, m_rotation.W);
            }
            set
            {
                m_rotation.X = value.X;
                m_rotation.Y = value.Y;
                m_rotation.Z = value.Z;
                m_rotation.W = value.W;
            }
        }

        public bool UpdateActive
        {
            get { return m_update; }
            set { m_update = value; }
        }

        public Entity()
        {
            entityId = sequence++;
            World.Instance().AddEntity(entityId, this);
        }

        virtual public void AddEntity(Entity entity)
        {
            entity.m_parentID = this.entityId;
            Children.Add(entity.entityId, entity);
        }

        virtual public bool RemoveEntity(int id, out Entity entity)
        {
            bool ret = Children.Remove(id, out entity);
            if (ret)
            {
                entity.m_parentID = 0;
            }
            return ret;
        }

        // find entity in children list
        virtual public Entity GetChildEntity(int id)
        {
            return Children[id];
        }

        // find entity in children list
        virtual public bool FindChildEntity(int id, out Entity entity)
        {
            return Children.TryGetValue(id, out entity);
        }


        virtual public void Broadcast(Message message, bool exclude = false)
        {
            World.Instance().Broadcast(message, GetScene(), this, 100, exclude ? entityId : 0);
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
        public void Tick()
        {
            while (m_timers.Count != 0)
            {
                var kv = m_timers.Peek();
                if (kv.Key <= DateTime.Now)
                {
                    kv.Value.Invoke();
                    m_timers.Dequeue();
                }
            }
        }

        public void DelayInvoke(int seconds, OnTimer onTimer)
        {
            var ts = DateTime.Now.Add(TimeSpan.FromSeconds(seconds));
            var kv = new KeyValuePair<DateTime, OnTimer>(ts, onTimer);
            m_timers.Enqueue(kv);
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

        virtual public void ReSpawn()
        {
        }

        virtual public DEntity ToDEntity()
        {
            DEntity entity = new DEntity();
            entity.entityID = entityId;
            entity.name = name;

            entity.pos = Point3dToV3(m_position);

            entity.rot.x = (float)m_rotation.X;
            entity.rot.y = (float)m_rotation.Y;
            entity.rot.z = (float)m_rotation.Z;
            entity.rot.w = (float)m_rotation.W;

            entity.forClone = forClone;
            entity.type = (int)entityType;

            return entity;
        }

        virtual public void FromDEntity(DEntity entity)
        {
            if (m_entityData == null)
                m_entityData = entity;

            name = entity.name;

            //id = entity.id; not assign here
            m_position.X = entity.pos.x;
            m_position.Y = entity.pos.y;
            m_position.Z = entity.pos.z;

            m_rotation.X = entity.rot.x;
            m_rotation.Y = entity.rot.y;
            m_rotation.Z = entity.rot.z;
            m_rotation.W = entity.rot.w;

            forClone = entity.forClone;
            entityType = (EntityType)entity.type;
        }

        public V3 GetPosition()
        {
            return new V3((float)m_position.X, (float)m_position.Y, (float)m_position.Z);
        }

        public V4 GetRotation()
        {
            return new V4((float)m_rotation.X, (float)m_rotation.Y, (float)m_rotation.Z, (float)m_rotation.W);
        }

        static public Point3d V3ToPoint3d(V3 pos)
        {
            return new Point3d(pos.x, pos.y, pos.z);
        }

        static public V3 Point3dToV3(Point3d point)
        {
            return new V3((float)point.X, (float)point.Y, (float)point.Z);
        }

        public float Distance(Entity entity)
        {
            return (float)m_position.DistanceTo(entity.m_position);
        }

        public void Reset()
        {
            FromDEntity(m_entityData);
            foreach (KeyValuePair<int, Entity> kv in m_children)
            {
                kv.Value.Reset();
            }
        }
    }
}
