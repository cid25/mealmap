namespace Mealmap.Api;

public class HostingOptions
{
    public const string SectionName = "Hosting";

    public string[] Hosts { get; set; } = { String.Empty };
}
