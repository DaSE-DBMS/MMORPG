using Common;
using Backend.Game;
using System;
using System.IO;

namespace Backend.Network
{
    public partial class Incoming
    {
        private void OnRecvRegister(IChannel channel, Message message)
        {
            // TODO ...
            // write to database
            CRegister request = message as CRegister;
            ClientTipInfo(channel, "TODO: write register info to database");
        }
    }
}
