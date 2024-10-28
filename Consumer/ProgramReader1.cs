using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    internal class ProgramReader1
    {
        static void Main(string[] args)
        {
            string userName = "guest";
            string password = "guest";
            string host = "localhost";
            int port = 5672;
            string clientName = "Sincronizador Sucursal #1";

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


                //Codigo para leer mensajes
                //Confiruacion de lectura, solo se procesa un mensaje a la vez
                channel.BasicQos(0, 1, false);
                //Creamos leector del canal
                EventingBasicConsumer consumer = new(channel);
                //Evento que se dispara al recibir un mensaje
                consumer.Received += (model, ea) =>
                {
                    Task.Delay(5000).Wait();
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Message Received: {message}");
                    //Marca mensaje como leido y se elimina de la cola
                    //Si no se manda el ack, el mensaje se vuelve a agregar a la cola
                    channel.BasicAck(ea.DeliveryTag, false);
                };
                //Datos del lector/consumidor
                string consumerTag = channel.BasicConsume(queueName, false, consumer);
                Console.ReadLine();
                //Se cancela la suscripción para evitar que se sigan recibiendo mensajes
                channel.BasicCancel(consumerTag);
            }
        }
    }
}
