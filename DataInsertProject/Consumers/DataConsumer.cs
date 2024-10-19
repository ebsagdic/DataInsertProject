using DataInsertProject.Context;
using DataInsertProject.Models;
using DataInsertProject.Repositoy;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DataInsertProject.Consumers
{
    public class DataConsumer : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IModel _channel;
        private IConnection _connection;

        public DataConsumer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public  Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "queueEmre", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var dataModels = JsonSerializer.Deserialize<IEnumerable<DataModel>>(message);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await dbContext.DataModels.AddRangeAsync(dataModels);
                    await dbContext.SaveChangesAsync();
                }
            };

            _channel.BasicConsume(queue: "queueEmre", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return Task.CompletedTask;
        }
    }

}
