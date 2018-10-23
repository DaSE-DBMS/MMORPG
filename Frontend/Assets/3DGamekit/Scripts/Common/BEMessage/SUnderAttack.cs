using System;

namespace Common
{
    [Serializable]
    public partial class SUnderAttack : Message
    {
        public SUnderAttack() : base(Command.S_UNDER_ATTACK) { }
        public int ID;
        public int sourceID;
        public int HP;
    }
}
