using System.Text;
using RabbitMQ.Client;

namespace Producer
{
    internal class ProgramProducer
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            string exchangeName = "DemoExchange";
            string routingKey = "routing-key";
            string queueName = "DemoQueue";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
                channel.QueueDeclare(queueName, false, false, false, null);
                channel.QueueBind(queueName, exchangeName, routingKey, null);

                for (int i = 0; i < 60; i++)
                {
                    var body = Encoding.UTF8.GetBytes($"Sending Message: {i}");
                    channel.BasicPublish(exchange: "DemoExchange", routingKey: "routing-key", basicProperties: null, body: body);
                    Console.WriteLine($"Sending Message: {i}");
                    Thread.Sleep(1000);
                }
            }

            Console.ReadLine();
        }
    }
}
