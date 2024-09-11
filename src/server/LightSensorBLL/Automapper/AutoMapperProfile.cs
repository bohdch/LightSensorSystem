using AutoMapper;
using LightSensorBLL.DTOs;
using LightSensorDAL.Entities;

namespace LightSensorBLL.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Client, ClientDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<NewClient, Client>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Login))
                .ForMember(dest => dest.HashedPassword, opt => opt.Ignore());

            CreateMap<TelemetryDto, Telemetry>()
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time).DateTime))
                .ForMember(dest => dest.Illum, opt => opt.MapFrom(src => src.Illum));

            CreateMap<Telemetry, IlluminanceStatisticDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.MaxIlluminance, opt => opt.MapFrom(src => src.Illum));
        }
    }
}
