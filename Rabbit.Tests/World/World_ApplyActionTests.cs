using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.World
{
    public class World_ApplyActionTests
    {

        private WorldState GivenOnePlayerWorld(int x, int y)
        {
            var parser = new WorldParser("worldstate::1;joueur1," + x + "," + y + ",0,playing;42,51;joueur1,51,42;");
            var world = parser.Parse();

            Assert.NotNull(world);
            Assert.Equal(1, world.Players.Count);

            var prevPlayer = world.Players[0];
            APlayer.Is(prevPlayer, x, y, 0, PlayerState.Playing);
            return world;
        }

        [Fact]
        public void MovingNorth_ShouldSucceed()
        {
            var world = this.GivenOnePlayerWorld(5, 7);

            var newWorld = world.ApplyAction(0, Direction.N);

            Assert.NotNull(newWorld);
            Assert.Equal(1, newWorld.Players.Count);

            var player = newWorld.Players[0];
            APlayer.Is(player, 5, 8, 0, PlayerState.Playing);
        }

        [Fact]
        public void MovingSouth_ShouldSucceed()
        {
            var world = this.GivenOnePlayerWorld(5, 7);

            var newWorld = world.ApplyAction(0, Direction.S);

            Assert.NotNull(newWorld);
            Assert.Equal(1, newWorld.Players.Count);

            var player = newWorld.Players[0];
            APlayer.Is(player, 5, 6, 0, PlayerState.Playing);
        }

        [Fact]
        public void MovingEast_ShouldSucceed()
        {
            var world = this.GivenOnePlayerWorld(5, 7);

            var newWorld = world.ApplyAction(0, Direction.E);

            Assert.NotNull(newWorld);
            Assert.Equal(1, newWorld.Players.Count);

            var player = newWorld.Players[0];
            APlayer.Is(player, 6, 7, 0, PlayerState.Playing);
        }

        [Fact]
        public void MovingWest_ShouldSucceed()
        {
            var world = this.GivenOnePlayerWorld(5, 7);

            var newWorld = world.ApplyAction(0, Direction.O);

            Assert.NotNull(newWorld);
            Assert.Equal(1, newWorld.Players.Count);

            var player = newWorld.Players[0];
            APlayer.Is(player, 4, 7, 0, PlayerState.Playing);
        }

        [Fact]
        public void OnOrigin_MovingSouth_ShouldStunned()
        {
            var world = this.GivenOnePlayerWorld(0, 0);

            var newWorld = world.ApplyAction(0, Direction.S);
            APlayer.Is(newWorld.Players[0], 0, 0, 0, PlayerState.Stunned);
        }

        [Fact]
        public void OnOrigin_MovingWest_ShouldStunned()
        {
            var world = this.GivenOnePlayerWorld(0, 0);

            var newWorld = world.ApplyAction(0, Direction.O);
            APlayer.Is(newWorld.Players[0], 0, 0, 0, PlayerState.Stunned);
        }

        [Fact]
        public void OnExtreme_MovingNorth_ShouldStunned()
        {
            var world = this.GivenOnePlayerWorld(WorldState.MAP_WIDTH - 1, WorldState.MAP_HEIGHT - 1);

            var newWorld = world.ApplyAction(0, Direction.N);
            APlayer.Is(newWorld.Players[0], WorldState.MAP_WIDTH - 1, WorldState.MAP_HEIGHT - 1, 0, PlayerState.Stunned);
        }

        [Fact]
        public void OnExtreme_MovingEast_ShouldStunned()
        {
            var world = this.GivenOnePlayerWorld(WorldState.MAP_WIDTH - 1, WorldState.MAP_HEIGHT - 1);

            var newWorld = world.ApplyAction(0, Direction.E);
            APlayer.Is(newWorld.Players[0], WorldState.MAP_WIDTH - 1, WorldState.MAP_HEIGHT - 1, 0, PlayerState.Stunned);
        }
    }
}
