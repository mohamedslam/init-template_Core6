using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(WrapperName = "sms_send", WrapperNamespace = "http://www.zanzara.ru/", IsWrapped = true)]
public class SendSmsRequest
{
    [MessageBodyMember(Name = "userinfo", Namespace = "http://www.zanzara.ru/", Order = 0)]
    public UserInfo UserInfo { get; set; } = null!;

    [MessageBodyMember(Namespace = "http://www.zanzara.ru/", Order = 1)]
    [XmlElement("sms")]
    public Sms[] SmsCollection { get; set; } = Array.Empty<Sms>();
}