
using System;
public interface IChannel
{
    void SetContent(Object content);

    Object GetContent();

    void Send(Message msg);

    void OnClose();
}

