using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MSChat.NotificationWorker.Configuration;

namespace MSChat.NotificationWorker.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async ValueTask SendUnreadMessageNotificationAsync(UnreadMessageInfo messageInfo, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();

        message.To.Add(new MailboxAddress(messageInfo.UserName, messageInfo.UserEmail));
        message.From.Add(new MailboxAddress(_emailSettings.FromName ?? _emailSettings.FromEmail, _emailSettings.FromEmail));
        message.Subject = $"Unread message in chat \"{messageInfo.ChatName}\"";
        message.Body = new TextPart()
        {
            Text = $"You have unread message in the \"{messageInfo.ChatName}\" chat."
        }; ;

        await SendEmailAsync(message, cancellationToken);
    }

    private async ValueTask SendEmailAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, true, cancellationToken);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
