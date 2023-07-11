namespace Fab.Infrastructure.DataAccess.PostgreSQL.Module;

public class DataAccessOptions
{
    public string Connection { get; set; } = null!;
    public bool Logging { get; set; }
}