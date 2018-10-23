using System;
using UnityEngine;
using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvExit(IChannel channel, Message message)
        {
            MyNetwork.Close();
        }
    }
}
