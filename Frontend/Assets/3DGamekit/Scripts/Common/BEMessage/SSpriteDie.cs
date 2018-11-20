using System;

namespace Common
{
    [Serializable]
    public class SSpriteDie : Message
    {
        public SSpriteDie() : base(Command.S_SPRITE_DIE)
        { }
        public int entityId;
    }
}
