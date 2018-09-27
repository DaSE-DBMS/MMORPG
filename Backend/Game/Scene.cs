using System.Collections.Generic;
using Common.Data;
using Backend.AI;

namespace Backend.Game
{
    public class Scene : Entity
    {
        private Dictionary<int, Player> players = new Dictionary<int, Player>();
        private Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();
        private PathFinding path = new PathFinding();

        public void Load(DSceneAsset asset)
        {
            path.LoadNavMesh(asset.mesh);
            foreach (DEntity e in asset.entities.list)
            {
                Entity entity = CreateEntity(e);
                if (entity != null)
                {
                    AddEntity(e.id, entity);
                }
            }
        }

        public bool FindPath(V3 start, V3 end, out List<V3> steps)
        {
            return path.FindPath(start, end, out steps);
        }

        public Dictionary<int, Player> Players
        {
            get { return players; }
        }

        public Dictionary<int, Sprite> Sprites
        {
            get { return sprites; }
        }

        override public void AddEntity(int id, Entity entity)
        {
            if (entity.GetType() == typeof(Player))
            {
                PlayerEnter((Player)entity);
            }
            if (entity.GetType() == typeof(Sprite))
            {
                sprites.Add(id, (Sprite)entity);
            }
            base.AddEntity(id, entity);
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
        void PlayerEnter(Player player)
        {
            players.Add(player.id, player);
            player.SendSpawn(player);
            foreach (KeyValuePair<int, Sprite> p in Sprites)
            {
                player.SendSpawn(p.Value);
            }
            foreach (KeyValuePair<int, Player> p in Players)
            {
                if (player.id != p.Value.id)
                {
                    p.Value.SendSpawn(player);
                    player.SendSpawn(p.Value);
                }

            }
            player.OnEnterScene(this);
        }

        void PlayerLeave(Player player)
        {
            players.Remove(player.id);
            SEntityDestory msg = new SEntityDestory();
            msg.id = player.id;
            Broundcast(msg, player.id);
            player.OnLeaveScene(this);
        }

        public void Broundcast(Message message, int exclude)
        {
            foreach (KeyValuePair<int, Player> p in players)
            {
                if (p.Key != exclude)
                {
                    p.Value.connection.Send(message);
                }
            }
        }

        public void Broundcast(Message message, int x, int y, int z)
        {
            foreach (KeyValuePair<int, Player> p in players)
            {
                p.Value.connection.Send(message);
            }
        }

        static Entity CreateEntity(DEntity de)
        {
            Entity entity = null;
            switch ((EntityType)de.type)
            {
                case EntityType.PLAYER:
                    break;
                case EntityType.SPRITE:
                    Sprite sprite = new Sprite();
                    sprite.pos = de.pos;
                    sprite.rot = de.rot;
                    sprite.hitPoints = de.HP;
                    sprite.maxHitPoints = de.maxHP;
                    sprite.level = de.level;
                    sprite.speed = de.speed;
                    sprite.aggressive = de.aggressive;
                    sprite.name = de.name;
                    entity = sprite;
                    break;
                case EntityType.ITEM:
                    Item item = new Item();
                    item.pos = de.pos;
                    item.rot = de.rot;
                    item.name = de.name;
                    entity = item;
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
                        entity.AddEntity(childEntity.id, childEntity);
                    }
                }
            }
            return entity;
        }
    }
}
