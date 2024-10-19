using DataInsertProject.Models;
using DataInsertProject.Repositoy;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using MoreLinq;

namespace DataInsertProject.Service
{
    public class DataService
    {
        private readonly DataRepository _repository;

        public DataService(DataRepository repository)
        {
            _repository = repository;
        }

        public async Task ProcessDataAsync(IEnumerable<DataModel> dataModels)
        {
            var dataList = dataModels.ToList();

            if (dataList.Count > 100)
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "queueEmre", durable: false, exclusive: false, autoDelete: false, arguments: null);

                foreach (var batch in dataList.Batch(10))
                {
                    var message = JsonSerializer.Serialize(batch);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "", routingKey: "queueEmre", basicProperties: null, body: body);
                }
            }
            else
            {
                await _repository.InsertDataAsync(dataList);
            }
        }
    }
}
