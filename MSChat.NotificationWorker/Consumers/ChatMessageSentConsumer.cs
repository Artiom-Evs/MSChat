using MassTransit;
using MSChat.Shared.Events;

namespace MSChat.NotificationWorker.Consumers;

public class ChatMessageSentConsumer : IConsumer<ChatMessageSent>
{
    public Task Consume(ConsumeContext<ChatMessageSent> context)
    {
        Console.WriteLine(context.Message);

        return Task.CompletedTask;
    }
}
