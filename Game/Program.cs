using System;
using System.Diagnostics;

namespace Game
{
    internal class Program
    {
        public static int TeamId = 170;
        public static string Secret = "mUrUs2";
        public static int GameId = -1;

        private static void Main(string[] args)
        {
            var manager = new GameManager();

            int GameId = manager.Create(Program.TeamId, Program.Secret);

            Log.Debug("Game ID is : " + GameId);
            Console.WriteLine("Game ID is : " + GameId);

            if (GameId == -1)
            {
                return;
            }

            Console.CancelKeyPress += Program.Console_CancelKeyPress;

            try
            {
                Console.WriteLine("Press any key to start game...");
                Console.ReadKey();

                Process.Start(GameManager.BASE + "?gameId=" + GameId);

                manager.StartGame(GameId, Program.TeamId, Program.Secret);

                Console.WriteLine("Press any key to stop game...");
                Console.ReadKey();

                manager.StopGame(GameId, Program.TeamId, Program.Secret);
            }
            catch (Exception)
            {
                manager.StopGame(GameId, Program.TeamId, Program.Secret);
                throw;
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            var manager = new GameManager();
            manager.StopGame(Program.GameId, Program.TeamId, Program.Secret);
        }
    }
}