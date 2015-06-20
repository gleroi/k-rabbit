using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.World.Parser
{
    public class WorldParserTests
    {
        [Fact]
        public void ValidWorld_ShouldSucceed()
        {
            var msg = "worldstate::25;joueur1,12,15,2,playing:joueur2,21,51,-3,stunned;" +
                "5,1:42,64;joueur1,5023,-8963:joueur1,12,15;";
            var parser = new WorldParser(msg);
            var world = parser.Parse();

            Assert.NotNull(world);
            Assert.Equal(25, world.Round);

            Assert.Equal(2, world.Players.Count);
            Assert.Equal(2, world.Compteurs.Count);
            Assert.Equal(2, world.Caddies.Count);

            var firstPlayer = world.Players[0];
            IsPlayer(firstPlayer, 12, 15, 2, PlayerState.Playing);

            var secondPlayer = world.Players[1];
            IsPlayer(secondPlayer, 21, 51, -3, PlayerState.Stunned);

            IsCompteur(world.Compteurs[0], 5, 1);
            IsCompteur(world.Compteurs[1], 42, 64);

            IsCaddy(world.Caddies[0], 5023, -8963);
            IsCaddy(world.Caddies[1], 12, 15);

        }

        private void IsCaddy(Caddy caddy, int x, int y)
        {
            Assert.Equal(x, caddy.Pos.X);
            Assert.Equal(y, caddy.Pos.Y);
        }

        private void IsCompteur(Compteur compteur, int x, int y)
        {
            Assert.Equal(x, compteur.X);
            Assert.Equal(y, compteur.Y);
        }

        private static void IsPlayer(Player player, int x, int y, int score, PlayerState state)
        {
            Assert.Equal(x, player.X);
            Assert.Equal(y, player.Y);
            Assert.Equal(score, player.Score);
            Assert.Equal(state, player.State);
        }

    }
}
