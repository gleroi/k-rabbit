using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Client
{
    using Rabbit.AI;
    using Rabbit.World;

    class KRabbit
    {
        private readonly int Id;
        private Ai Ai;
        private Client Client;
        private Random random = new Random();

        public KRabbit(int id, Ai ai, int gameId)
        {
            this.Id = id;
            this.Ai = ai;
            this.Client = new Client(new Connection(Program.Host, Program.Port, new SocketWrapper()), Program.Secret, gameId, Program.TeamId + id);
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
                    case MessageType.WorkState:

                        var parser = new WorldParser(msg.Data);
                        var world = parser.Parse();
                        Log.Write("Player {0} round {1}", this.Id, world.Round);
                        Direction direction;

                        try
                        {
                            direction = this.Ai.Decide(world);
                        }
                        catch (Exception e)
                        {
                            direction = (Direction) this.random.Next(0, 4);
                        }
                        finally
                        {
                            this.Ai = this.Ai.NextAi();
                        }

                        Log.Write("Player {0} decides {1}", this.Id, direction);
                        this.Client.SendMove(world.Round, direction);
                        break;
                }
            }
        }
    }
}
