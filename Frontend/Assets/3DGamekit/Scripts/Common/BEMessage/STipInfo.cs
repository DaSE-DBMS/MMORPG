using System;
using System.Collections.Generic;

namespace Common
{

    [Serializable]
    public class STipInfo : Message
    {
        public STipInfo() : base(Command.S_TIP_INFO) { }
        public string info;
    }
}
