namespace MassTransitSagaDeadlock.Worker.Auxiliary
{
    public class Error
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the friendly message.
        /// </summary>
        /// <value>
        /// The friendly message.
        /// </value>
        public string FriendlyMessage { get; set; }
    }
}