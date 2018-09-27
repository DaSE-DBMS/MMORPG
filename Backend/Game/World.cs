using System.Collections.Generic;
using Common.Data;

namespace Backend.Game
{
    class World : Singleton<World>
    {
        private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        private Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();


        public Entity GetEntity(int id)
        {
            Entity entity = null;
            if (entities.TryGetValue(id, out entity))
            {
                return entity;
            }
            return null;
        }

        public void AddEntity(int id, Entity entity)
        {
            entities.Add(id, entity);
        }

        public bool RemoveEntity(int id)
        {
            Entity entity;
            bool ret = entities.Remove(id, out entity);
            if (ret)
            {
                if (entity.Parent() != 0)
                {
                    Entity parent;
                    bool hasParent = entities.TryGetValue(entity.Parent(), out parent);
                    if (hasParent)
                    {
                        parent.RemoveEntity(entity.id, out entity);
                    }
                }
            }
            return ret;
        }

        public void LoadScene(DSceneAsset asset)
        {
            Scene scene = new Scene();
            scene.Load(asset);
        }

        public Scene GetScene(string name)
        {
            return scenes[name];
        }
    }
}
