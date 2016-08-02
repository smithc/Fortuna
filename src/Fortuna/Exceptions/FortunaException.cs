using System;

namespace Fortuna.Exceptions
{
    public class FortunaException : Exception
    {

        public FortunaException(string message) : base(message)
        {
        }

        public FortunaException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
