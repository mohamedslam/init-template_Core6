namespace Fab.Infrastructure.Interfaces.Resources;

public class ResourceStats
{
    public string ObjectName { get; }
    public long Size { get; }
    public DateTime LastModified { get; }
    public string ETag { get; }
    public string ContentType { get; }
    public IDictionary<string, string> MetaData { get; }

    public ResourceStats(string objectName, long size, DateTime lastModified, string eTag, string contentType,
                         IDictionary<string, string> metaData)
    {
        ObjectName = objectName;
        Size = size;
        LastModified = lastModified;
        ETag = eTag;
        ContentType = contentType;
        MetaData = metaData;
    }
}