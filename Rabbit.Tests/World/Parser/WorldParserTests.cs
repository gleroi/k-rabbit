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

//        worldstate::3;170,1,1,-4,playing:171,1,3,-4,playing:172,1,5,-4,playing:173,1,7,-4,playing:174,1,9,-4,playing:175,1,11,-4,playing;13,3:11,2:15,1:11,12:9,1:14,5:12,5:14,1:12,7:15,0;170,1,1:171,1,3:172,1,5:173,1,7:174,1,9:175,1,11;\n
//worldstate::4;170,1,1,-6,playing:171,1,3,-6,playing:172,1,5,-6,playing:173,1,7,-6,playing:174,1,9,-6,playing:175,1,11,-6,playing;13,3:11,2:15,1:11,12:9,1:14,5:12,5:14,1:12,7:15,0;170,1,1:171,1,3:172,1,5:173,1,7:174,1,9:175,1,11;\n
//worldstate::5;170,1,1,-8,playing:171,1,3,-8,playing:172,1,5,-8,playing:173,1,7,-8,playing:174,1,9,-8,playing:175,1,11,-8,playing;13,3:11,2:15,1:11,12:9,1:14,5:12,5:14,1:12,7:15,0;170,1,1:171,1,3:172,1,5:173,1,7:174,1,9:175,1,11;\n
//worldstate::6;170,1,1,-10,playing:171,1,3,-10,playing:172,1,5,-10,playing:173,1,7,-10,playing:174,1,9,-10,playing:175,1,11,-10,playing;13,3:11,2:15,1:11,12:9,1:14,5:12,5:14,1:12,7:15,0;170,1,1:171,1,3:172,1,5:173,1,7:174,1,9:175,1,11;\n



        [Fact]
        public void ValidWorld_ShouldSucceed()
        {
            var msg = "worldstate::25;joueur1,12,15,2,playing:joueur2,21,51,-3,stunned;" +
                "12,15:42,64;joueur1,5023,-8963:joueur2,12,15;";
            var parser = new WorldParser(msg);
            var world = parser.Parse();

            Assert.NotNull(world);
            Assert.Equal(25, world.Round);

            Assert.Equal(2, world.Players.Count);
            Assert.Equal(2, world.Compteurs.Count);
            Assert.Equal(2, world.Caddies.Count);

            var firstPlayer = world.Players[0];
            APlayer.Is(firstPlayer, 12, 15, 2, PlayerState.Playing);

            var secondPlayer = world.Players[1];
            APlayer.Is(secondPlayer, 21, 51, -3, PlayerState.Stunned);

            IsCompteur(world.Compteurs[0], 12, 15);
            IsCompteur(world.Compteurs[1], 42, 64);

            IsCaddy(world.Caddies[0], 5023, -8963);
            IsCaddy(world.Caddies[1], 12, 15);

            Assert.True(firstPlayer.HasCompteur);
            Assert.False(secondPlayer.HasCompteur);
        }

        private void IsCaddy(Caddy caddy, int x, int y)
        {
            Assert.Equal(x, caddy.Pos.X);
            Assert.Equal(y, caddy.Pos.Y);
        }

        private void IsCompteur(Compteur compteur, int x, int y)
        {
            Assert.Equal(x, compteur.Pos.X);
            Assert.Equal(y, compteur.Pos.Y);
        }



    }
}
