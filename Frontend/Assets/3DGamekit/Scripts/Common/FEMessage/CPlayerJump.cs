using System;

namespace Common
{
    [Serializable]
    public class CPlayerJump : Message
    {
        public CPlayerJump() : base(Command.C_PLAYER_JUMP) { }
        public int player;
    }
}
