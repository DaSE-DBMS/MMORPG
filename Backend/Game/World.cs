using System.Collections.Generic;

namespace Backend.Game
{
    class World : MyObject
    {
        private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        private static World instance = new World();

        private Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();

        private World() { }

        public static World Instance
        {
            get
            {
                return instance;
            }
        }

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

        public void Create()
        {
            Scene scene = new Scene();
            scene.name = "Level1";

            Scene scene1 = new Scene();
            scene1.name = "Level2";



            scenes.Add(scene.name, scene);
            scenes.Add(scene1.name, scene1);

        }

        public Scene GetScene(string name)
        {
            return scenes[name];
        }
    }
}
