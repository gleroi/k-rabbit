using System;
using System.Linq;
using Rabbit.World;

namespace Rabbit.AI
{
    internal class Strategist : Ai
    {
        public Strategist(int id)
            : base(id) {}

        public override Direction Decide(WorldState world)
        {
            this.Map = new Map(world, this.Id);

            var decide = SelectTactic(world);
            return decide(world);
        }

        private Func<WorldState, Direction> SelectTactic(WorldState world)
        {
            var me = world.Players[this.Id];
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