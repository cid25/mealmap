namespace Mealmap.Api.Settings;

public class AngularOptions
{
    public const string SectionName = "Angular";

    public string? ClientId { get; set; }
    public string? Authority { get; set; }
    public string? RedirectUri { get; set; }
    public string? ApiScope { get; set; }
}
