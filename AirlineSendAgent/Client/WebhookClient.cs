using AirlineSendAgent.Dtos;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AirlineSendAgent.Client;

public class WebhookClient : IWebhookClient
{
  private readonly IHttpClientFactory _httpClientFactory;

  public WebhookClient(IHttpClientFactory httpClientFactory)
  {
    _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
  }

  public async Task SendWebhookNotificationAsync(FlightDetailChangePayloadDto flightDetailChangePayloadDto)
  {
    var serializedPayload = JsonSerializer.Serialize(flightDetailChangePayloadDto);
    var httpClient = _httpClientFactory.CreateClient();

    var request = new HttpRequestMessage(HttpMethod.Post, flightDetailChangePayloadDto.WebhookURI);
    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    request.Content = new StringContent(serializedPayload, Encoding.UTF8, "application/json");
    //request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

    try {
      using (var response = await httpClient.SendAsync(request)) {
        Console.WriteLine("Success");
        response.EnsureSuccessStatusCode();
      }
    } catch (Exception ex) {
      Console.WriteLine($"Unsuccessfull {ex.Message}");
    }
  }
}