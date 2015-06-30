using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.World;

namespace Rabbit.AI
{
    class CompteurHomeAi : Ai
    {
        public CompteurHomeAi(int id)
            : base(id) {}


        protected override Direction InnerDecide(WorldState world)
        {
            var me = world.Players[this.Id];
            if (!me.HasCompteur)
            {
                return this.GoClosestCompteur(world);
            }
            else
            {
                return this.GoHome(world);
            }
        }
    }
}
