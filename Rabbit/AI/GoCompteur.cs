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
            return this.GoClosestCompteur(world);
        }
    }
}