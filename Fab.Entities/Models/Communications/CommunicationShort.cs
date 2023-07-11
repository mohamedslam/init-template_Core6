using Fab.Entities.Enums.Communications;
using Fab.Utils.Extensions;
using System.Text.RegularExpressions;

#pragma warning disable CS8618

namespace Fab.Entities.Models.Communications;

public class CommunicationShort
{
    private static readonly Regex SipRegex = new(@"^sip:\+?(\d{11})@.+$",
        RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex PhoneRegex = new(@"^\+?(\d{11})$",
        RegexOptions.Singleline | RegexOptions.Compiled);

    public CommunicationType Type { get; }
    public string Value { get; }
    public string? DeviceId { get; }

    public CommunicationShort(CommunicationType type, string value)
    {
        Type = type;
        Value = value;
    }

    public static CommunicationShort? FromSip(string? sip)
    {
        if (!(sip?.StartsWith("sip:") ?? false))
        {
            return null;
        }

        return new CommunicationShort(
            CommunicationType.Phone,
            SipRegex.Replace(sip, "$1"));
    }

    public static CommunicationShort? FromPhone(string? phone)
    {
        if (phone?.Let(x => !PhoneRegex.IsMatch(x)) ?? true)
        {
            return null;
        }

        return new CommunicationShort(
            CommunicationType.Phone,
            PhoneRegex.Replace(phone, "$1"));
    }
}