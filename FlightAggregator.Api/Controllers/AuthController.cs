namespace FlightAggregator.Api.Controllers;

/// <summary>
/// Контроллер для аутентификации пользователей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IJwtTokenGenerator tokenGenerator) : ControllerBase
{
    /// <summary>
    /// Выполняет вход пользователя и генерирует JWT-токен.
    /// </summary>
    /// <param name="request">Запрос на вход, содержащий email.</param>
    /// <returns>JWT-токен в случае успешного входа.</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var userId = Guid.NewGuid().ToString();

        var token = tokenGenerator.GenerateToken(userId, request.Email, "User");

        return Ok(new { Token = token });
    }
}