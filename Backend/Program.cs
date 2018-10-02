using System;
using System.Threading;

using Backend.Network;

class Program
{

    static public int Main(String[] args)
    {
        GameServer gs = new GameServer("0.0.0.0", 7777);
        gs.StartUp();
        return 0;
    }
}
