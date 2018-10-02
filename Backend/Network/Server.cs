using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using Backend.Game;
using Common;

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

        public const int DeltaTime = 500;
        public int m_millisecondsElapsed = DeltaTime;
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
                Console.WriteLine("Backend start up and waiting for a connection on port {0}...", port);
                while (true)
                {
                    // Set the event to nonsignaled state.

                    // Start an asynchronous socket to listen for connections.

                    DateTime start = DateTime.Now;

                    // Wait until a connection is made before continuing.
                    InvokeCompletion(m_millisecondsElapsed);
                    DateTime end = DateTime.Now;
                    TimeSpan interval = end - start;
                    m_millisecondsElapsed -= (int)interval.TotalMilliseconds;
                    if (m_millisecondsElapsed <= 0)
                    {
                        World.Instance().Tick();
                        m_millisecondsElapsed = DeltaTime;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public bool InvokeCompletion(int millisecondsTimeout)
        {
            try
            {
                CompleteEvent ce;
                if (millisecondsTimeout == 0 && !CompleteQueue.TryTake(out ce))
                {
                    return false;
                }
                else
                {
                    if (!CompleteQueue.TryTake(out ce, millisecondsTimeout))
                    {
                        return false;
                    }
                }

                if (ce.message == null)
                {
                    ((ChannelDelegate)ce.@delegate).Invoke(ce.channel);
                }
                else
                {
                    ((MessageDelegate)ce.@delegate).Invoke(ce.channel, ce.message);
                }
                return true;
            }
            catch (KeyNotFoundException e)
            {
                Trace.WriteLine(string.Format("catch KeyNotFoundException {0} when calling invoke", e.ToString()));
                return false;
            }
            catch (SystemException e)
            {// Catch what exception ????

                Debug.WriteLine(string.Format("catch SystemException {0} when calling invoke", e.ToString()));
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
