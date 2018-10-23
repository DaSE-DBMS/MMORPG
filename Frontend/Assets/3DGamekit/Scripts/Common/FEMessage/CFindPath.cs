using System;

namespace Common
{
    // FOR debug ...
    [Serializable]
    public class CFindPath : Message
    {
        public CFindPath() : base(Command.C_FIND_PATH)
        {
            pos = new V3();
        }

        public V3 pos;
    }

}
