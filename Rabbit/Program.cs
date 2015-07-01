using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using Rabbit.AI;
using Rabbit.Client;

namespace Rabbit
{
    internal class Program
    {
        public static int TeamId = 170;
        public static string Secret = "mUrUs2";

        public static int Port; //= 2026;

        public static string Host; // = "battle.gate.vm.gate.erdf.fr";


        private static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("usage: rabbit.exe <host> <port> <gameId>");
                return;
            }

            Program.Host = args[0];
            Program.Port = int.Parse(args[1]);
            int GameId = int.Parse(args[2]);

            ConfigLog();

            const int MAX_RABBITS = 1;
            var rabbits = new Task[MAX_RABBITS];
            for (int i = 0; i < MAX_RABBITS; i++)
            {
                var subId = i;
                Task task = new Task(
                    () =>
                    {
                        Ai ai = new Strategist(subId);
                        var rabbit = new KRabbit(subId, ai, GameId);
                        rabbit.Run();
                    });
                task.Start();
                rabbits[i] = task;
            }

            Task.WaitAll(rabbits);
        }

        private static void ConfigLog()
        {
            var config = new LoggingConfiguration();

            var console = new ColoredConsoleTarget();
            config.AddTarget("console", console);
            var cRule = new LoggingRule("*", LogLevel.Info, console);
            config.LoggingRules.Add(cRule);

            var file = new FileTarget();
            file.FileName = "${basedir}/${shortdate}_rabbit.log";
            config.AddTarget("file", file);
            var fRule = new LoggingRule("*", LogLevel.Debug, file);
            config.LoggingRules.Add(fRule);

            LogManager.Configuration = config;
        }


    }
}