using System.Text.RegularExpressions;

namespace Fab.UseCases.Validators;

public class RegularExpressions
{
    public static readonly Regex Kpp = new("^[0-9]{4}[0-9A-Z]{2}[0-9]{3}$",
        RegexOptions.Singleline | RegexOptions.Compiled);

    public static readonly Regex Phone = new(@"^\d{11}$",
        RegexOptions.Singleline | RegexOptions.Compiled);

    public static readonly Regex PhoneFormatted = new(@"^\+?\d[ ]?(\d{3}|\(\d{3}\))[ ]?\d{3}[ -]?\d{2}[ -]?\d{2}$",
        RegexOptions.Singleline | RegexOptions.Compiled);

    // a000aa174
    public static readonly Regex CarNumber = new(@"^[авекмнорстух]{1}\d{3}[авекмнорстух]{2}\d{2,3}$",
        RegexOptions.Singleline | RegexOptions.Compiled);

    // аа0000174
    public static readonly Regex TrailerNumber = new(@"^[авекмнорстух]{2}\d{4}\d{2,3}$",
        RegexOptions.Singleline | RegexOptions.Compiled);
}