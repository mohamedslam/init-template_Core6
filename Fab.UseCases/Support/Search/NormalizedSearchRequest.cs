namespace Fab.UseCases.Support.Search;

public class NormalizedSearchRequest : ISearchRequest
{
    private readonly ISearchRequest _request;

    public string? Query => _request.Query
                                    ?.Replace("%", "%%");

    public NormalizedSearchRequest(ISearchRequest request) =>
        _request = request;
}