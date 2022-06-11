using Microsoft.AspNetCore.Mvc;
using TravelAgentWeb.Data;
using TravelAgentWeb.Dtos;

namespace TravelAgentWeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
  private readonly ILogger<NotificationsController> _logger;
  private readonly TravelAgentDbContext _dbContext;

  public NotificationsController(ILogger<NotificationsController> logger, TravelAgentDbContext dbContext)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
  }

  [HttpPost]
  public ActionResult FlightChanged(FlightDetailUpdateDto flightDetailUpdateDto)
  {
    Console.WriteLine($"Webhook received from: {flightDetailUpdateDto.Publisher}");

    var secretModel = _dbContext.SubscriptionSecrets.FirstOrDefault(s =>
      s.Publisher == flightDetailUpdateDto.Publisher &&
      s.Secret == flightDetailUpdateDto.Secret);

    if (secretModel == null) {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Invalid Secret - Ignore Webhook");
      Console.ResetColor();

      return Ok(); // because of Webhook, should return always 200
    } else {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Valid webhook");
      Console.WriteLine($"Old Price {flightDetailUpdateDto.OldPrice}, New Price {flightDetailUpdateDto.NewPrice}");
      Console.ResetColor();

      return Ok();
    }
  }
}