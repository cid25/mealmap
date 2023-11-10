namespace Mealmap.Api.Dishes;

#pragma warning disable CS8618
public record Image
{

    private string _contentType;

    public byte[] Content { get; init; }

    public string ContentType
    {
        get => _contentType;
        set
        {
            if (value is not ("image/jpeg" or "image/png"))
                throw new ArgumentException("ContentType must be one of image/jpeg, image/png");
            _contentType = value;
        }
    }

    public Image(byte[] content, string contentType)
    {
        Content = content;
        ContentType = contentType;
    }
}
#pragma warning restore CS8618

