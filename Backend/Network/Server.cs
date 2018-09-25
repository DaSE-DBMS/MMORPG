using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Backend.Network
{
    public class Server : IServer
    {
        private Socket listener;
        // Thread signal.
        private ManualResetEvent allDone = new ManualResetEvent(false);

        private Dictionary<Command, MessageDelegate> onMessageRecv = new Dictionary<Command, MessageDelegate>();

        private List<ChannelDelegate> onAccept = new List<ChannelDelegate>();

        private List<ChannelDelegate> onClose = new List<ChannelDelegate>();

        private BlockingCollection<CompleteEvent> completeQueue = new BlockingCollection<CompleteEvent>();

        public BlockingCollection<CompleteEvent> CompleteQueue { get { return completeQueue; } }

        public Server()
        {
        }
        override public void Start(string ip, short port)
        {
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.
            listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);
                //Console.WriteLine("Waiting for a connection...");
                while (true)
                {
                    // Set the event to nonsignaled state.

                    // Start an asynchronous socket to listen for connections.



                    // Wait until a connection is made before continuing.
                    InvokeCompletion();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public bool InvokeCompletion(bool nonblocking = false)
        {
            try
            {
                CompleteEvent e;
                bool success;
                if (nonblocking && !CompleteQueue.TryTake(out e))
                {
                    return false;
                }
                else
                {
                    e = CompleteQueue.Take();
                    success = true;
                }
                if (e.message == null)
                {
                    ((ChannelDelegate)e.@delegate).Invoke(e.channel);
                }
                else
                {
                    ((MessageDelegate)e.@delegate).Invoke(e.channel, e.message);
                }
                return success;
            }
            catch (SystemException e)
            {
                return false;
            }

        }
        override public void RegisterConnect(ChannelDelegate @delegate)
        {
            onAccept.Add(@delegate);
        }

        override public void RegisterClose(ChannelDelegate @delegate)
        {
            onClose.Add(@delegate);
        }

        override public void RegisterMessageRecv(Command cmd, MessageDelegate @delegate)
        {
            onMessageRecv[cmd] = @delegate;
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            // Create the state object.
            Channel channel = new Channel(handler);
            channel.RegisterOnMessageRecv(onMessageRecv);
            channel.RegisterOnClose(onClose);
            channel.CompleteQueue = CompleteQueue;
            //channel.workSocket = handler;
            listener.BeginAccept(
                new AsyncCallback(AcceptCallback),
                listener);
            foreach (ChannelDelegate d in onAccept)
            {
                //d.Invoke(channel);
                CompleteEvent e = new CompleteEvent();
                e.@delegate = d;
                e.channel = channel;
                e.message = null;
                CompleteQueue.Add(e);
            }

            Console.WriteLine("Accept connection from {0}", handler.RemoteEndPoint.ToString());
            // Issue the firest receive command
            channel.BeginRecv();
            // Issue a new accept command
        }
    }
}
