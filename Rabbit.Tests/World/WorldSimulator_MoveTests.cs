using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.World
{
    public class WorldSimulator_MoveTests
    {
        [Theory,
         InlineData(Direction.N, 5, 6),
         InlineData(Direction.E, 6, 7),
         InlineData(Direction.S, 5, 8),
         InlineData(Direction.O, 4, 7)]
        public void BasicMove_ShouldSucceed(Direction direction, int x, int y)
        {
            var world = AWorld.WithOnePlayerAt(5, 7);
            var simulator = new WorldSimulator(world);
            var newWorld = simulator.ApplyAction(0, direction);

            Assert.NotNull(newWorld);
            Assert.Equal(1, newWorld.Players.Count);

            var player = newWorld.Players[0];
            APlayer.Is(player, x, y, 0, PlayerState.Playing);
        }

        [Theory,
         InlineData(0, 0, Direction.N),
         InlineData(0, 0, Direction.O),
         InlineData(WorldState.MAP_WIDTH - 1, 0, Direction.E),
         InlineData(WorldState.MAP_WIDTH - 1, 0, Direction.N),
         InlineData(0, WorldState.MAP_HEIGHT - 1, Direction.S),
         InlineData(0, WorldState.MAP_HEIGHT - 1, Direction.O),
         InlineData(WorldState.MAP_WIDTH - 1, WorldState.MAP_HEIGHT - 1, Direction.E),
         InlineData(WorldState.MAP_WIDTH - 1, WorldState.MAP_HEIGHT - 1, Direction.S)]
        public void MovingOutOfBound_ShouldStunned(int x, int y, Direction direction)
        {
            var world = AWorld.WithOnePlayerAt(x, y);
            var simulator = new WorldSimulator(world);
            var newWorld = simulator.ApplyAction(0, direction);

            APlayer.Is(newWorld.Players[0], x, y, -5, PlayerState.Stunned);
        }

        [Theory,
         InlineData(8, 9, Direction.E),
         InlineData(10, 9, Direction.O),
         InlineData(9, 10, Direction.N),
         InlineData(9, 8, Direction.S)]
        public void MoveOnCompteur_ShouldHasCompteur(int x, int y, Direction direction)
        {
            var world = AWorld.WithOnePlayerAt(x, y);
            Assert.False(world.Players[0].HasCompteur);

            var simulator = new WorldSimulator(world);
            var nworld = simulator.ApplyAction(0, direction);

            APlayer.Is(nworld.Players[0], 9, 9, 1, PlayerState.Playing);
            Assert.True(nworld.Players[0].HasCompteur);
        }

        [Theory,
         InlineData(10, 9, Direction.E),
         InlineData(8, 9, Direction.O),
         InlineData(9, 8, Direction.N),
         InlineData(9, 10, Direction.S)]
        public void MoveWithCompteur_ShouldHasCompteur(int x, int y, Direction direction)
        {
            var world = AWorld.WithOnePlayerAt(9, 9);
            world.Initialize();

            ACompteur.Is(world.Compteurs[0], 9, 9);
            Assert.True(world.Players[0].HasCompteur);

            var simulator = new WorldSimulator(world);
            var nworld = simulator.ApplyAction(0, direction);

            APlayer.Is(nworld.Players[0], x, y, 0, PlayerState.Playing);
            ACompteur.Is(nworld.Compteurs[0], x, y);
            Assert.True(nworld.Players[0].HasCompteur);
        }

        [Fact]
        public void HaveCompteur_ShouldKeepIt()
        {
            var world = AWorld.WithOnePlayerAt(11, 9);
            world.Initialize();

            var simulator = new WorldSimulator(world);

            ACompteur.Is(world.Compteurs[0], 9, 9);
            ACompteur.Is(world.Compteurs[1], 11, 9);
            Assert.Equal(1, world.Players[0].CompteurId);

            world = simulator.ApplyAction(0, Direction.O);

            Assert.Equal(1, world.Players[0].CompteurId);
            ACompteur.Is(world.Compteurs[0], 9, 9);
            ACompteur.Is(world.Compteurs[1], 10, 9);

            world = simulator.ApplyAction(0, Direction.O);

            Assert.Equal(1, world.Players[0].CompteurId);
            ACompteur.Is(world.Compteurs[0], 9, 9);
            ACompteur.Is(world.Compteurs[1], 9, 9);

            world = simulator.ApplyAction(0, Direction.O);

            Assert.Equal(1, world.Players[0].CompteurId);
            ACompteur.Is(world.Compteurs[0], 9, 9);
            ACompteur.Is(world.Compteurs[1], 8, 9);

            world = simulator.ApplyAction(0, Direction.O);

            Assert.Equal(1, world.Players[0].CompteurId);
            ACompteur.Is(world.Compteurs[0], 9, 9);
            ACompteur.Is(world.Compteurs[1], 7, 9);
        }
    }
}