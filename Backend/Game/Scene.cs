using System.Collections.Generic;
using Common.Data;
using Backend.AI;

namespace Backend.Game
{
    public class Scene : Entity
    {
        private Dictionary<int, Player> players = new Dictionary<int, Player>();
        private Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();
        private Dictionary<int, Item> items = new Dictionary<int, Item>();
        private PathFinding path = new PathFinding();

        public void Load(DSceneAsset asset)
        {
            name = asset.scene;
            path.LoadNavMesh(asset.mesh);
            foreach (DEntity e in asset.entities.list)
            {
                Entity entity = CreateEntity(e);
                if (entity != null)
                {
                    AddEntity(entity);
                }
            }
        }

        public bool FindPath(V3 start, V3 end, Queue<V3> steps)
        {
            return path.FindPath(start, end, steps);
        }

        public Dictionary<int, Player> Players
        {
            get { return players; }
        }

        public Dictionary<int, Sprite> Sprites
        {
            get { return sprites; }
        }
        public Dictionary<int, Item> Items
        {
            get { return items; }
        }

        override public void AddEntity(Entity entity)
        {
            if (entity.GetType() == typeof(Player))
            {
                PlayerEnter((Player)entity);
            }
            if (entity.GetType() == typeof(Sprite))
            {
                sprites.Add(entity.id, (Sprite)entity);
            }
            base.AddEntity(entity);
        }

        override public bool RemoveEntity(int id, out Entity entity)
        {
            bool ret = base.RemoveEntity(id, out entity);
            if (ret && entity.GetType() == typeof(Player))
            {
                PlayerLeave((Player)entity);
            }
            if (ret && entity.GetType() == typeof(Sprite))
            {
                sprites.Remove(id);
            }
            return ret;

        }

        public override void Update()
        {
            foreach (KeyValuePair<int, Entity> kv in children)
            {
                if (kv.Value.update)
                {
                    kv.Value.Update();
                }
            }
            base.Update();
        }

        void PlayerEnter(Player player)
        {
            players.Add(player.id, player);
            player.SendSpawn(player.ToDEntity());
            foreach (KeyValuePair<int, Item> kv in Items)
            {
                player.SendSpawn(kv.Value.ToDEntity());
            }
            foreach (KeyValuePair<int, Sprite> kv in Sprites)
            {
                player.SendSpawn(kv.Value.ToDEntity());
            }
            foreach (KeyValuePair<int, Player> kv in Players)
            {
                if (player.id != kv.Value.id)
                {
                    kv.Value.SendSpawn(kv.Value.ToDEntity());
                    player.SendSpawn(kv.Value.ToDEntity());
                }

            }
            player.OnEnterScene(this);
        }

        void PlayerLeave(Player player)
        {
            SEntityDestory msg = new SEntityDestory();
            msg.id = player.id;
            player.Broundcast(msg);
            players.Remove(player.id);
            player.OnLeaveScene(this);
        }

        Entity CreateEntity(DEntity de)
        {
            Entity entity = null;
            switch ((EntityType)de.type)
            {
                case EntityType.PLAYER:
                    World.Instance().InitialData.Add(de.name, de);
                    break;
                case EntityType.SPRITE:
                    entity = new Sprite();
                    entity.FromDEntity(de);
                    break;
                case EntityType.ITEM:
                    entity = new Item();
                    entity.FromDEntity(de);
                    break;
                default:
                    break;
            }
            if (entity != null)
            {
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
