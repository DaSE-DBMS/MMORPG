using System;

namespace Common
{
    [Serializable]
    public class SEntityDestroy : Message
    {
        public SEntityDestroy() : base(Command.S_ENTITY_DESTROY) { }
        public int entityID;
    }
}
