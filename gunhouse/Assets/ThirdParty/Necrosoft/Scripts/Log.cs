using System;

public class Log
{
    public enum MessageType { Info, Warning, Error }
    public struct Message { public string text; public MessageType type; }
    public static event Action<Message> messages;

    public static void Info(string text)
    {
        if (messages == null) return;
        Message message = new Message() { text = text, type = MessageType.Info };
        messages(message);
    }

    public static void Warning(string text)
    {
        if (messages == null) return;
        Message message = new Message() { text = text, type = MessageType.Warning };
        messages(message);
    }

    public static void Error(string text)
    {
        if (messages == null) return;
        Message message = new Message() { text = text, type = MessageType.Error };
        messages(message);
    }
}