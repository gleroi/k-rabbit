using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    class Program
    {
        public static int TeamId = 170;
        public static string Secret = "mUrUs2";
        public static int GameId = -1;

        static void Main(string[] args)
        {
            var manager = new GameManager();

            int GameId = manager.Create(TeamId, Secret);

            Log.Debug("Game ID is : " + GameId);
            Console.WriteLine("Game ID is : " + GameId);

            if (GameId == -1)
            {
                return;
            }

            Console.CancelKeyPress += Console_CancelKeyPress;

            try
            {
                Console.WriteLine("Press any key to start game...");
                Console.ReadKey();

                Process.Start(GameManager.BASE + "?gameId=" + GameId);

                manager.StartGame(GameId, TeamId, Secret);

                Console.WriteLine("Press any key to stop game...");
                Console.ReadKey();

                manager.StopGame(GameId, TeamId, Secret);
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
