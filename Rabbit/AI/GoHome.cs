using System.Linq;
using Rabbit.World;

namespace Rabbit.AI
{
    internal class GoHome : Ai
    {
        public GoHome(int id)
            : base(id) {}

        protected override Direction InnerDecide(WorldState world)
        {
            return this.GoHome(world);
        }
    }
}