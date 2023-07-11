using System.ServiceModel;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[ServiceContract(Namespace = "http://www.zanzara.ru/", ConfigurationName = "ZanzaraSMSServiceSoap")]
public interface IZanzaraSmsServiceSoap
{
    [OperationContract(Action = "http://www.zanzara.ru/HelloWorld", ReplyAction = "*")]
    [XmlSerializerFormat(SupportFaults = true)]
    Task<string> HelloWorldAsync();

    [OperationContract(Name = "transport_state", Action = "http://www.zanzara.ru/transport_state", ReplyAction = "*")]
    [XmlSerializerFormat(SupportFaults = true)]
    Task<Result> GetTransportStateAsync([MessageParameter(Name = "userinfo")] UserInfo userInfo);

    [OperationContract(Name = "sms_send", Action = "http://www.zanzara.ru/sms_send", ReplyAction = "*")]
    [XmlSerializerFormat(SupportFaults = true)]
    Task<SendSmsResponse> SendSmsAsync(SendSmsRequest request);

    [OperationContract(Name = "VCARD_send", Action = "http://www.zanzara.ru/VCARD_send", ReplyAction = "*")]
    [XmlSerializerFormat(SupportFaults = true)]
    Task<SendVCardResponse> SendVCardAsync(SendVCardRequest request);

    [OperationContract(Name = "sms_status", Action = "http://www.zanzara.ru/sms_status", ReplyAction = "*")]
    [XmlSerializerFormat(SupportFaults = true)]
    Task<SmsStatusResponse> GetSmsStatusAsync(SmsStatusRequest request);

    [OperationContract(Name = "sms_receive", Action = "http://www.zanzara.ru/sms_receive", ReplyAction = "*")]
    [XmlSerializerFormat(SupportFaults = true)]
    Task<SmsReceiveResult> ReceiveSmsAsync([MessageParameter(Name = "userinfo")] UserInfo userInfo,
                                           [MessageParameter(Name = "req_sms")] ReceiveSmsRequest request);

    [OperationContract(Name = "get_sourceaddress", Action = "http://www.zanzara.ru/get_sourceaddress",
        ReplyAction = "*")]
    [XmlSerializerFormat(SupportFaults = true)]
    Task<SourceAddressResult> GetSourceAddressesAsync([MessageParameter(Name = "userinfo")] UserInfo userInfo);

    [OperationContract(Name = "get_balance", Action = "http://www.zanzara.ru/get_balance", ReplyAction = "*")]
    [XmlSerializerFormat(SupportFaults = true)]
    Task<BalanceResult> GetBalanceAsync([MessageParameter(Name = "userinfo")] UserInfo userInfo);
}