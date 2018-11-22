using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using Common;
using UnityEngine;

namespace Gamekit3D.Network
{
    // Client use non-blocking socket, receive synchronously, send asynchronously
    public class Client : Singleton<Client>, IChannel, IRegister
    {
        private Socket socket;

        private Command command = Command.NONE;

        private int readSize = 0;

        private int unreadSize = 4;

        private const int BufferSize = 1024;

        private Byte[] buffer = new Byte[BufferSize];
        private int offset = 0;

        private MemoryStream stream = new MemoryStream();

        private Dictionary<Command, MessageDelegate> messageDelegate = new Dictionary<Command, MessageDelegate>();

        private ChannelDelegate onClose;

        private BinaryFormatter formatter = new BinaryFormatter();

        private object player;

        public void SetContent(object content)
        {
            player = content;
        }

        public object GetContent()
        {
            return player;
        }

        public void Send(Message message)
        {
            MemoryStream stream = new MemoryStream();

            stream.Seek(Message.MsgHeaderSize, SeekOrigin.Begin);
            formatter.Serialize(stream, message);
            UInt16 length = (UInt16)stream.Length;
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes((UInt16)message.command), 0, 2);
            stream.Write(BitConverter.GetBytes(length), 0, 2);
            stream.Seek(0, SeekOrigin.Begin);
            // begin sending the data to the remote device.
            socket.BeginSend(stream.GetBuffer(), 0, (int)stream.Length, 0,
                new AsyncCallback(SendCallback), socket);
        }

        public void Close()
        {
            if (onClose != null)
            {
                try
                {
                    onClose.Invoke(this);
                }
                catch (SystemException)
                {

                }
            }
            socket.Shutdown(SocketShutdown.Both);
        }
        public void RecvMessage()
        {
            while (true)
            {   //loop till read nothing
                int size = 0;
                if (socket == null)
                {
                    return;
                }
                if (readSize < 4)
                {
                    // read message header ...
                    if (socket.Available < unreadSize)
                    {
                        return;
                    }
                    size = socket.Receive(buffer, offset, unreadSize, SocketFlags.None);
                    if (size <= 0)
                    {
                        return;
                    }
                    readSize += size;
                    offset += size;
                    unreadSize -= size;
                    if (unreadSize != 0)
                    {
                        return;
                    }

                    command = (Command)BitConverter.ToUInt16(buffer, 0);
                    unreadSize = (int)BitConverter.ToUInt16(buffer, 2) - 4;
                    offset = 0;
                }

                // read message body ...
                if (socket.Available < unreadSize)
                {
                    return;
                }
                size = socket.Receive(buffer, offset, Math.Min(BufferSize - offset, unreadSize), SocketFlags.None);
                if (size <= 0)
                {
                    return;
                }
                readSize += size;
                offset += size;
                unreadSize -= size;
                stream.Write(buffer, 0, size);
                offset = 0;

                if (unreadSize == 0)
                {// finish read a message, deserialize message to a class, invoke message receive callback
                    stream.Seek(0, SeekOrigin.Begin);
                    Message msg = (Message)formatter.Deserialize(stream);
                    try
                    {
                        MessageDelegate @delegate = messageDelegate[command];
                        @delegate.Invoke(this, msg);
                    }
                    catch (KeyNotFoundException e)
                    {
                        Trace.WriteLine(string.Format("catch KeyNotFoundException {0} when calling invoke", e.ToString()));
                    }
                    catch (SystemException)
                    {

                    }
                    // clear the receive strean and reset receive state
                    stream.Seek(0, SeekOrigin.End);
                    stream.Position = 0;
                    offset = 0;
                    command = Command.NONE;
                    readSize = 0;
                    unreadSize = 4;
                }
            }
        }

        public void Register(Command cmd, MessageDelegate @delegate)
        {
            messageDelegate[cmd] = @delegate;
        }

        public void RegisterClose(ChannelDelegate @delegate)
        {
            onClose = @delegate;
        }

        public bool Connect(string ip, short port)
        {
            if (socket != null && socket.Connected)
            {
                return true;
            }

            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            try
            {
                // Create a TCP/IP socket.
                socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                socket.Connect(remoteEP);
                socket.Blocking = false;
                return true;
            }
            catch (SocketException ex)
            {
                UnityEngine.Debug.Log(ex.Message);
                return false;
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                if (bytesSent <= 0)
                {
                    // TODO.... error
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
