using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_receiveSms", Namespace="http://www.zanzara.ru/")]
public class ReceiveSmsRequest
{
    [XmlAttribute("request_id")]
    public string RequestId { get; set; } = null!;

    [XmlAttribute("count")]
    public int Count { get; set; }
}