using System;
using System.Diagnostics;
using System.Linq;
using Rabbit.AI;
using Rabbit.World;

namespace Rabbit.Client
{
    internal class KRabbit
    {
        private int Id;
        private Ai Ai;
        private readonly Client Client;
        private readonly Random random = new Random();

        public KRabbit(int id, Ai ai, int gameId)
        {
            this.Id = id;
            this.Ai = ai;
            this.Client = new Client(new Connection(Program.Host, Program.Port, new ClientSocketWrapper()), Program.Secret,
                                     gameId, Program.TeamId);
        }

        public void Run()
        {
            this.Client.Connect();

            bool gameover = false;
            foreach (var msg in this.Client.WaitMessages())
            {
                switch (msg.Type)
                {
                    case MessageType.GameOver:
                    case MessageType.InscriptionKo:
                        gameover = true;
                        return;
                    case MessageType.InscriptionOk:
                        break;
                    case MessageType.WorldState:
                        this.HandleWorldState(msg);
                        break;
                }
            }
        }

        private void HandleWorldState(Message msg)
        {
            Stopwatch watch = Stopwatch.StartNew();
            var parser = new WorldParser(msg.Data);
            var world = parser.Parse();
            this.Id = world.Players.FindIndex(p => p.Id == Program.TeamId);
            this.Ai.Id = this.Id;
            Log.Info("Player {0} round {1}", this.Id, world.Round);
            Direction direction;

            try
            {
                direction = this.Ai.Decide(world);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                direction = (Direction) this.random.Next(0, 4);
                Log.Error("Player {0} decides randomly {1}", this.Id, direction);
            }

            watch.Stop();

            this.Client.SendMove(world.Round, direction);

            Log.Info("Player {0} decides {1} in {2}ms", this.Id, direction, watch.ElapsedMilliseconds);
        }
    }
}