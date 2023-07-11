using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_result_statusSms", Namespace = "http://www.zanzara.ru/")]
public class SmsStatusResult : Result
{
    [XmlElement("statusres", Order = 0)]
    public StatusResult[] Statuses { get; set; } = Array.Empty<StatusResult>();
}