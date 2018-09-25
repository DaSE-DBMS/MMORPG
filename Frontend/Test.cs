using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Backend.Network;
using Frontend.Network;
// State object for receiving data from remote device.
public class Test
{

    bool stop = false;

    static public int Main(String[] args)
    {
        Test test = new Test();
        test.Start();
        return 0;
    }

    void Start()
    {
        GameServer gs = new GameServer("0.0.0.0", 7777);
        Thread serverThd = new Thread(gs.StartUp);
        serverThd.Start();
        Thread.Sleep(1000);
        Client gameClient = new Client();

        // register callback
        gameClient.Register(Command.S_PLAYER_ENTER, OnEnter);

        // connect to game server
        gameClient.Connect("127.0.0.1", 7777);

        // sent first message
        CLogin cLogin = new CLogin();
        cLogin.user = "ybbh";
        cLogin.password = "123456";
        gameClient.Send(cLogin);
        while (!stop)
        {
            Thread.Sleep(1000);
            gameClient.RecvMessage();
        }
    }
    private void OnEnter(IChannel channel, Message message)
    {
        //Console.WriteLine("Receive Enter...");
        SPlayerEnter enter = (SPlayerEnter)message;
        CLogin cLogin = new CLogin();
        cLogin.user = enter.user;
        cLogin.password = "123456";
        channel.Send(cLogin);
    }
}
