using Fab.Infrastructure.Interfaces.Email;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Fab.Infrastructure.Implementation.Email;

public class EmailService : IEmailService
{
    private readonly EmailOptions _options;

    public EmailService(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(string email, string subject, string content,
                                CancellationToken cancellationToken = default)
        => await SendAsync(new[] { email }, subject, content, cancellationToken);

    public async Task SendAsync(IReadOnlyCollection<string> emails, string subject, string content,
                                CancellationToken cancellationToken = default)
    {
        var mailboxAddresses = new List<MailboxAddress>(emails.Count);
        var exceptions = new List<ParseException>();

        foreach (var email in emails)
        {
            try
            {
                mailboxAddresses.Add(MailboxAddress.Parse(email));
            }
            catch (ParseException e)
            {
                exceptions.Add(e);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException("Some emails has wrong format", exceptions);
        }

        var message = new MimeMessage
        {
            From =
            {
                new MailboxAddress(_options.SenderName, _options.SenderEmail)
            },
            Subject = subject,
            Body = new TextPart(TextFormat.Html)
            {
                Text = content
            }
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.SmtpHost, _options.SmtpPort,
            cancellationToken: cancellationToken, useSsl: true);
        await client.AuthenticateAsync(_options.Account, _options.Password, cancellationToken);
        foreach (var address in mailboxAddresses)
        {
            message.To.Clear();
            message.To.Add(address);
            await client.SendAsync(message, cancellationToken);
        }

        await client.DisconnectAsync(true, cancellationToken);
    }
}