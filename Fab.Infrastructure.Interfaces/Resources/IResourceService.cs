using Fab.Entities.Models.Resources;

namespace Fab.Infrastructure.Interfaces.Resources;

public interface IResourceService
{
    Task<byte[]> ComputeResourceHashAsync(Stream content, CancellationToken cancellationToken = default);

    Task<Uri> GetSignedUrlAsync(Resource resource, CancellationToken cancellationToken = default);
    Task<Stream> GetContentAsync(Resource resource, CancellationToken cancellationToken = default);

    Task<bool> IsResourceExistsAsync(Resource resource, CancellationToken cancellationToken = default);
    Task<ResourceStats?> GetStatsAsync(Resource resource, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Загружает ресурс в бакет, если его там ещё нет
    /// </summary>
    /// <returns>Возвращает true, если файл успешно загружен или false, если файл уже есть в бакете</returns>
    Task<bool> PutResourceAsync(Resource resource, Stream content,
                                CancellationToken cancellationToken = default);

    /// <summary>
    ///     Жестко удаляет <see cref="Resource"/> из базы и удаляет файл из бакета в случае,
    ///     если нет других ресурсов, которые ссылаются на этот файл
    /// </summary>
    Task DeleteResourceAsync(Resource resource, CancellationToken cancellationToken = default);
}