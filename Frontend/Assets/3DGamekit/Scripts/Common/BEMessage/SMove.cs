using System;

namespace Common
{
    public enum MoveState
    {
        BEGIN,
        STEP,
        END
    }

    [Serializable]
    public class SMove : Message
    {
        public SMove() : base(Command.S_MOVE) { }
        public int ID;
        public MoveState state;
        public V3 move;
        public V3 pos;
        public V4 rot;
    }
}
