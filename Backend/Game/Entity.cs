
using System.Collections.Generic;

namespace Backend.Game
{
    public class Entity : MyObject
    {
        private static int sequence = 1;
        private int parent = 0;
        public Dictionary<int, Entity> children = new Dictionary<int, Entity>();
        public int id;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationX = 0;
        public float rotationY = 0;
        public float rotationZ = 0;
        public float rotationW = 0;
        public string name;

        public Entity()
        {
            id = sequence++;
            World.Instance.AddEntity(id, this);
        }

        public int Parent()
        {
            return parent;
        }
        ~Entity()
        {
            World.Instance.RemoveEntity(id);
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
