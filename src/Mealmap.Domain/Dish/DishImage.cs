using Microsoft.EntityFrameworkCore;

namespace Mealmap.Domain.DishAggregate;

[Owned]
public record DishImage
{
    public byte[] Content { get; }

    public string ContentType { get; }

    internal DishImage(byte[] content, string contentType)
    {
        (Content, ContentType) = (content, contentType);
    }
}
