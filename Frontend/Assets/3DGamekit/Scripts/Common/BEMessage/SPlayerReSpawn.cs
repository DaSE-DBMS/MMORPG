using System;
using System.Collections.Generic;

namespace Common
{
    [Serializable]
    public class SPlayerReSpawn : Message
    {
        public SPlayerReSpawn() : base(Command.S_PLAYER_RESPAWN) { }
        public int entityId;
        public int HP;
        public V3 position;
        public V4 rotation;
    }
}
