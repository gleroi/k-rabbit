using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rabbit.World;

namespace Rabbit.AI
{
    /// <summary>
    /// Choose the best next move, or first
    /// </summary>
    class BasicAi : Ai
    {
        private readonly int Id;

        public BasicAi(int id)
        {
            this.Id = id;
        }

        public Direction Decide(WorldState world)
        {
            var nextStates = GenerateForPlayer(world, this.Id);

            int imax = -1;
            int scoreMax = int.MinValue;
            for (int i = 0; i < nextStates.Length; i++)
            {
                var state = nextStates[i];
                var myPlayer = state.Players[this.Id];
                if (myPlayer.State != PlayerState.Stunned && myPlayer.Score > scoreMax)
                {
                    scoreMax = myPlayer.Score;
                    imax = i;
                }
            }
            return (Direction)imax;
        }


    }
}
