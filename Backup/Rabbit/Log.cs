using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Rabbit
{
    static class Log
    {
        public static void Write(string message)
        {
            Debug.WriteLine(message);
        }

        public static void Write(string format, params object[] parameters)
        {
            Debug.WriteLine(String.Format(format, parameters));
        }
    }
}
