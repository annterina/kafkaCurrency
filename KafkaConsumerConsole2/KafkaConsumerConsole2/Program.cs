using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaConsumerConsole2
{
    class Program
    {
        static void Main(string[] args)
        {
            var conf = new Dictionary<string, object>
            {
              { "group.id", "consumer" },
              { "bootstrap.servers", "192.168.1.30:9092" },
              { "auto.commit.interval.ms", 5000 },
              { "auto.offset.reset", "earliest" }
            };

            using (var consumer = new Consumer<Null, string>(conf, null, new StringDeserializer(Encoding.UTF8)))
            {
                consumer.OnMessage += (_, msg)
                  => Console.WriteLine($"Read '{msg.Value}' from: {msg.TopicPartitionOffset}");

                consumer.OnError += (_, error)
                  => Console.WriteLine($"Error: {error}");

                consumer.OnConsumeError += (_, msg)
                  => Console.WriteLine($"Consume error ({msg.TopicPartitionOffset}): {msg.Error}");

                consumer.Subscribe(new List<string>() { "XRPUSD-topic", "DSHUSD-topic", "BCHUSD-topic" });

                while (true)
                {
                    consumer.Poll(TimeSpan.FromMilliseconds(100));
                }
            }
        }
    }
}
