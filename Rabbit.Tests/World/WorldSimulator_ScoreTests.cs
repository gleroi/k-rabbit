using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.World
{
    public class WorldSimulator_ScoreTests
    {
        private WorldState GivenWorld(int x, int y, int cx, int cy)
        {
            var world = AWorld.GivenWorld()
                .WithPlayer(x, y)
                .WithCaddy(1, 1)
                .WithCaddy(1, 3)
                .WithCompteur(cx, cy);
            world.Initialize();
            return world;
        }

        [Theory,
         InlineData(10, 9, Direction.E),
         InlineData(8, 9, Direction.O),
         InlineData(9, 8, Direction.N),
         InlineData(9, 10, Direction.S)]
        public void Move_ShouldNotChangeScore
            (int x, int y, Direction direction)
        {
            var world = this.GivenWorld(9, 9, 0, 0);

            APlayer.Is(world.Players[0], 9, 9, 0, PlayerState.Playing);
            ACompteur.Is(world.Compteurs[0], 0, 0);
            Assert.False(world.Players[0].HasCompteur);

            var simulator = new WorldSimulator(world);
            var nworld = simulator.ApplyAction(0, direction);

            APlayer.Is(nworld.Players[0], x, y, 0, PlayerState.Playing);
            ACompteur.Is(nworld.Compteurs[0], 0, 0);
            Assert.False(nworld.Players[0].HasCompteur);
        }

        [Theory,
         InlineData(8, 9, Direction.E),
         InlineData(10, 9, Direction.O),
         InlineData(9, 10, Direction.N),
         InlineData(9, 8, Direction.S)]
        public void MoveOnCompteur_ShouldIncreaseScore(int x, int y, Direction direction)
        {
            var world = this.GivenWorld(x, y, 9, 9);

            APlayer.Is(world.Players[0], x, y, 0, PlayerState.Playing);
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
        public void MoveWithCompteur_ShouldNotChangeScore(int x, int y, Direction direction)
        {
            var world = this.GivenWorld(9, 9, 9, 9);

            APlayer.Is(world.Players[0], 9, 9, 0, PlayerState.Playing);
            ACompteur.Is(world.Compteurs[0], 9, 9);
            Assert.True(world.Players[0].HasCompteur);

            var simulator = new WorldSimulator(world);
            var nworld = simulator.ApplyAction(0, direction);

            APlayer.Is(nworld.Players[0], x, y, 0, PlayerState.Playing);
            ACompteur.Is(nworld.Compteurs[0], x, y);
            Assert.True(nworld.Players[0].HasCompteur);
        }

        [Theory,
         InlineData(1, 2, Direction.S),
         InlineData(0, 3, Direction.E),
         InlineData(1, 4, Direction.N),
         InlineData(2, 3, Direction.O)]
        public void MoveOnOtherCaddyWithCompteur_ShouldNotChangeScore(int x, int y, Direction direction)
        {
            var world = this.GivenWorld(x, y, x, y);

            APlayer.Is(world.Players[0], x, y, 0, PlayerState.Playing);
            Assert.True(world.Players[0].HasCompteur);

            var simulator = new WorldSimulator(world);
            var nworld = simulator.ApplyAction(0, direction);

            APlayer.Is(nworld.Players[0], 1, 3, 0, PlayerState.Playing);
            Assert.True(nworld.Players[0].HasCompteur);
        }
    }
}