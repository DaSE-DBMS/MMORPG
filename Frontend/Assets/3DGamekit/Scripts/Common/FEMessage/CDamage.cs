using System;

namespace Common
{
    [Serializable]
    public class CDamage : Message
    {
        public CDamage() : base(Command.C_DAMAGE) { }
        public int entityId;
        public int decHP;
    }
}
