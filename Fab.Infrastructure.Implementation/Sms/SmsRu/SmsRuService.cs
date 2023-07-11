using Fab.Infrastructure.Implementation.Sms.SmsRu.Internal;
using Fab.Infrastructure.Interfaces.Sms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Net.Mime;

namespace Fab.Infrastructure.Implementation.Sms.SmsRu;

public class SmsRuService : ISmsService
{
    private readonly SmsRuOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<SmsRuService> _logger;

    private static readonly ResponseCode[] SuccessCodes =
    {
        ResponseCode.Completed,
        ResponseCode.MessageDelivered,
        ResponseCode.MessageSent,
        ResponseCode.MessageSending,
        ResponseCode.MessageRead
    };

    public SmsRuService(IOptions<SmsRuOptions> options, HttpClient httpClient, ILogger<SmsRuService> logger)
    {
        _options = options.Value;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> CheckApiKeyAsync(CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<KeyCheckResponse>("/auth/check", cancellationToken: cancellationToken);
        return response.Code == ResponseCode.Completed;
    }

    public Task SendAsync(string phone, string content,
                                             CancellationToken cancellationToken = default) =>
        SendAsync(new[] { phone }, content, cancellationToken);

    public async Task<IReadOnlyCollection<MessageInfo>> SendAsync(IReadOnlyCollection<string> phones,
                                                                  string content,
                                                                  CancellationToken cancellationToken = default)
    {
        if (phones.Any(phone =>
                string.IsNullOrEmpty(phone) || phone.Length != 11 || !phone.StartsWith('7') ||
                !phone.All(char.IsDigit)))
        {
            throw new ArgumentException("Wrong phone format");
        }

        var response = await SendRequestAsync<SmsInfoResponse>(
            "/sms/send",
            new Dictionary<string, string>
            {
                ["msg"] = content,
                ["to"] = string.Join(',', phones)
            },
            cancellationToken
        );

        var messages = response.Sms.Select(x =>
                               {
                                   x.Value.Phone = x.Key;
                                   return x.Value;
                               })
                               .ToList();

        foreach (var message in messages)
        {
            if (SuccessCodes.Contains(message.Status))
            {
                _logger.LogInformation("SMS sent: {Message}", message);
            }
            else
            {
                _logger.LogWarning("SMS sending failed: {Message}", message);
            }
        }

        return messages;
    }

    public async Task<MessageInfo> GetStatusAsync(string messageId, CancellationToken cancellationToken = default)
        => (await GetStatusAsync(new[] { messageId }, cancellationToken)).First();

    public async Task<IReadOnlyCollection<MessageInfo>> GetStatusAsync(IReadOnlyCollection<string> messageIds,
                                                                       CancellationToken cancellationToken =
                                                                           default)
    {
        if (messageIds.Any(string.IsNullOrEmpty))
        {
            throw new ArgumentException("Wrong id format");
        }

        var response = await SendRequestAsync<SmsInfoResponse>(
            "/sms/status",
            new Dictionary<string, string>
            {
                ["sms_id"] = string.Join(',', messageIds)
            },
            cancellationToken
        );

        foreach (var (id, info) in response.Sms)
        {
            info.Id = id;
        }

        return response.Sms.Values.ToList();
    }

    public async Task<double> GetBalanceAsync(CancellationToken cancellationToken = default)
        => (await SendRequestAsync<BalanceResponse>("/my/balance", cancellationToken: cancellationToken)).Balance;

    private async Task<T> SendRequestAsync<T>(string endpoint,
                                              IDictionary<string, string>? args = null,
                                              CancellationToken cancellationToken = default)
    {
        args ??= new Dictionary<string, string>();

        args.Add("api_id", _options.ApiKey);
        args.Add("json", "1");

        if (!string.IsNullOrEmpty(_options.Sender))
        {
            args.Add("from", _options.Sender);
        }

        if (_options.Debug)
        {
            args.Add("test", "1");
        }

        var multipart = new MultipartFormDataContent();

        foreach (var (key, value) in args)
        {
            multipart.Add(new StringContent(value), key);
        }

        var response = await _httpClient.PostAsync(endpoint, multipart, cancellationToken);
        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentType?.MediaType != MediaTypeNames.Application.Json)
        {
            _logger.LogError("Answer from sms.ru has not expected format\nRequest uri: {RequestUri}",
                response.RequestMessage!.RequestUri);
            throw new Exception("Answer from sms.ru has not expected format");
        }

        return (await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken))!;
    }
}