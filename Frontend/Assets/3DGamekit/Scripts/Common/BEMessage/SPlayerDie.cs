using System;

namespace Common
{
    [Serializable]
    public class SPlayerDie : Message
    {
        public SPlayerDie() : base(Command.S_PLAYER_DIE)
        { }
        public int entityId;
        public bool isMine;
    }
}
