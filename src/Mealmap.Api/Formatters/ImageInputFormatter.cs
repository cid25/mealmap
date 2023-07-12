using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Mealmap.Api.Formatters
{
    /// <summary>
    /// Formatter that parses image/jpeg and image/png to a raw byte array.
    /// Can be used in action methods like this:
    /// 
    /// public ActionResult PutImage([FromBody] ImageInput image)
    /// </summary>
    public class ImageInputFormatter : InputFormatter
    {
        public ImageInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("image/jpeg"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("image/png"));
        }

        public override Boolean CanRead(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var contentType = context.HttpContext.Request.ContentType;
            if (contentType is ("image/jpeg" or "image/png"))
                return true;

            return false;
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType!;

            const int defaultStreamCapacity = 1024;
            using (var ms = new MemoryStream(defaultStreamCapacity))
            {
                
                await request.Body.CopyToAsync(ms);

                var image = new Image(
                    content: ms.ToArray(),
                    contentType: contentType
                );

                return await InputFormatterResult.SuccessAsync(image);
            }
        }
    }
}
