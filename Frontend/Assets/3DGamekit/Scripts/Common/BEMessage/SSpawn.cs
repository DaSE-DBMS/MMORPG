using System;
using System.Collections.Generic;

namespace Common
{
    [Serializable]
    public class SSpawn : Message
    {
        public SSpawn() : base(Command.S_SPAWN) { }
        public bool isMine;
        public DEntity entity;
    }
}
