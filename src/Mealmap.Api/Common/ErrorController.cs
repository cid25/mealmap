using Microsoft.AspNetCore.Mvc;

namespace Mealmap.Api.Common;

[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    [Route("api/error")]
    public IActionResult HandleError()
        => Problem();
}
