using System;
using System.Diagnostics;
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

        public static int Port = 2026;

        public static string Host = "battle.gate.vm.gate.erdf.fr";

        public static int GameId = -1;

        private static void Main(string[] args)
        {
            if (args.Length != 2 && args.Length != 0)
            {
                Console.WriteLine("usage: rabbit.exe <host> <port>");
            }

            ConfigLog();

            var manager = new GameManager();

            GameId = manager.Create(TeamId, Secret);

            Log.Debug("Game ID is : " + GameId);

            if (GameId == -1)
            {
                return;
            }
            const int MAX_RABBITS = 6;
            var rabbits = new Task[MAX_RABBITS];
            for (int i = 0; i < MAX_RABBITS; i++)
            {
                var subId = i;
                Task task = new Task(
                    () =>
                    {
                        var rabbit = new KRabbit(subId, new GoCompteurAi(subId), GameId);
                        rabbit.Run();
                    });
                task.Start();
                rabbits[i] = task;
            }

            Console.CancelKeyPress += Console_CancelKeyPress;

            try
            {
                Process.Start(GameManager.BASE + "?gameId=" + GameId);

                manager.StartGame(GameId, TeamId, Secret);
                Task.WaitAll(rabbits);
            }
            catch (Exception)
            {
                manager.StopGame(GameId, TeamId, Secret);
                throw;
            }
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

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            var manager = new GameManager();
            manager.StopGame(GameId, TeamId, Secret);
        }
    }
}