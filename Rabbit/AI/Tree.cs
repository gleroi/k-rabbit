using System;

using Rabbit.World;

namespace Rabbit.AI
{
    class Tree
    {
        private int[] PlayerScores;
        private int CurrentPlayer;

        Tree North;
        Tree South;
        Tree West;
        Tree East;

        public int MetaScore(int iplayer)
        {
            if (this.North == null) 
            {
                var score = this.PlayerScores[iplayer];
                return score;
            }
            if (iplayer == this.CurrentPlayer)
            {
                return Math.Max(this.North.MetaScore(iplayer),
                       Math.Max(this.South.MetaScore(iplayer),
                       Math.Max(this.East.MetaScore(iplayer), this.West.MetaScore(iplayer))));
            }
            else
            {
                return Math.Min(this.North.MetaScore(iplayer),
                       Math.Min(this.South.MetaScore(iplayer),
                       Math.Min(this.East.MetaScore(iplayer), this.West.MetaScore(iplayer))));
            }
        }

        public static Tree Generate(WorldState state, int iplayer, int depth = 5)
        {
            var nbPlayers = state.Players.Count;
            Tree tree = new Tree()
            {
                PlayerScores = new int[nbPlayers],
                CurrentPlayer = iplayer
            };

            for (int i = 0; i < nbPlayers; i++)
            {
                tree.PlayerScores[i] = state.Players[i].Score;
            }

            var player = state.Players[iplayer];

            if (depth > 0 && player.State == PlayerState.Playing)
            {
                var nextPlayer = (iplayer + 1) % state.Players.Count;
                tree.North = Generate(state.ApplyAction(iplayer, Direction.N), nextPlayer, depth - 1);
                tree.South = Generate(state.ApplyAction(iplayer, Direction.S), nextPlayer, depth - 1);
                tree.East = Generate(state.ApplyAction(iplayer, Direction.E), nextPlayer, depth - 1);
                tree.West = Generate(state.ApplyAction(iplayer, Direction.O), nextPlayer, depth - 1);
            }
            return tree;
        }
    }
}
