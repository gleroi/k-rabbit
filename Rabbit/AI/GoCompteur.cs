using System;
using System.Collections.Generic;
using System.Linq;
using Rabbit.World;

namespace Rabbit.AI
{
    internal class GoCompteur : Ai
    {
        public GoCompteur(int id)
            : base(id) {}

        public override Direction Decide(WorldState world)
        {
            var cpt = this.FindClosestCompteur(world);
            var direction = this.MoveTo(world, world.Compteurs[cpt].Pos);
            var nextPos = world.Players[this.Id].Pos.Move(direction);
            if (world.Compteurs.Any(compteur => compteur.Pos == nextPos))
            {
                this.NextAi = () => new GoHome(this.Id);
            }
            return direction;
        }

        internal int FindClosestCompteur(WorldState world)
        {
            var players = world.Players;
            var compteurs = world.Compteurs;
            List<Tuple<int, int>>[] distances = new List<Tuple<int, int>>[compteurs.Count];

            for (var c = 0; c < compteurs.Count; c++)
            {
                var cptDist = new List<Tuple<int, int>>(players.Count);
                var compteur = compteurs[c];

                for (var p = 0; p < players.Count; p++)
                {
                    var player = players[p];
                    var dist = compteur.Pos.Dist(player.Pos);
                    cptDist.Add(new Tuple<int, int>(dist, p));
                }
                cptDist.Sort();
                distances[c] = cptDist;
            }

            int myCpt = -1;
            int minDist = int.MaxValue;

            for (int pos = 0; pos < players.Count; pos++)
            {
                for (var cpt = 0; cpt < distances.Length; cpt++)
                {
                    var cptDist = distances[cpt];
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