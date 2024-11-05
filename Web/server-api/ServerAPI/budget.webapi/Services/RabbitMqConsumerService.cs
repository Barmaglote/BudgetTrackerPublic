using budget.core.Models;
using budget.core.Services.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;

namespace budget.webapi.Services {
  public class RabbitMqConsumerService : IDisposable {
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMqConsumerService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory) {
      _configuration = configuration;
      _serviceScopeFactory = serviceScopeFactory;
      var factory = new ConnectionFactory() {
        HostName = configuration["RabbitMQ:HostName"],
        UserName = configuration["RabbitMQ:UserName"],
        Password = configuration["RabbitMQ:Password"],
        Port = int.Parse(configuration["RabbitMQ:Port"])
      };

      _connection = factory.CreateConnection();
      _channel = _connection.CreateModel();

      _channel.QueueDeclare(queue: configuration["RabbitMQ:Queue"],
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);
    }

    public void StartListening() {
      var consumer = new EventingBasicConsumer(_channel);
      consumer.Received += (model, ea) =>
      {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        using (var scope = _serviceScopeFactory.CreateScope()) {
          HandleSubscriptionMessage(message, scope.ServiceProvider.GetRequiredService<IUserSettingsService>());
        }
      };

      _channel.BasicConsume(queue: _configuration["RabbitMQ:Queue"],
                             autoAck: true,
                             consumer: consumer);
    }

    public void Dispose() {
      _channel.Close();
      _connection.Close();
    }

    public void HandleSubscriptionMessage(string message, IUserSettingsService userSettingsService) {
      var subscriptionMessage = JsonConvert.DeserializeObject<UserSubscribtionMessage>(message);

      if (subscriptionMessage != null) {
        var userSubscribtion = new UserSubscribtion(subscriptionMessage);

        var userId = subscriptionMessage switch {
          { Provider: "GOOGLE" } => string.Format("GG-{0}", subscriptionMessage.UserId),
          { Provider: "FACEBOOK" } => string.Format("FB-{0}", subscriptionMessage.UserId),
          _ => string.Format("BT-{0}", subscriptionMessage.UserId)
        };

        if (userSubscribtion.IsActive) {
          userSettingsService.UpsertUserSubscription(userId, userSubscribtion);
        } else {
          userSettingsService.RemoveUserSubscription(userId, userSubscribtion.SubscribtionId);
        }
      }
    }
  }
}