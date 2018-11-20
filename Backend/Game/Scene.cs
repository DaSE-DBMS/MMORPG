using System.Collections.Generic;
using Common;
using Backend.AI;
using GeometRi;

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
                if (e.forClone)
                {
                    World.Instance.EntityData.TryAdd(e.name, e);
                }
                Entity entity = World.CreateEntity(e);
                if (entity != null)
                {
                    AddEntity(entity);
                }
            }
            foreach (KeyValuePair<int, Sprite> kv in sprites)
            {
                kv.Value.Spawn();
            }
            foreach (KeyValuePair<int, Item> kv in items)
            {
                kv.Value.Spawn();
            }
        }

        public bool FindPath(Point3d start, Point3d end, LinkedList<Point3d> steps)
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
            if (entity is Player)
            {
                PlayerEnter((Player)entity);
            }
            else if (entity is Sprite)
            {
                sprites.Add(entity.entityId, (Sprite)entity);
            }
            else if (entity is Item)
            {
                items.Add(entity.entityId, (Item)entity);
            }
            base.AddEntity(entity);
        }

        override public bool RemoveEntity(int id, out Entity entity)
        {
            if (!base.FindChildEntity(id, out entity))
            {
                return false;
            }
            if (entity is Player)
            {
                PlayerLeave((Player)entity);
            }
            else if (entity is Sprite)
            {
                sprites.Remove(id);
            }
            else if (entity is Item)
            {
                items.Remove(id);
            }
            if (!base.RemoveEntity(id, out entity))
            {
                return false;
            }
            return true;

        }

        public override void Update()
        {
            foreach (KeyValuePair<int, Entity> kv in Children)
            {
                if (kv.Value.UpdateActive)
                {
                    kv.Value.Update();
                }
            }
            base.Update();
        }

        void PlayerEnter(Player player)
        {
            players.Add(player.entityId, player);
            player.SendSpawn(player.ToDEntity());
            if (player.IsEquipedWeapon())
            {
                player.SendEquipWeapon(player);
            }
            foreach (KeyValuePair<int, Item> kv in items)
            {
                player.SendSpawn(kv.Value.ToDEntity());
            }
            foreach (KeyValuePair<int, Sprite> kv in sprites)
            {
                player.SendSpawn(kv.Value.ToDEntity());
            }
            foreach (KeyValuePair<int, Player> kv in players)
            {
                if (player.entityId != kv.Value.entityId)
                {
                    // tell other players my spawning
                    kv.Value.SendSpawn(player.ToDEntity());
                    // tell me other players' spawning
                    if (player.IsEquipedWeapon())
                    {
                        kv.Value.SendEquipWeapon(player);
                    }

                    player.SendSpawn(kv.Value.ToDEntity());
                    if (kv.Value.IsEquipedWeapon())
                    {
                        player.SendEquipWeapon(kv.Value);
                    }
                }

            }
            player.OnEnterScene(this);
        }

        void PlayerLeave(Player player)
        {
            SEntityDestroy msg = new SEntityDestroy();
            msg.entityID = player.entityId;
            player.Broadcast(msg, true);
            players.Remove(player.entityId);
            player.OnLeaveScene(this);
        }
    }
}
