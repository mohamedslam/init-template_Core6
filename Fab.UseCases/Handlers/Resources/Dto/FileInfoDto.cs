namespace Fab.UseCases.Handlers.Resources.Dto;

public class FileInfoDto
{
    public string OriginalName { get; set; } = null!;

    public long Size { get; set; }
    public string ContentType { get; set; } = null!;
    public string Extension { get; set; } = null!;

    public Stream Content { get; set; } = null!;
}