using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_result_SourceAddress", Namespace="http://www.zanzara.ru/")]
public class SourceAddressResult : Result
{
    [XmlElement("sourceaddress", Order=0)]
    public string[] SourceAddresses { get; set; } = Array.Empty<string>();
}