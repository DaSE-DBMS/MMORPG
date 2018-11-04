using System;

namespace Common
{
    [Serializable]
    public class CPositionRevise : Message
    {
        public CPositionRevise() : base(Command.C_POSITION_REVISE) { }
        public int entityId;
        public V3 pos;
    }
}
