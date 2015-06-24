using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.AI;
using Rabbit.World;

namespace Rabbit.Tests
{
    class TestAi : Ai
    {
        public TestAi(int id, WorldState world)
            : base(id)
        {
            this.Map = new Map(world, id);
        }

        public override Direction Decide(Rabbit.World.WorldState world)
        {
            return Direction.N;
        }
    }
}
