using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FlightsController : ControllerBase
{
  private readonly ILogger<FlightsController> _logger;
  private readonly AirlineDbContext _dbContext;
  private readonly IMapper _mapper;

  public FlightsController(ILogger<FlightsController> logger, AirlineDbContext dbContext, IMapper mapper)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
  }

  [HttpGet("{flightCode}", Name = "GetFlightDetailsByCode")]
  public ActionResult<FlightDetailReadDto> GetFlightDetailsByCode(string flightCode)
  {
    var flight = _dbContext.FlightDetails.FirstOrDefault(f => f.FlightCode == flightCode);

    if (flight == null) {
      return NotFound();
    }

    return Ok(_mapper.Map<FlightDetailReadDto>(flight));
  }

  [HttpPost]
  public ActionResult<FlightDetailReadDto> CreateFlight(FlightDetailCreateDto flightDetailCreateDto)
  {
    var flight = _dbContext.FlightDetails.FirstOrDefault(f => f.FlightCode == flightDetailCreateDto.FlightCode);

    if (flight == null) {
      var flightDetailModel = _mapper.Map<FlightDetail>(flightDetailCreateDto);

      try {
        _dbContext.FlightDetails.Add(flightDetailModel);
        _dbContext.SaveChanges();
      } catch (Exception ex) {
        return BadRequest(ex.Message);
      }

      var flightDetailReadDto = _mapper.Map<FlightDetailReadDto>(flightDetailModel);

      return CreatedAtRoute(nameof(GetFlightDetailsByCode), new { flightCode = flightDetailReadDto.FlightCode }, flightDetailReadDto);
    } else {
      return NoContent();
    }
  }

  [HttpPut("{id}")]
  public ActionResult UpdateFlight(int id, FlightDetailUpdateDto flightDetailUpdateDto)
  {
    var flight = _dbContext.FlightDetails.FirstOrDefault(f => f.Id == id);

    if(flight == null) {
      return NotFound();
    }

    _mapper.Map(flightDetailUpdateDto, flight);
    _dbContext.SaveChanges();

    return NoContent();
  }
}