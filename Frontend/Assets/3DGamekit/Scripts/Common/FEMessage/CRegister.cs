using System;

namespace Common
{
    [Serializable]
    public class CRegister : Message
    {
        public CRegister() : base(Command.C_REGISTER) { }
        public string user;
        public string password;
    }
}
