namespace BuildingBlocks.Messaging.RabbiMQ
{
    public class RabbitMqOptions
    {
        public const string SectionName = "RabbitMq";

        public string Host { get; set; } = default!;

        public int Port { get; set; } = 5672;

        public string UserName { get; set; } = "guest";

        public string Password { get; set; } = "guest";
    }
}
