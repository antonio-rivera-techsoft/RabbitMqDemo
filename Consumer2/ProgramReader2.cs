using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer2
{
    internal class ProgramReader2
    {
        static void Main(string[] args)
        {
            string userName = "guest";
            string password = "guest";
            string host = "localhost";
            int port = 5672;
            string clientName = "Sincronizador Sucursal #2";

            ConnectionFactory factory = new()
            {
                UserName = userName,
                Password = password,
                HostName = host,
                Port = port,
                ClientProvidedName = clientName
            };

            string exchangeName = "DemoExchange";
            string routingKey = "routing-key";
            string queueName = "DemoQueue";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false);
                channel.QueueDeclare(queueName, true, false, false, null);
                channel.QueueBind(queueName, exchangeName, routingKey, null);

                channel.BasicQos(0, 1, false);
                EventingBasicConsumer consumer = new(channel);
                consumer.Received += (model, ea) =>
                {
                    try 
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"Message Received: {message}");
                        Task.Delay(3000).Wait();

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {

                    }
                };
                string consumerTag = channel.BasicConsume(queueName, false, consumer);
                Console.ReadLine();
                channel.BasicCancel(consumerTag);
            }
        }
    }
}
