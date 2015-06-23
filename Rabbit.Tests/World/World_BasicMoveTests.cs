using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.World
{
    public class World_BasicMoveTests
    {

        [Fact]
        public void MovingNorth_ShouldSucceed()
        {
            var world = AWorld.WithOnePlayerAt(5, 7);

            var newWorld = world.ApplyAction(0, Direction.N);

            Assert.NotNull(newWorld);
            Assert.Equal(1, newWorld.Players.Count);

            var player = newWorld.Players[0];
            APlayer.Is(player, 5, 8, 0, PlayerState.Playing);
        }

        [Fact]
        public void MovingSouth_ShouldSucceed()
        {
            var world = AWorld.WithOnePlayerAt(5, 7);

            var newWorld = world.ApplyAction(0, Direction.S);

            Assert.NotNull(newWorld);
            Assert.Equal(1, newWorld.Players.Count);

            var player = newWorld.Players[0];
            APlayer.Is(player, 5, 6, 0, PlayerState.Playing);
        }

        [Fact]
        public void MovingEast_ShouldSucceed()
        {
            var world = AWorld.WithOnePlayerAt(5, 7);

            var newWorld = world.ApplyAction(0, Direction.E);

            Assert.NotNull(newWorld);
            Assert.Equal(1, newWorld.Players.Count);

            var player = newWorld.Players[0];
            APlayer.Is(player, 6, 7, 0, PlayerState.Playing);
        }

        [Fact]
        public void MovingWest_ShouldSucceed()
        {
            var world = AWorld.WithOnePlayerAt(5, 7);

            var newWorld = world.ApplyAction(0, Direction.O);

            Assert.NotNull(newWorld);
            Assert.Equal(1, newWorld.Players.Count);

            var player = newWorld.Players[0];
            APlayer.Is(player, 4, 7, 0, PlayerState.Playing);
        }

        [Fact]
        public void OnOrigin_MovingSouth_ShouldStunned()
        {
            var world = AWorld.WithOnePlayerAt(0, 0);

            var newWorld = world.ApplyAction(0, Direction.S);
            APlayer.Is(newWorld.Players[0], 0, 0, 0, PlayerState.Stunned);
        }

        [Fact]
        public void OnOrigin_MovingWest_ShouldStunned()
        {
            var world = AWorld.WithOnePlayerAt(0, 0);

            var newWorld = world.ApplyAction(0, Direction.O);
            APlayer.Is(newWorld.Players[0], 0, 0, 0, PlayerState.Stunned);
        }

        [Fact]
        public void OnExtreme_MovingNorth_ShouldStunned()
        {
            var world = AWorld.WithOnePlayerAt(WorldState.MAP_WIDTH - 1, WorldState.MAP_HEIGHT - 1);

            var newWorld = world.ApplyAction(0, Direction.N);
            APlayer.Is(newWorld.Players[0], WorldState.MAP_WIDTH - 1, WorldState.MAP_HEIGHT - 1, 0, PlayerState.Stunned);
        }

        [Fact]
        public void OnExtreme_MovingEast_ShouldStunned()
        {
            var world = AWorld.WithOnePlayerAt(WorldState.MAP_WIDTH - 1, WorldState.MAP_HEIGHT - 1);

            var newWorld = world.ApplyAction(0, Direction.E);
            APlayer.Is(newWorld.Players[0], WorldState.MAP_WIDTH - 1, WorldState.MAP_HEIGHT - 1, 0, PlayerState.Stunned);
        }
    }
}
