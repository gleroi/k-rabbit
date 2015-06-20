using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit.World
{
    class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException(string expected, string read)
            : base("Expecting " + expected + ", but read " + read)
        { }

        public UnexpectedTokenException(char expected, char read)
            : base("Expecting " + expected + ", but read " + read)
        { }
    }
}
