namespace Mealmap.Api.Shared;

public class HostingOptions
{
    public const string SectionName = "Hosting";

    public string[] Hosts { get; set; } = { string.Empty };
}
