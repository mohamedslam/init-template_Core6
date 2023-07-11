namespace Fab.Infrastructure.Interfaces.Push;

public class PushOptions
{
    public string ProjectId { get; set; } = null!;
    public string PrivateKey { get; set; } = null!;
    public string PrivateKeyId { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientEmail { get; set; } = null!;
}