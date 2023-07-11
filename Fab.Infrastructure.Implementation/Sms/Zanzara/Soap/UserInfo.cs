using System.Diagnostics;
using System.Xml.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[XmlType("Z_UserInfo", Namespace="http://www.zanzara.ru/")]
public class UserInfo
{
    [XmlAttribute("usr")]
    public string User { get; set; } = null!;

    [XmlAttribute("pwd")]
    public string Password { get; set; } = null!;

    [XmlAttribute("appinfo")]
    public string? AppInfo { get; set; }

    [XmlAttribute("pv")]
    public int Pv { get; set; }
}