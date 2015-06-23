using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NLog;

namespace Rabbit
{
    static class Log
    {
        private static Logger _instance = null;
        private static Logger Instance
        {
            get
            {
                return _instance ?? (_instance = LogManager.GetLogger("Default"));
            }
        }

        public static void Write(string message)
        {
            Debug.WriteLine(message);
            Instance.Info(message);
        }

        public static void Write(string format, params object[] parameters)
        {
            Debug.WriteLine(format, parameters);
            Instance.Info(format, parameters);
        }
    }
}
