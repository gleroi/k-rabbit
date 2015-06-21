using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rabbit.World;

namespace Rabbit.AI
{
    class Tree
    {
        WorldState State;

        Tree North;
        Tree South;
        Tree West;
        Tree East;

        public int MetaScore(int iplayer)
        {
            if (this.North == null) 
            {
                var player = this.State.Players[iplayer];
                var score = player.Score;
                if (player.HasCompteur)
                {
                    score += OWN_COMPTEUR_BONUS;
                }
                return score;
            }
            return Math.Max(this.North.MetaScore(iplayer), 
                Math.Max(this.South.MetaScore(iplayer),
                Math.Max(this.East.MetaScore(iplayer), this.West.MetaScore(iplayer))));
        }

        public static Tree Generate(WorldState state, int iplayer, int depth = 5)
        {
            Tree tree = new Tree()
            {
                State = state,
            };

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

        const int OWN_COMPTEUR_BONUS = 15;
    }
}
