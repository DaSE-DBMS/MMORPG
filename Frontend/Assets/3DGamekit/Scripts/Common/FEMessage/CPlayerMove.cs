using System;

namespace Common
{
    public enum MoveStep
    {
        JUMP,
        ATTACK,
        MOVE_BEGIN,
        MOVE_STEP,
        MOVE_END,
    }

    [Serializable]
    public class CPlayerMove : Message
    {
        public CPlayerMove() : base(Command.C_PLAYER_MOVE) { }
        // target is valid when action is ATTACK/MOVE
        // x, y, z is valid when action is MOVE
        public int player;
        public MoveState state;
        public V3 move;
        public V3 pos;
        public V4 rot;
    }
}
