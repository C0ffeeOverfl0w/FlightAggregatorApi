namespace FlightAggregator.Application.Features.Flights.Queries;

/// <summary>
/// Валидатор для запроса поиска рейсов.
/// </summary>
public class SearchFlightsQueryValidator : AbstractValidator<SearchFlightsQuery>
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="SearchFlightsQueryValidator"/>.
    /// </summary>
    public SearchFlightsQueryValidator()
    {
        RuleFor(x => x.DepartureTime)
            .NotEmpty().WithMessage("Дата вылета обязательна.")
            .Must(date => date >= DateTime.UtcNow.Date).WithMessage("Дата вылета не может быть в прошлом.");
        RuleFor(x => x.Airline)
            .MaximumLength(50).WithMessage("Название авиакомпании не должно превышать 50 символов.");
        RuleFor(x => x.MaxPrice)
            .GreaterThan(0).WithMessage("Максимальная цена должна быть больше 0.");
        RuleFor(x => x.MaxStops)
            .GreaterThanOrEqualTo(0).WithMessage("Максимальное количество пересадок должно быть больше или равно 0.");
        RuleFor(x => x.SortBy)
            .Matches("price|date").WithMessage("Поле для сортировки должно быть price или date.");
        RuleFor(x => x.Origin)
            .NotEmpty().WithMessage("Город вылета обязателен.")
            .MaximumLength(50).WithMessage("Город вылета не должен превышать 50 символов.");
        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("Город прилёта обязателен.")
            .MaximumLength(50).WithMessage("Город прилёта не должен превышать 50 символов.");
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Номер страницы должен быть больше 0.");
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Размер страницы должен быть от 1 до 50.");
    }
}