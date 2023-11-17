namespace Mealmap.Api.Common;

public class HostingOptions
{
    public const string SectionName = "Hosting";

    public string[] Hosts { get; set; } = [string.Empty];
}
