using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mealmap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class SettingsController : ControllerBase
{
    private readonly IOptions<AngularSettings> _settings;

    public SettingsController(IOptions<AngularSettings> settings)
        => _settings = settings;

    /// <summary>
    /// Returns settings for Angular.
    /// </summary>
    /// <response code="200">Settings Returned</response>
    [HttpGet(Name = nameof(GetSettings))]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AngularSettings), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<AngularSettings> GetSettings()
    {
        return Ok(_settings.Value);
    }
}
