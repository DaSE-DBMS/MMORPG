
using System;

namespace Common
{
    public interface IChannel
    {
        void SetContent(Object content);

        Object GetContent();

        void Send(Message msg);

        void OnClose();
    }
}
