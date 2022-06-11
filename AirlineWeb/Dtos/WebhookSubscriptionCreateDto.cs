using System.ComponentModel.DataAnnotations;

namespace AirlineWeb.Dtos;

public class WebhookSubscriptionCreateDto
{
  [Required] // because of parameter validation
  public string WebhookURI { get; set; }
  [Required]
  public string WebhookType { get; set; }
}