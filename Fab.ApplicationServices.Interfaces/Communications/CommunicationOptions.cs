using System.Diagnostics.CodeAnalysis;
using Fab.Entities.Enums.Communications;


namespace Fab.ApplicationServices.Interfaces.Communications;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class CommunicationOptions
{
    /// <summary>
    ///     Дефолтный код подтверждения для коммуникации
    /// </summary>
    /// <remarks>
    ///     Не использовать в production окружении!
    /// </remarks>
    public string? DefaultCode { get; set; }

    /// <summary>
    ///     Дефолтные коды подтверждения для выбранных коммуникаций
    /// </summary>
    public List<StaticCode> StaticCodes { get; set; } = new();

    /// <summary>
    ///     Таймаут для переотправки кода подтверждения на коммуникацию
    /// </summary>
    public TimeSpan VerificationTimeout { get; set; }

    public class StaticCode
    {
        public CommunicationType Type { get; set; }
        public string Value { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}