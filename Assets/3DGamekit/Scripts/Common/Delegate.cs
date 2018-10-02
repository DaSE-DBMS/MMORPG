using System;

namespace Common
{
    public class CompleteEvent
    {
        public Object @delegate;
        public IChannel channel;
        public Message message;
    }

    public delegate void MessageDelegate(IChannel channel, Message message);
    public delegate void ChannelDelegate(IChannel channel);
}
