using System;

namespace Common
{
    [Serializable]
    public class SDie : Message
    {
        public SDie() : base(Command.S_DIE)
        { }
        public int entityId;
    }
}
