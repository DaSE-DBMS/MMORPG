
using System;

namespace Common
{
    public interface IChannel
    {
        void SetContent(object content);

        object GetContent();

        void Send(Message msg);

        void OnClose();
    }
}
