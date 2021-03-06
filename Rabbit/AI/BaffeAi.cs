﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.World;

namespace Rabbit.AI
{
    class BaffeAi : Ai
    {
        public BaffeAi(int id)
            : base(id) {}

        protected override Direction InnerDecide(WorldState world)
        {
            return Direction.N;
        }

        private int FindClosestRabbit(WorldState world)
        {
            Dictionary<int, List<Tuple<int, int>>> distances =
                new Dictionary<int, List<Tuple<int, int>>>();
            var players = world.Players;
            var compteurs = world.Compteurs;
            for (var p = 0; p < players.Count; p++)
            {
                var player = players[p];
                for (var c = 0; c < compteurs.Count; c++)
                {
                    var compteur = compteurs[c];
                    var dist = compteur.Pos.Dist(player.Pos);
                    List<Tuple<int, int>> cptList;
                    if (distances.TryGetValue(c, out cptList))
                    {
                        cptList.Add(new Tuple<int, int>(dist, p));
                    }
                    else
                    {
                        distances.Add(c, new List<Tuple<int, int>> { new Tuple<int, int>(dist, p) });
                    }
                }
            }

            int myCpt = -1;
            int minDist = int.MaxValue;

            for (int pos = 0; pos < players.Count; pos++)
            {
                foreach (var cpt in distances.Keys)
                {
                    var cptDist = distances[cpt];
                    cptDist.Sort();
                    if (cptDist[pos].Item2 == this.Id && cptDist[pos].Item1 < minDist)
                    {
                        myCpt = cpt;
                        minDist = cptDist[pos].Item1;
                    }
                }
                if (myCpt != -1)
                {
                    break;
                }
            }
            return myCpt;
        }
    }
}
