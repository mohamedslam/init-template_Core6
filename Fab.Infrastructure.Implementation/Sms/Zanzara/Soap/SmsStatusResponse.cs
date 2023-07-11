using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(WrapperName = "sms_statusResponse", WrapperNamespace = "http://www.zanzara.ru/", IsWrapped = true)]
public class SmsStatusResponse
{
    [MessageBodyMember(Name = "sms_statusResult", Namespace = "http://www.zanzara.ru/", Order = 0)]
    public SmsStatusResult Result { get; set; } = null!;
}