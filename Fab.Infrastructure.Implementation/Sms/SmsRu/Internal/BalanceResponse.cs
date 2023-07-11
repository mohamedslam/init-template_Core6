using Fab.Infrastructure.Interfaces.Sms;
using System.Text.Json.Serialization;

namespace Fab.Infrastructure.Implementation.Sms.SmsRu.Internal;

internal class BalanceResponse
{
    public string Status { get; set; } = null!;

    [JsonPropertyName("status_code")]
    public ResponseCode Code { get; set; }

    public double Balance { get; set; }
}