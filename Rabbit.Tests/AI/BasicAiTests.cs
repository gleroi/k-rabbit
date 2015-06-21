using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.AI;
using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.AI
{
    public class BasicAiTests
    {
        [Fact]
        public void Ai_ShouldAlwaysMove()
        {
            var world = AWorld.WithOnePlayerAt(8, 6);

            var ai = new BasicAi(0);

            for (int i = 0; i < 60; i++)
            {
                var prevPops = world.Players[0].Pos;
                var direction = ai.Decide(world);
                world = world.ApplyAction(0, direction);

                Assert.Equal(PlayerState.Playing, world.Players[0].State);
                Assert.True(prevPops != world.Players[0].Pos);
                Assert.Equal(1, prevPops.Dist(world.Players[0].Pos));
            }
        }
    }
}
