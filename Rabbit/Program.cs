﻿using System;
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

        public static int GameId = -1;

        static void Main(string[] args)
        {
            if (args.Length != 2 && args.Length != 0)
            {
                Console.WriteLine("usage: rabbit.exe <host> <port>");
            }

            var manager = new GameManager();

            GameId = manager.Create(TeamId, Secret);

            Log.Write("Game ID is : " + GameId);

            if (GameId == -1)
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
                            var rabbit = new KRabbit(subId, new BasicAi(subId), GameId);
                            rabbit.Run();
                        });
                task.Start();
                rabbits[i] = task;
            }

            Console.CancelKeyPress += Console_CancelKeyPress;

            try
            {
                manager.StartGame(GameId, TeamId, Secret);
                Task.WaitAll(rabbits);
            }
            catch (Exception)
            {
                manager.StopGame(GameId, TeamId, Secret);
                throw;
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            var manager = new GameManager();
            manager.StopGame(GameId, TeamId, Secret);
        }
    }
}
