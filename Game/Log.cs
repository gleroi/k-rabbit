using NLog;

namespace Game
{
    internal static class Log
    {
        private static Logger _instance;

        private static Logger Instance
        {
            get { return _instance ?? (_instance = LogManager.GetLogger("Default")); }
        }

        public static void Debug(string message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
            Instance.Debug(message);
#endif
        }

        public static void Debug(string format, params object[] parameters)
        {

#if DEBUG
            System.Diagnostics.Debug.WriteLine(format, parameters);
            Instance.Debug(format, parameters);
#endif
        }

        public static void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            Instance.Info(message);
        }

        public static void Info(string format, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine(format, parameters);
            Instance.Info(format, parameters);
        }

        public static void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            Instance.Error(message);
        }

        public static void Error(string format, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine(format, parameters);
            Instance.Error(format, parameters);
        }
    }
}