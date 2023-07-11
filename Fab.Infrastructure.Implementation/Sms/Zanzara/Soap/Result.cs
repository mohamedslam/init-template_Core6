using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[XmlInclude(typeof(BalanceResult))]
[XmlInclude(typeof(SourceAddressResult))]
[XmlInclude(typeof(SmsReceiveResult))]
[XmlInclude(typeof(SmsStatusResult))]
[XmlInclude(typeof(SendSmsResult))]
[DebuggerStepThrough]
[XmlType("Z_result", Namespace="http://www.zanzara.ru/")]
public class Result
{
    [XmlAttribute("res")]
    public int ResultCode { get; set; }

    [XmlAttribute("description")]
    public string Description { get; set; } = null!;
}