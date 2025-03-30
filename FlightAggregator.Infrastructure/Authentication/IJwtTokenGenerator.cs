namespace FlightAggregator.Infrastructure.Authentication;

/// <summary>
/// Генератор JWT-токенов.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Генерирует JWT-токен для пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="email">Адрес почты</param>
    /// <param name="role">Роль</param>
    /// <returns></returns>
    string GenerateToken(string userId, string email, string role = "User");
}