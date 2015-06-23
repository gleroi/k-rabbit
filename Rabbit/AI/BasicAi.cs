﻿using System;
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
        public BasicAi(int id) 
            :base(id)
        { }

        private int[] dirs = new int[4] {2, 0, 1, 3};

        public override Direction Decide(WorldState world)
        {
            var nextStates = GenerateForPlayer(world, this.Id);

            int imax = -1;
            int scoreMax = int.MinValue;
            foreach (int i  in this.dirs)
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
