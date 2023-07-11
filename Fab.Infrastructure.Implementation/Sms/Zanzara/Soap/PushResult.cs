using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_resultPush", Namespace = "http://www.zanzara.ru/")]
public class PushResult
{
    [XmlAttribute("sms_id")]
    public string SmsId { get; set; } = null!;

    [XmlAttribute("push_id")]
    public long PushId { get; set; }

    [XmlAttribute("description")]
    public string Description { get; set; } = null!;

    [XmlAttribute("res")]
    public int Res { get; set; }

    [XmlAttribute("sms_count")]
    public int SmsCount { get; set; }
}