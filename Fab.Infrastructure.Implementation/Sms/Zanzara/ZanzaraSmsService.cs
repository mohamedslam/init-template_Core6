using Autofac;
using Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;
using Fab.Infrastructure.Interfaces.Sms;
using Fab.Utils.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara;

public class ZanzaraSmsService : ISmsService, IStartable
{
    private readonly IZanzaraSmsServiceSoap _soap;
    private readonly UserInfo _userInfo;
    private readonly ILogger<ZanzaraSmsService> _logger;

    public ZanzaraSmsService(IZanzaraSmsServiceSoap soap, IOptions<ZanzaraOptions> options,
                             ILogger<ZanzaraSmsService> logger)
    {
        _soap = soap;
        _logger = logger;
        _userInfo = new UserInfo
        {
            User = options.Value.Username,
            Password = options.Value.Password,
            AppInfo = options.Value.AppInfo,
        };
    }

    public async Task SendAsync(string phone, string content, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending sms: phone={Phone}, contents={@Contents}", phone, content);

        var result = await _soap.SendSmsAsync(new SendSmsRequest
                                {
                                    UserInfo = _userInfo,
                                    SmsCollection = new[]
                                    {
                                        new Soap.Sms
                                        {
                                            SmsId = Guid.NewGuid()
                                                        .ToString(),
                                            Message = content,
                                            Number = phone
                                        }
                                    }
                                })
                                .WithCancellation(cancellationToken);

        _logger.LogInformation("Sms sent: result={ResultCode}, description={@Description}, results={@Results}",
            result.Result.ResultCode,
            result.Result.Description,
            result.Result.Results);
    }

    public void Start() =>
        Task.Run(async () =>
        {
            try
            {
                var result = await _soap.GetTransportStateAsync(_userInfo);

                if (result.ResultCode == 0)
                {
                    var balance = await _soap.GetBalanceAsync(_userInfo);

                    _logger.LogInformation("Service is ready, balance={Balance} sms", balance.Balance);
                }
                else
                {
                    _logger.LogWarning("Something went wrong: result={Result}, description={Description}",
                        result.ResultCode,
                        result.Description);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred");
            }
        });
}