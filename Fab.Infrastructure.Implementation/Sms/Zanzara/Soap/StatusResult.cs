using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_resultStatus", Namespace = "http://www.zanzara.ru/")]
public class StatusResult
{
    [XmlAttribute("push_id")]
    public long PushId { get; set; }

    [XmlAttribute("description")]
    public string Description { get; set; } = null!;

    [XmlAttribute("status")]
    public int Status { get; set; }

    [XmlAttribute("statustime")]
    public DateTime StatusTime { get; set; }

    [XmlAttribute("SMS_sent")]
    public int Sent { get; set; }

    [XmlAttribute("SMS_dlvr")]
    public int Delivered { get; set; }
}