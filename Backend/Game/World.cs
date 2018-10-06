using System.Collections.Generic;
using System.Diagnostics;
using Common;
using Common.Data;

namespace Backend.Game
{
    class World : Singleton<World>
    {
        public const float DeltaTime = 0.5f;

        private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        private Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();

        private Dictionary<string, DEntity> data = new Dictionary<string, DEntity>();

        public Dictionary<string, DEntity> EntityData { get { return data; } }

        public void Tick()
        {
            foreach (KeyValuePair<string, Scene> kv in scenes)
            {
                kv.Value.Update();
            }
        }
        public Entity GetEntity(int id)
        {
            return entities[id];

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
                Entity parent = entity.Parent;
                if (parent != null)
                {
                    parent.RemoveEntity(entity.entityID, out entity);
                }
            }
            else
            {
                Trace.WriteLine(string.Format("cannot find entity {0}", entity.entityID));
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

        static public Entity CreateEntityByName(string name)
        {
            DEntity dentity;
            if (!World.Instance().EntityData.TryGetValue(name, out dentity))
            {
                return null;
            }
            return CreateEntity(dentity);
        }


        public static Entity CreateEntity(DEntity de)
        {
            Entity entity = null;
            switch ((EntityType)de.type)
            {
                case EntityType.PLAYER:
                    break;
                case EntityType.SPRITE:
                    entity = new Sprite();
                    break;
                case EntityType.ITEM:
                    entity = new Item();
                    break;
                case EntityType.WEAPON:
                    entity = new Weapon();
                    break;
                default:
                    break;
            }
            if (entity != null)
            {
                entity.FromDEntity(de);
                foreach (DEntity e in de.children)
                {
                    Entity childEntity = CreateEntity(e);
                    if (childEntity != null)
                    {
                        entity.AddEntity(childEntity);
                    }
                }
            }
            return entity;
        }
    }
}
