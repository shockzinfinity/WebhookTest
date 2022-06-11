using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebhookSubscriptionController : ControllerBase
{
  private readonly ILogger<WebhookSubscriptionController> _logger;
  private readonly AirlineDbContext _dbContext;
  private readonly IMapper _mapper;

  public WebhookSubscriptionController(ILogger<WebhookSubscriptionController> logger, AirlineDbContext dbContext, IMapper mapper)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
  }

  [HttpGet("{secret}", Name = "GetSubscriptionBySecret")]
  public ActionResult<WebhookSubscriptionReadDto> GetSubscriptionBySecret(string secret)
  {
    var subscription = _dbContext.WebhookSubscriptions.FirstOrDefault(s => s.Secret == secret);

    if (subscription == null) {
      return NotFound();
    }

    return Ok(_mapper.Map<WebhookSubscriptionReadDto>(subscription));
  }

  [HttpPost]
  public ActionResult<WebhookSubscriptionReadDto> CreateSubscription(WebhookSubscriptionCreateDto webhookSubscriptionCreateDto)
  {
    var subscription = _dbContext.WebhookSubscriptions.FirstOrDefault(s => s.WebhookURI == webhookSubscriptionCreateDto.WebhookURI);

    if (subscription == null) {
      subscription = _mapper.Map<WebhookSubscription>(webhookSubscriptionCreateDto);
      subscription.Secret = Guid.NewGuid().ToString();
      subscription.WebhookPublisher = "PanKOR";

      try {
        _dbContext.WebhookSubscriptions.Add(subscription);
        _dbContext.SaveChanges();
      } catch (Exception ex) {
        return BadRequest(ex.Message);
      }

      var webhookSubscriptionReadDto = _mapper.Map<WebhookSubscriptionReadDto>(subscription);

      return CreatedAtRoute(nameof(GetSubscriptionBySecret), new { secret = webhookSubscriptionReadDto.Secret }, webhookSubscriptionReadDto);
    } else {
      return NoContent();
    }
  }
}