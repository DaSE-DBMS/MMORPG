using System;
using System.Collections.Generic;

namespace Common
{

    [Serializable]
    public class SPlayerTake : Message
    {
        public SPlayerTake() : base(Command.S_PLAYER_TAKE) { }
        public bool clone;
        public string name;
        public int itemID;
    }
}
