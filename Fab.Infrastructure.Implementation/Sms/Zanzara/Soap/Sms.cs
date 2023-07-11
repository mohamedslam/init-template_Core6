using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_sendSms", Namespace = "http://www.zanzara.ru/")]
public class Sms
{
    [XmlElement("message", Order = 0)]
    public string Message { get; set; } = null!;

    [XmlAttribute("sms_id")]
    public string SmsId { get; set; } = null!;

    [XmlAttribute("source_number")]
    public string SourceNumber { get; set; } = null!;

    [XmlAttribute("number")]
    public string Number { get; set; } = null!;

    [XmlAttribute("ttl")]
    public long Ttl { get; set; }

    [XmlAttribute("schedule_delivery_time")]
    public DateTime ScheduleDeliveryTime { get; set; } = DateTime.UtcNow;

    [XmlAttribute("port")]
    public int Port { get; set; }

    [XmlAttribute("email")]
    public string Email { get; set; } = null!;
}