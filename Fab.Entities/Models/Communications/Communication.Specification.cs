using Fab.Entities.Abstractions;
using Fab.Entities.Enums.Communications;

namespace Fab.Entities.Models.Communications;

public partial class Communication
{
    public static Spec<Communication> ByTypeAndValue(CommunicationType type, string value) =>
        new(x => x.Type == type && x.Value == value);

    public static Spec<Communication> ByShortCommunication(CommunicationShort communication) =>
        ByTypeAndValue(communication.Type, communication.Value);
}