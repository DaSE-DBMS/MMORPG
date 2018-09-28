using System.Collections.Generic;
using Common.Data;

namespace Backend.Game
{
    class World : Singleton<World>
    {
        private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        private Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();

        public Dictionary<string, DEntity> InitialData = new Dictionary<string, DEntity>();

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
                Entity parent = entity.GetParent();
                if (parent != null)
                {
                    parent.RemoveEntity(entity.id, out entity);
                }
            }
            return ret;
        }

        public void LoadScene(DSceneAsset asset)
        {
            Scene scene = new Scene();
            scene.Load(asset);
            scenes[scene.name] = scene;
        }

        public Scene GetScene(string name)
        {
            return scenes[name];
        }


        public void Broundcast(Message message)
        {
            foreach (KeyValuePair<string, Scene> kv in scenes)
            {
                foreach (KeyValuePair<int, Player> p in kv.Value.Players)
                {
                    p.Value.connection.Send(message);
                }
            }
        }

        public void Broundcast(Message message, Scene scene, int exclude)
        {
            foreach (KeyValuePair<int, Player> p in scene.Players)
            {
                if (p.Key != exclude)
                {
                    p.Value.connection.Send(message);
                }
            }
        }

        public void Broundcast(Message message, Scene scene, Entity centre, float radius, int exclude)
        {
            foreach (KeyValuePair<int, Player> p in scene.Players)
            {
                if (p.Value.Distance(centre) < radius && p.Key != exclude)
                {
                    p.Value.connection.Send(message);
                }
            }
        }
    }
}
