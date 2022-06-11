using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;

namespace AirlineWeb.Profiles;

public class FlightDetailProfile : Profile
{
  public FlightDetailProfile()
  {
    CreateMap<FlightDetailCreateDto, FlightDetail>();
    CreateMap<FlightDetailUpdateDto, FlightDetail>();
    CreateMap<FlightDetail, FlightDetailReadDto>();
  }
}