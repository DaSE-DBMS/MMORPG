
using System.Collections.Generic;
using Common.Data;

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
        public string name;

        public Entity()
        {
            id = sequence++;
            World.Instance().AddEntity(id, this);
        }

        public int Parent()
        {
            return parent;
        }
        ~Entity()
        {
            World.Instance().RemoveEntity(id);
        }

        virtual public void AddEntity(int id, Entity entity)
        {
            entity.parent = this.id;
            children.Add(id, entity);
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

        virtual public void Spawn()
        {

        }

        virtual public void Vanish()
        {

        }
    }
}
