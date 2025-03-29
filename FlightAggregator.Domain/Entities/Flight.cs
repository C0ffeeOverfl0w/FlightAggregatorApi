namespace FlightAggregator.Domain.Entities
{
    public class Flight
    {
        public string Id { get; private set; }
        public string FlightNumber { get; private set; }
        public DateTime DepartureTime { get; private set; }
        public DateTime ArrivalTime { get; private set; }
        public int DurationMinutes { get; private set; }
        public Airline Airline { get; private set; }
        public Money Price { get; private set; }
        public int Stops { get; private set; }
        public List<StopDetailData> StopDetails { get; private set; }
        public string Origin { get; private set; }
        public string Destination { get; private set; }
        public string Source { get; private set; }

        protected Flight()
        {
        }

        public Flight(
            string flightNumber,
            DateTime departureTime,
            DateTime arrivalTime,
            int durationMinutes,
            Airline airline,
            Money price,
            int stops,
            List<StopDetailData> stopDetails,
            string origin,
            string destination,
            string source
        )
        {
            Id = Guid.NewGuid().ToString(); // Или можно передавать извне, или генерировать в БД
            FlightNumber = string.IsNullOrWhiteSpace(flightNumber)
                ? throw new ArgumentException("Номер рейса не может быть пустым.", nameof(flightNumber))
                : flightNumber;

            if (departureTime < DateTime.UtcNow)
                throw new ArgumentException("Дата вылета не может быть в прошлом.", nameof(departureTime));
            DepartureTime = departureTime;

            if (arrivalTime < departureTime)
                throw new ArgumentException("Дата прилета не может быть раньше даты вылета.", nameof(arrivalTime));
            ArrivalTime = arrivalTime;

            if (durationMinutes < 0)
                throw new ArgumentException("Длительность рейса не может быть отрицательной.", nameof(durationMinutes));
            DurationMinutes = durationMinutes;

            Airline = airline ?? throw new ArgumentNullException(nameof(airline));

            if (price.Amount < 0)
                throw new ArgumentException("Цена не может быть отрицательной.", nameof(price));
            Price = price;

            if (stops < 0)
                throw new ArgumentException("Количество пересадок не может быть отрицательным.", nameof(stops));
            Stops = stops;

            StopDetails = stopDetails ?? new List<StopDetailData>();

            Origin = string.IsNullOrWhiteSpace(origin)
                ? throw new ArgumentException("Пункт отправления не может быть пустым.", nameof(origin))
                : origin;

            Destination = string.IsNullOrWhiteSpace(destination)
                ? throw new ArgumentException("Пункт назначения не может быть пустым.", nameof(destination))
                : destination;

            Source = string.IsNullOrWhiteSpace(source)
                ? throw new ArgumentException("Источник не может быть пустым.", nameof(source))
                : source;
        }

        public sealed class StopDetailData
        {
            public string Id { get; private set; }
            public string Airport { get; private set; }
            public int DurationMinutes { get; private set; }

            protected StopDetailData()
            { }

            public StopDetailData(string airport, int durationMinutes)
            {
                Id = Guid.NewGuid().ToString();
                Airport = airport;
                DurationMinutes = durationMinutes;
            }
        }
    }
}