namespace Mealmap.Api.Shared;

public class RequestContext : IRequestContext
{
    private readonly HttpContext? _context;

    public RequestContext(IHttpContextAccessor httpContextAccessor)
        => _context = httpContextAccessor.HttpContext;

    public string Scheme => _context != null ? _context.Request.Scheme : string.Empty;

    public string Host => _context != null ? _context.Request.Host.Host : string.Empty;

    public int Port => _context != null ? _context.Request.Host.Port ?? -1 : -1;

    public string Method => _context != null ? _context.Request.Method : string.Empty;

    public string? IfMatchHeader => _context != null && _context.Request.Headers.IfMatch.Any() ? _context?.Request.Headers.IfMatch.First() : null;
}
