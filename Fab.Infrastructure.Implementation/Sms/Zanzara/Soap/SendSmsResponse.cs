using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(WrapperName = "sms_sendResponse", WrapperNamespace = "http://www.zanzara.ru/", IsWrapped = true)]
public class SendSmsResponse
{
    [MessageBodyMember(Name = "sms_sendResult", Namespace = "http://www.zanzara.ru/", Order = 0)]
    public SendSmsResult Result { get; set; } = null!;
}