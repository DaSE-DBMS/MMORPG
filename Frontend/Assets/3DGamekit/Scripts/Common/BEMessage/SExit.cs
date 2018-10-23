using System;

namespace Common
{
    [Serializable]
    public class SExit : Message
    {
        public SExit() : base(Command.S_EXIT) { }
    }
}
