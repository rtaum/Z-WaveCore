namespace ZWaveCore.Core.Exceptions
{
    public class ReponseFormatException : ResponseException
    {
        public ReponseFormatException() : base("The response was not in the expected format") { }
        public ReponseFormatException(string message) : base(message) { }
        public ReponseFormatException(string message, System.Exception inner) : base(message, inner) { }
    }
}
