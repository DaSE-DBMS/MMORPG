using System;
using System.Collections.Generic;

namespace Common
{
    [Serializable]
    public class SFindPath : Message
    {
        public SFindPath() : base(Command.S_FIND_PATH)
        {
            path = new List<V3>();
        }

        public List<V3> path;
    }
}
