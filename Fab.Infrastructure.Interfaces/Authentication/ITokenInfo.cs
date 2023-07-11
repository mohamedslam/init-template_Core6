using System.Net;

namespace Fab.Infrastructure.Interfaces.Authentication;

public interface ITokenInfo
{
    public IPAddress IpAddress { get; set; }
    public string UserAgent { get; set; }
}