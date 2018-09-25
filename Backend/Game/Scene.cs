using System.Collections.Generic;

namespace Backend.Game
{
    public class Scene : Entity
    {
        private Dictionary<int, Player> players = new Dictionary<int, Player>();
        private Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();

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
    }
}
