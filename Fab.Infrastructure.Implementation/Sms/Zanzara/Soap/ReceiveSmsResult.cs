using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_resultreceiveSms", Namespace = "http://www.zanzara.ru/")]
public class ReceiveSmsResult
{
    [XmlAttribute("response_id")]
    public long ResponseId { get; set; }

    [XmlAttribute("src_number")]
    public string SourceNumber { get; set; } = null!;

    [XmlAttribute("number")]
    public string Number { get; set; } = null!;

    [XmlAttribute("text")]
    public string Text { get; set; } = null!;

    [XmlAttribute("request_time")]
    public DateTime RequestTime { get; set; }
}