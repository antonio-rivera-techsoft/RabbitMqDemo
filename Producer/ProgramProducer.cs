using System.Text;
using RabbitMQ.Client;

namespace Producer
{
    internal class ProgramProducer
    {
        static void Main(string[] args)
        {
            //Valores default de RabbitMq en ambiente local
            //Los valores verdaderos se deben extraer de AppSettings
            string userName = "guest";
            string password = "guest";
            string host = "localhost";
            int port = 5672;
            string clientName = "Procesador";

            //Opciones para configurar conexión
            //Opcion 1
            ConnectionFactory factory = new()
            {
                UserName = userName,
                Password = password,
                HostName = host,
                Port = port,
                ClientProvidedName = clientName
            };

            //Opcion 2
            //ConnectionFactory factory = new();
            //factory.Uri = new Uri($"amqp://{userName}:{password}@{host}:{port}");
            //factory.ClientProvidedName = clientName;

            //El exchange es el padre de las colas
            //Un exchange puede tener varias colas
            string exchangeName = "DemoExchange";

            //El exchange usa el rountingKey para
            //enrutar mensajes a una cola específica
            string routingKey = "routing-key";

            //Se usa al momento de crear una cola y
            //para bindear la cola al exchange por el key
            string queueName = "DemoQueue";

            //Creación de conexión y canal
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //La siguiente declaracion y bindeo se puede hacer en codigo pero deberi hacerse por RabbitMq management
                //Declaración de exchange y cola
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false);
                channel.QueueDeclare(queueName, true, false, false, null);
                //Se bindea la cola al exchange mediante la routingKey
                channel.QueueBind(queueName, exchangeName, routingKey, null);

                for (int i = 0; i < 60; i++)
                {
                    //Codificación del mensaje, mandaremos dtos serializados
                    var body = Encoding.UTF8.GetBytes($"Message #{i}");
                    //Mandamos el mensaje a un exchange diciendole a que cola mediante el routing key
                    channel.BasicPublish(exchangeName, routingKey, null, body);
                    Console.WriteLine($"Sending Message: {i}");
                    Thread.Sleep(1000);
                }
            }
            Console.ReadLine();
        }
    }
}
