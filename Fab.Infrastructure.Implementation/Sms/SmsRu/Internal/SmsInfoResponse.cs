using Fab.Infrastructure.Interfaces.Sms;
using System.Text.Json.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.SmsRu.Internal;

internal class SmsInfoResponse
{
    public string Status { get; set; } = null!;

    [JsonPropertyName("status_code")]
    public ResponseCode Code { get; set; }

    public IDictionary<string, MessageInfo> Sms { get; set; } = null!;
}