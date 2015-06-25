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

        protected override Direction InnerDecide(WorldState world)
        {
            return this.GoClosestCompteur(world);
        }
    }
}