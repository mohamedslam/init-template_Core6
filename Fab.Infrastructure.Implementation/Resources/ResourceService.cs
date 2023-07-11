using Autofac;
using Fab.Entities.Models.Resources;
using Fab.Infrastructure.Interfaces.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;
using System.Security.Cryptography;

namespace Fab.Infrastructure.Implementation.Resources;

public class ResourceService : IResourceService, IStartable
{
    private readonly MinioClient _s3;
    private readonly S3Options _options;
    private readonly SHA512 _sha512 = SHA512.Create();
    private readonly ILogger<ResourceService> _logger;

    public ResourceService(MinioClient client, IOptions<S3Options> options, ILogger<ResourceService> logger)
    {
        _s3 = client;
        _logger = logger;
        _options = options.Value;
    }

    public void Start()
    {
        Task.Run(CreateDefaultBucket)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    private async Task CreateDefaultBucket()
    {
        try
        {
            if (!await _s3.BucketExistsAsync(new BucketExistsArgs().WithBucket(_options.Bucket)))
            {
                await _s3.MakeBucketAsync(new MakeBucketArgs().WithBucket(_options.Bucket)
                                                              .WithLocation(_options.Region));
                _logger.LogInformation(@"Bucket ""{Bucket}"" created", _options.Bucket);
            }
            else
            {
                _logger.LogInformation(@"Bucket ""{Bucket}"" already exists", _options.Bucket);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Error during create new bucket");
            throw;
        }
    }

    public Task<byte[]> ComputeResourceHashAsync(Stream content, CancellationToken cancellationToken = default) =>
        _sha512.ComputeHashAsync(content, cancellationToken);

    public async Task<Uri> GetSignedUrlAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        var expiresIn = (int)(_options.DownloadLinkExpiration.Ticks / TimeSpan.TicksPerSecond);
        var name = resource.Name.EndsWith(resource.Extension)
            ? resource.Name
            : resource.Name + resource.Extension;

        var url = await _s3.PresignedGetObjectAsync(
            new PresignedGetObjectArgs().WithBucket(resource.Bucket)
                                        .WithObject(resource.Target)
                                        .WithExpiry(expiresIn)
                                        .WithHeaders(new()
                                        {
                                            ["ResponseContentDisposition"] =
                                                $@"attachment; filename=""${name}""",
                                            ["ResponseContentType"] = resource.ContentType
                                        }));

        return new Uri(url);
    }

    public Task<Stream> GetContentAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<Stream>(TaskCreationOptions.RunContinuationsAsynchronously);

        _s3.GetObjectAsync(new GetObjectArgs().WithBucket(resource.Bucket)
                                              .WithObject(resource.Target)
                                              .WithCallbackStream(tcs.SetResult), cancellationToken)
           .ContinueWith(task =>
           {
               if (task.IsCanceled)
               {
                   tcs.SetCanceled(cancellationToken);
               }

               if (task.IsFaulted && task.Exception != null)
               {
                   tcs.SetException(task.Exception);
               }
           }, TaskContinuationOptions.NotOnRanToCompletion);

        return tcs.Task;
    }

    public async Task<bool> IsResourceExistsAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        try
        {
            await _s3.StatObjectAsync(new StatObjectArgs().WithBucket(resource.Bucket)
                                                          .WithObject(resource.Target), cancellationToken);
            return true;
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
    }

    public async Task<ResourceStats?> GetStatsAsync(Resource resource,
                                                    CancellationToken cancellationToken = default)
    {
        try
        {
            var stats = await _s3.StatObjectAsync(new StatObjectArgs().WithBucket(resource.Bucket)
                                                                      .WithObject(resource.Target), cancellationToken);

            return new ResourceStats(stats.ObjectName, stats.Size, stats.LastModified, stats.ETag,
                stats.ContentType, stats.MetaData);
        }
        catch (ObjectNotFoundException)
        {
            return null;
        }
    }

    public async Task<bool> PutResourceAsync(Resource resource, Stream content,
                                             CancellationToken cancellationToken = default)
    {
        if (content.CanSeek)
        {
            content.Seek(0, SeekOrigin.Begin);
        }
        else if (content.Position != 0)
        {
            throw new ArgumentException("Stream already readed");
        }

        // ReSharper disable once ConstantNullCoalescingCondition
        resource.Bucket ??= _options.Bucket;

        if (string.IsNullOrWhiteSpace(resource.Target))
        {
            throw new ArgumentException("Target must not be null");
        }

        if (resource.Bucket != _options.Bucket &&
            !await _s3.BucketExistsAsync(new BucketExistsArgs().WithBucket(resource.Bucket), cancellationToken))
        {
            throw new ArgumentException($@"Bucket ""{resource.Bucket}"" doesn't exists");
        }

        if (await IsResourceExistsAsync(resource, cancellationToken))
        {
            return false;
        }

        await _s3.PutObjectAsync(
            new PutObjectArgs().WithBucket(resource.Bucket)
                               .WithObject(resource.Target)
                               .WithStreamData(content)
                               .WithObjectSize(content.Length)
                               .WithContentType(resource.ContentType),
            cancellationToken);

        return true;
    }

    public async Task DeleteResourceAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        try
        {
            await _s3.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(resource.Bucket)
                                                              .WithObject(resource.Target), cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "An error occurred during deleting file in s3 bucket (bucket={Bucket}, resource={Resource})",
                resource.Bucket, resource.Target);

            throw;
        }
    }
}