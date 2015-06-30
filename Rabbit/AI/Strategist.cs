using System;
using System.Linq;
using Rabbit.World;

namespace Rabbit.AI
{
    internal class Strategist : Ai
    {
        public Strategist(int id)
            : base(id) {}

        protected override Direction InnerDecide(WorldState world)
        {
            var decide = SelectTactic(world);
            return decide(world);
        }

        private Func<WorldState, Direction> SelectTactic(WorldState world)
        {
            var me = world.Players[this.Id];
            var cpt = this.GetClosestCompteurDist(world);

            if (me.HasCompteur && me.Pos.Dist(me.Caddy) > world.RemainingRounds || 
                !me.HasCompteur && me.Pos.Dist(me.Caddy) + (me.Pos.Dist(world.Compteurs[cpt].Pos) * 2) > world.RemainingRounds)
            {
                //choisir entre chopper un compteur pour 1 point ou baffer un lapinou
                //penser au bouclier des lapinous baffés
            }

            if (!me.HasCompteur)
            {
                return this.GoClosestCompteur;
            }
            else
            {
                return this.GoHome;
            }
            //var home = world.Caddies[this.Id].Pos;
            //if (me.Pos == home)
            //{
            //    return this.GoClosestCompteur;
            //}
        }
    }
}