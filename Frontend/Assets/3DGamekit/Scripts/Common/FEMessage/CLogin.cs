using System;

namespace Common
{
    [Serializable]
    public class CLogin : Message
    {
        public CLogin() : base(Command.C_LOGIN) { }
        public string user;
        public string password;
    }
}
