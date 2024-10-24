using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer2
{
    internal class ProgramReader2
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "DemoQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    Task.Delay(5000).Wait();
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Message Received: ", message);
                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume(queue: "DemoQueue", autoAck: true, consumer: consumer);

                Console.ReadLine();
            }
        }
    }
}
