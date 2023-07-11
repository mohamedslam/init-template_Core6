using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_result_receiveSms", Namespace = "http://www.zanzara.ru/")]
public class SmsReceiveResult : Result
{
    [XmlElement("receive", Order = 0)]
    public ReceiveSmsResult[] Received { get; set; } = Array.Empty<ReceiveSmsResult>();

    [XmlAttribute("sms_count")]
    public int SmsCount { get; set; }

    [XmlAttribute("sms_remains")]
    public int SmsRemains { get; set; }

    [XmlAttribute("request_id")]
    public string RequestId { get; set; } = null!;
}