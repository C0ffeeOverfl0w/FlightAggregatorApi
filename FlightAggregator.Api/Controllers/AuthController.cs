namespace FlightAggregator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IJwtTokenGenerator tokenGenerator) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var userId = Guid.NewGuid().ToString();

        var token = tokenGenerator.GenerateToken(userId, request.Email, "User");

        return Ok(new { Token = token });
    }
}