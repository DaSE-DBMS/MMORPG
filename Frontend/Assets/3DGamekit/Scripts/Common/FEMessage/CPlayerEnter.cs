using System;

namespace Common
{
    // when a player finishing enter a scene
    [Serializable]
    public class CPlayerEnter : Message
    {
        public CPlayerEnter() : base(Command.C_PLAYER_ENTER) { }
    }
}
