namespace Mealmap.Api;

public class UrlBuilder
{
    private readonly IRequestContext _context;

    public UrlBuilder(IRequestContext context)
    {
        _context = context;
    }

    public Uri NextPage(string basePath, Guid next, int limit, params (string key, string value)[] parameters)
    {
        var builder = RawBuilder();
        builder.Path = basePath;

        builder = appendQuery(builder, "next", next.ToString());
        builder = appendQuery(builder, "limit", limit.ToString());

        foreach (var (key, value) in parameters)
        {
            builder = appendQuery(builder, key, value);
        }

        return builder.Uri;
    }

    private UriBuilder RawBuilder()
    {
        return new UriBuilder()
        {
            Scheme = _context.Scheme,
            Host = _context.Host,
            Port = _context.Port
        };
    }

    private static UriBuilder appendQuery(UriBuilder builder, string key, string value)
    {
        var param = key + "=" + value;

        if (builder.Query != null && builder.Query.Length > 1)
            builder.Query = builder.Query + "&" + param;
        else
            builder.Query = param;

        return builder;
    }
}
