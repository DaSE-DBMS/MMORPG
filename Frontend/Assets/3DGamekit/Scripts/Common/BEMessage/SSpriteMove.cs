using System;

namespace Common
{
    [Serializable]
    public class SSpriteMove : Message
    {
        public SSpriteMove() : base(Command.S_SPRITE_MOVE) { }
        public int ID;
        public MoveState state;
        public int targetId;
        public V3 pos;
    }
}
