using DataInsertProject.Models;
using DataInsertProject.Repositoy;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using MoreLinq;
using Microsoft.Extensions.Caching.Distributed;

namespace DataInsertProject.Service
{
    public class DataService
    {
        private readonly DataRepository _repository;
        private readonly IDistributedCache _cache;

        public DataService(DataRepository repository,IDistributedCache cache)
        {
            _cache = cache;
            _repository = repository;
        }


        public async Task<IEnumerable<DataModel>> GetCachedDataAsync()
        {
            var cacheKey = "dataModels";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                // Redis'ten alınan veriyi deserialize et
                return JsonSerializer.Deserialize<IEnumerable<DataModel>>(cachedData);
            }

            // Veri yoksa veritabanından çek ve Redis'e kaydet
            var data = await _repository.GetAllDataAsync();
            var serializedData = JsonSerializer.Serialize(data);

            // Redis'e ekle, 5 dakika süre ile sakla
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

            return data;

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
