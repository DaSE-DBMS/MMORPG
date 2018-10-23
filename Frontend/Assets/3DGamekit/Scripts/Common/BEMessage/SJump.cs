using System;

namespace Common
{
    [Serializable]
    public class SJump : Message
    {
        public SJump() : base(Command.S_JUMP) { }
        public int ID;
    }
}
