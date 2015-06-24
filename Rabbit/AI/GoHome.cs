using System.Linq;
using Rabbit.World;

namespace Rabbit.AI
{
    internal class GoHome : Ai
    {
        public GoHome(int id)
            : base(id) {}

        public override Direction Decide(WorldState world)
        {
            var me = world.Players[this.Id];
            if (!world.Compteurs.Any(compteur => compteur.Pos == me.Pos))
            {
                this.NextAi = () => new GoCompteur(this.Id);
                return this.NextAi().Decide(world);
            }

            var home = world.Caddies[this.Id].Pos;
            var direction = this.MoveTo(world, home);
            var nextPos = world.Players[this.Id].Pos.Move(direction);

            if (nextPos == home)
            {
                this.NextAi = () => new GoCompteur(this.Id);
            }
            return direction;
        }
    }
}