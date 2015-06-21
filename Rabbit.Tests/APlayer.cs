using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.World;
using Xunit;

namespace Rabbit.Tests
{
    static class APlayer
    {
        public static void Is(Player player, int x, int y, int score, PlayerState state)
        {
            Assert.Equal(x, player.Pos.X);
            Assert.Equal(y, player.Pos.Y);
            Assert.Equal(score, player.Score);
            Assert.Equal(state, player.State);
        }
    }
}
