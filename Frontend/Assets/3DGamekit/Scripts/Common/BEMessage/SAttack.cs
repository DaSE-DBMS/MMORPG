using System;

namespace Common
{
    [Serializable]
    public class SAttack : Message
    {
        public SAttack() : base(Command.S_ATTACK) { }
        public int ID;
        public int targetID;
    }
}
