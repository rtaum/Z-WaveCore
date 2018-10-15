namespace ZWaveCore.Core.Exceptions
{
    public class TransmissionException : CommunicationException
    {
        public TransmissionException() : base("Transmission failure.") { }
        public TransmissionException(string message) : base(message) { }
        public TransmissionException(string message, System.Exception inner) : base(message, inner) { }
    }
}
