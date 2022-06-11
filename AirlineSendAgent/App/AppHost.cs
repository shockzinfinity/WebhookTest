using AirlineSendAgent.Client;
using AirlineSendAgent.Data;
using AirlineSendAgent.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AirlineSendAgent.App;

public class AppHost : IAppHost
{
  private readonly SendAgentDbContext _dbContext;
  private readonly IWebhookClient _webhookClient;

  public AppHost(SendAgentDbContext dbContext, IWebhookClient webhookClient)
  {
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    _webhookClient = webhookClient ?? throw new ArgumentNullException(nameof(webhookClient));
  }

  public void Run()
  {
    //Console.WriteLine("Hello world!");

    var factory = new ConnectionFactory { HostName = "localhost", Port = 5672 };
    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel()) {
      channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

      var queueName = channel.QueueDeclare().QueueName;

      channel.QueueBind(queue: queueName, exchange: "trigger", routingKey: "");

      var consumer = new EventingBasicConsumer(channel);
      Console.WriteLine("Listening on the message bus...");

      consumer.Received += async (ModuleHandle, ea) =>
      {
        // consuming logic
        Console.WriteLine("Event is triggered");

        var body = ea.Body;
        var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
        var message = JsonSerializer.Deserialize<NotificationMessageDto>(notificationMessage);

        var webhookToSend = new FlightDetailChangePayloadDto()
        {
          WebhookType = message.WebhookType,
          WebhookURI = string.Empty,
          Secret = string.Empty,
          Publisher = string.Empty,
          OldPrice = message.OldPrice,
          NewPrice = message.NewPrice,
          FlightCode = message.FlightCode
        };

        foreach (var subscription in _dbContext.WebhookSubscriptions.Where(s => s.WebhookType.Equals(message.WebhookType))) {
          webhookToSend.WebhookURI = subscription.WebhookURI;
          webhookToSend.Secret = subscription.Secret;
          webhookToSend.Publisher = subscription.WebhookPublisher;

          await _webhookClient.SendWebhookNotificationAsync(webhookToSend);
        }
      };

      channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
      Console.ReadLine();
    }
  }
}