using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[SuppressMessage("ReSharper", "PartialMethodWithSinglePart")]
[SuppressMessage("ReSharper", "InvocationIsSkipped")]
public partial class ZanzaraSMSServiceSoapClient : ClientBase<IZanzaraSmsServiceSoap>, IZanzaraSmsServiceSoap
{
    /// <summary>
    /// Реализуйте этот разделяемый метод для настройки конечной точки службы.
    /// </summary>
    /// <param name="serviceEndpoint">Настраиваемая конечная точка</param>
    /// <param name="clientCredentials">Учетные данные клиента.</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint,
                                          System.ServiceModel.Description.ClientCredentials clientCredentials);

    public ZanzaraSMSServiceSoapClient(EndpointConfiguration endpointConfiguration) :
        base(GetBindingForEndpoint(endpointConfiguration), GetEndpointAddress(endpointConfiguration))
    {
        Endpoint.Name = endpointConfiguration.ToString();
        ConfigureEndpoint(Endpoint, ClientCredentials);
    }

    public ZanzaraSMSServiceSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) :
        base(GetBindingForEndpoint(endpointConfiguration), new EndpointAddress(remoteAddress))
    {
        Endpoint.Name = endpointConfiguration.ToString();
        ConfigureEndpoint(Endpoint, ClientCredentials);
    }

    public ZanzaraSMSServiceSoapClient(EndpointConfiguration endpointConfiguration, EndpointAddress remoteAddress) :
        base(GetBindingForEndpoint(endpointConfiguration), remoteAddress)
    {
        Endpoint.Name = endpointConfiguration.ToString();
        ConfigureEndpoint(Endpoint, ClientCredentials);
    }

    public ZanzaraSMSServiceSoapClient(Binding binding, EndpointAddress remoteAddress) :
        base(binding, remoteAddress)
    {
    }

    public Task<string> HelloWorldAsync() =>
        Channel.HelloWorldAsync();

    public Task<Result> GetTransportStateAsync(UserInfo userInfo) =>
        Channel.GetTransportStateAsync(userInfo);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<SendSmsResponse> IZanzaraSmsServiceSoap.SendSmsAsync(SendSmsRequest request) =>
        Channel.SendSmsAsync(request);

    public Task<SendSmsResponse> SendSmsAsync(UserInfo userinfo, Sms[] sms)
    {
        var inValue = new SendSmsRequest
        {
            UserInfo = userinfo,
            SmsCollection = sms
        };
        return ((IZanzaraSmsServiceSoap)this).SendSmsAsync(inValue);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<SendVCardResponse> IZanzaraSmsServiceSoap.SendVCardAsync(SendVCardRequest request) =>
        Channel.SendVCardAsync(request);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<SmsStatusResponse> IZanzaraSmsServiceSoap.GetSmsStatusAsync(SmsStatusRequest request) =>
        Channel.GetSmsStatusAsync(request);

    public Task<SmsReceiveResult> ReceiveSmsAsync(UserInfo userinfo, ReceiveSmsRequest request) =>
        Channel.ReceiveSmsAsync(userinfo, request);

    public Task<SourceAddressResult> GetSourceAddressesAsync(UserInfo userinfo) =>
        Channel.GetSourceAddressesAsync(userinfo);

    public Task<BalanceResult> GetBalanceAsync(UserInfo userinfo) =>
        Channel.GetBalanceAsync(userinfo);

    public virtual Task OpenAsync() =>
        Task.Factory.FromAsync(((ICommunicationObject)this).BeginOpen(null, null),
            ((ICommunicationObject)this).EndOpen);

    public virtual Task CloseAsync() =>
        Task.Factory.FromAsync(((ICommunicationObject)this).BeginClose(null, null),
            ((ICommunicationObject)this).EndClose);

    private static Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration) =>
        endpointConfiguration switch
        {
            EndpointConfiguration.ZanzaraSMSServiceSoap => new BasicHttpBinding
            {
                MaxBufferSize = int.MaxValue,
                ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                MaxReceivedMessageSize = int.MaxValue,
                AllowCookies = true,
                Security =
                {
                    Mode = BasicHttpSecurityMode.Transport
                }
            },

            EndpointConfiguration.ZanzaraSMSServiceSoap12 => new CustomBinding
            {
                Elements =
                {
                    new TextMessageEncodingBindingElement
                    {
                        MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None)
                    },
                    new HttpTransportBindingElement
                    {
                        AllowCookies = true,
                        MaxBufferSize = int.MaxValue,
                        MaxReceivedMessageSize = int.MaxValue
                    }
                }
            },

            _ => throw new InvalidOperationException(
                $"Не удалось найти конечную точку с именем \"{endpointConfiguration}\".")
        };

    private static EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
    {
        return endpointConfiguration switch
        {
            EndpointConfiguration.ZanzaraSMSServiceSoap => new EndpointAddress(
                "http://192.168.10.36:8088/ws/ZService.asmx"),

            EndpointConfiguration.ZanzaraSMSServiceSoap12 => new EndpointAddress(
                "http://192.168.10.36:8088/ws/ZService.asmx"),

            _ => throw new InvalidOperationException(
                $"Не удалось найти конечную точку с именем \"{endpointConfiguration}\".")
        };
    }

    public enum EndpointConfiguration
    {
        ZanzaraSMSServiceSoap,
        ZanzaraSMSServiceSoap12,
    }
}