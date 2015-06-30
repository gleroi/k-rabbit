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
            var cpt = this.FindClosestFreeCompteur(world);

            if (cpt == -1 ||
                me.HasCompteur && me.Pos.Dist(me.Caddy) > world.RemainingRounds || // caddy trop loin 
                !me.HasCompteur && me.Pos.Dist(world.Compteurs[cpt].Pos) + me.Caddy.Dist(world.Compteurs[cpt].Pos) > world.RemainingRounds) // compteur trop loin
            {
                int closestPlayer = this.FindClosestRabbit(world);
                return (aWorld) => this.GoBaffePlayer(aWorld, closestPlayer);
            }

            if (!me.HasCompteur)
            {
                return this.GoClosestCompteur;
            }
            else
            {
                return this.GoHome;
            }
        }
    }
}