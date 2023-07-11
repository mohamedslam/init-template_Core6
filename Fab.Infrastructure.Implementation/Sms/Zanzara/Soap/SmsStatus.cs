using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_statusSms", Namespace = "http://www.zanzara.ru/")]
public class SmsStatus
{
    [XmlAttribute]
    public long PushId { get; set; }
}