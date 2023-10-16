namespace Mealmap.Api;

public class AngularSettings
{
    public const string SectionName = "Angular";

    public string? ClientId { get; set; }
    public string? Authority { get; set; }
    public string? RedirectUri { get; set; }
    public string? ApiScope { get; set; }
}
