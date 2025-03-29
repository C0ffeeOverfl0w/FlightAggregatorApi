namespace FlightAggregator.Application.MappingProfiles;

public class ResponseToDomainProfile : Profile
{
    public ResponseToDomainProfile()
    {
        CreateMap<StopDetailDataDto, Flight.StopDetailData>();

        CreateMap<FlightDto, Flight>()
            .ForMember(dest => dest.Airline,
                opt => opt.MapFrom(src => src.Airline))
            .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src => new Money(src.Price, "USD")))
            .ForMember(dest => dest.StopDetails,
                opt => opt.MapFrom(src => src.StopDetails ?? new List<StopDetailDataDto>()));
    }
}