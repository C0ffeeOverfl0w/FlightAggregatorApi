namespace FlightAggregator.Api.Controllers;

/// <summary>
/// Контроллер для получения информации о текущем пользователе.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    /// <summary>
    /// Возвращает информацию о текущем авторизованном пользователе.
    /// </summary>
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var username = User.Identity?.Name ?? "anonymous";

        var claims = User.Claims.Select(c => new
        {
            c.Type,
            c.Value
        });

        return Ok(new
        {
            Username = username,
            Claims = claims
        });
    }
}