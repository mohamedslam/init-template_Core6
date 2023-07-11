using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;

[DebuggerStepThrough]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(WrapperName = "VCARD_sendResponse", WrapperNamespace = "http://www.zanzara.ru/", IsWrapped = true)]
public class SendVCardResponse
{
    [MessageBodyMember(Name = "VCARD_sendResult", Namespace = "http://www.zanzara.ru/", Order = 0)]
    public SendSmsResult Result { get; set; } = null!;
}