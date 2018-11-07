using System;

namespace Common
{
    [Serializable]
    public partial class SHit : Message
    {
        public SHit() : base(Command.S_HIT) { }
        public int targetId;
        public int sourceId;
        public int decHP;
    }
}
