using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(WrapperName = "sms_status", WrapperNamespace = "http://www.zanzara.ru/", IsWrapped = true)]
public class SmsStatusRequest
{
    [MessageBodyMember(Name = "userinfo", Namespace = "http://www.zanzara.ru/", Order = 0)]
    public UserInfo UserInfo { get; set; } = null!;

    [MessageBodyMember(Name = "status", Namespace = "http://www.zanzara.ru/", Order = 1)]
    [XmlElement("status")]
    public SmsStatus[] Status { get; set; } = Array.Empty<SmsStatus>();
}