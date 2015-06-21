using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.World;
using Xunit;

namespace Rabbit.Tests
{
    static class AWorld
    {
        public static WorldState WithOnePlayerAt(int x, int y)
        {
            var parser = new WorldParser("worldstate::1;joueur1," + x + "," + y + ",0,playing;9,9;joueur1,0,5;");
            var world = parser.Parse();

            Assert.NotNull(world);
            Assert.Equal(1, world.Players.Count);

            var prevPlayer = world.Players[0];
            APlayer.Is(prevPlayer, x, y, 0, PlayerState.Playing);
            return world;
        }
    }
}
