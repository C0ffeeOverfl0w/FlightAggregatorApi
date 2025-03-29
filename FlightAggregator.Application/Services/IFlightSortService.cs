namespace FlightAggregator.Application.Services
{
    /// <summary>
    /// Интерфейс для сортировки рейсов.
    /// </summary>
    public interface IFlightSortService
    {
        /// <summary>
        /// Сортирует коллекцию рейсов на основе заданного запроса.
        /// </summary>
        /// <param name="flights">Коллекция рейсов для сортировки.</param>
        /// <param name="query">Запрос, содержащий критерии сортировки.</param>
        /// <returns>Отсортированная коллекция рейсов.</returns>
        IEnumerable<Flight> Sort(IEnumerable<Flight> flights, SearchFlightsQuery query);
    }
}