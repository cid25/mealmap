using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mealmap.Api.Settings;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class SettingsController(IOptions<AngularOptions> settings) : ControllerBase
{
    /// <summary>
    /// Returns settings for Angular.
    /// </summary>
    /// <response code="200">Settings Returned</response>
    [HttpGet(Name = nameof(GetSettings))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AngularOptions), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<AngularOptions> GetSettings()
    {
        return Ok(settings.Value);
    }
}
