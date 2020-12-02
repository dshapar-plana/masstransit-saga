namespace MassTransitSagaDeadlock.Console.Client.Settings
{
    public class MessagingSettings
    {
        /// <summary>
        ///     The name of the parent service that consume EAPIMessaging framework services.
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        ///     Timeout to start a message bus action before throws the exception.
        /// </summary>
        public int StartTimeoutInSec { get; set; }
        /// <summary>Request timeout before throws the exception.</summary>
        public int RequestTimeoutInSec { get; set; }
        /// <summary>
        ///     Limits the number of concurrent messages consumed by the handler.
        /// </summary>
        public int ConcurrentMessageLimit { get; set; }
        /// <summary>Gets or sets RabbitMq settings.</summary>
        public RabbitMqSettings RabbitMq { get; set; }
    }
}