using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_result_sendSms", Namespace = "http://www.zanzara.ru/")]
public class SendSmsResult : Result
{
    [XmlElement("sendres", Order = 0)]
    public PushResult[] Results { get; set; } = null!;
}