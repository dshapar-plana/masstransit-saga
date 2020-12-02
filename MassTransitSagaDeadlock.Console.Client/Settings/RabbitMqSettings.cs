namespace MassTransitSagaDeadlock.Console.Client.Settings
{
    public class RabbitMqSettings
    {
        /// <summary>Hostname</summary>
        public string Hostname { get; set; }
        /// <summary>Port</summary>
        public int Port { get; set; }
        /// <summary>Username</summary>
        public string Username { get; set; }
        /// <summary>Password</summary>
        public string Password { get; set; }
        /// <summary>UseSsl</summary>
        public bool UseSsl { get; set; }
        /// <summary>ServerCN</summary>
        public string ServerCN { get; set; }
        /// <summary>IgnoreCertificate</summary>
        public bool IgnoreCertificate { get; set; }
        /// <summary>
        ///     Limit the number of unacknowledged messages for all consumers.
        /// </summary>
        public ushort PrefetchCount { set; get; }
    }
}