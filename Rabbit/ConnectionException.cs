using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit
{
    class ConnectionException : Exception
    {
        public ConnectionException(string message)
            : base(message)
        { }
    }
}
