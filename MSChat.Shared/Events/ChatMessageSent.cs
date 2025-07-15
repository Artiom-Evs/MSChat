namespace MSChat.Shared.Events;

public record ChatMessageSent(
    string SenderId, 
    long ChatId, 
    long MessageId, 
    string Text, 
    DateTime CreatedAt);
