namespace FlightAggregator.Infrastructure.Authentication;

/// <summary>
/// Генератор JWT-токенов.
/// </summary>
/// <param name="configuration">Конфигурация</param>
public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration = configuration;

    /// <inheritdoc/>
    public string GenerateToken(string userId, string email, string role = "User")
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim(ClaimTypes.Role, role)
    };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"]));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}