using System;

namespace Rabbit.Client
{
    class ConnectionException : Exception
    {
        public ConnectionException(string message)
            : base(message)
        { }
    }
}
