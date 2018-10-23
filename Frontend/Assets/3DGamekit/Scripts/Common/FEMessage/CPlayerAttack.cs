using System;

namespace Common
{
    [Serializable]
    public class CPlayerAttack : Message
    {
        public CPlayerAttack() : base(Command.C_PLAYER_ATTACK) { }
        public int player;
        public int target;
    }
}
