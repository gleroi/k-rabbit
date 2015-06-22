using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit
{
    using System.Threading.Tasks;

    using Rabbit.AI;
    using Rabbit.Client;

    class Program
    {
        static public int TeamId = 170;
        static public string Secret = "mUrUs2";

        public static int Port = 2026;

        public static string Host = "battle.gate.vm.gate.erdf.fr";

        static void Main(string[] args)
        {
            if (args.Length != 2 && args.Length != 0)
            {
                Console.WriteLine("usage: rabbit.exe <host> <port>");
            }

            var manager = new GameManager();

            var gameId = manager.Create(TeamId, Secret);

            Log.Write("Game ID is : " + gameId);

            if (gameId == -1)
            {
                return;
            }

            var rabbits = new Task[6];
            for (int i = 0; i < 6; i++)
            {
                var subId = i;
                Task task = new Task(
                    () =>
                        {
                            var rabbit = new KRabbit(subId, new BasicAi(subId), gameId);
                            rabbit.Run();
                        });
                task.Start();
                rabbits[i] = task;
            }

            try
            {
                manager.StartGame(gameId, TeamId, Secret);
                Task.WaitAll(rabbits);
            }
            catch (Exception)
            {
                manager.StopGame(gameId, TeamId, Secret);
                throw;
            }
        }
    }
}
