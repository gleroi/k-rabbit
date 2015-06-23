namespace Rabbit.Client
{
    using Rabbit.AI;
    using Rabbit.World;

    class KRabbit
    {
        private readonly BasicAi Ai;
        private readonly int Id;
        private Client Client;

        public KRabbit(int id, BasicAi ai, int gameId)
        {
            this.Id = id;
            this.Ai = ai;
            this.Client = new Client(new Connection(Program.Host, Program.Port, new SocketWrapper()), Program.Secret, gameId, Program.TeamId + id);
        }

        public void Run()
        {
            this.Client.Connect();

            bool gameover = false;
            while (!gameover)
            {
                var msg = this.Client.WaitMessage();

                switch (msg.Type)
                {
                    case MessageType.GameOver:
                    case MessageType.InscriptionKo:
                        gameover = true;
                        break;
                    case MessageType.InscriptionOk:
                        break;
                    case MessageType.WorkState:
                        var parser = new WorldParser(msg.Data);
                        var world = parser.Parse();
                        Log.Write("Player {0} round {1}", this.Id, world.Round);
                        var direction = this.Ai.Decide(world);
                        Log.Write("Player {0} decides {1}", this.Id, direction);
                        this.Client.SendMove(world.Round, direction);
                        break;
                }
            }
        }
    }
}
