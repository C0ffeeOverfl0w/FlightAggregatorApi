namespace FlightAggregator.Application.MappingProfiles;

public class FlightProfile : Profile
{
    public FlightProfile()
    {
        CreateMap<Flight.StopDetailData, StopDetailDataDto>()
            .ForMember(dest => dest.Airport, opt => opt.MapFrom(src => src.Airport))
            .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.DurationMinutes));

        CreateMap<Money, decimal>().ConvertUsing(m => m.Amount);
        CreateMap<Airline, string>().ConvertUsing(a => a.Name);

        CreateMap<Flight, FlightDto>()
            .ForMember(dest => dest.FlightNumber, opt => opt.MapFrom(src => src.FlightNumber))
            .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureTime))
            .ForMember(dest => dest.ArrivalTime, opt => opt.MapFrom(src => src.ArrivalTime))
            .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.DurationMinutes))
            .ForMember(dest => dest.Airline, opt => opt.MapFrom(src => src.Airline.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Stops, opt => opt.MapFrom(src => src.Stops))
            .ForMember(dest => dest.StopDetails, opt => opt.MapFrom(src => src.StopDetails))
            .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.Origin))
            .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Destination));
    }
}