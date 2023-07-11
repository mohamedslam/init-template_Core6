using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_result_Balance", Namespace="http://www.zanzara.ru/")]
public partial class BalanceResult : Result
{
    [XmlElement("balance", Order=0)]
    public int Balance { get; set; }

    [XmlElement("quota_type", Order=1)]
    public int QuotaType { get; set; }

    [XmlElement("credit_limit", Order=2)]
    public int CreditLimit { get; set; }

    [XmlElement("day_count", Order=3)]
    public int DayCount { get; set; }
}