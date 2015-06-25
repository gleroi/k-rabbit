using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.World
{
    public class MapTests
    {
        [Fact]
        public void EmptyWorld_GiveEmptyMap()
        {
            var world = AWorld.GivenWorld()
                .WithPlayer(1,1);
            var map = new Map(world, 0);

            for (int x = 0; x < WorldState.MAP_WIDTH; x++)
            {
                for (int y = 0; y < WorldState.MAP_HEIGHT; y++)
                {
                    var state = map.GetCell(new Point(x, y));
                    Assert.Equal(CellState.Nothing, state);
                }
            }
        }

        [Theory,
        InlineData(-1,-1),
        InlineData(WorldState.MAP_WIDTH, -1),
        InlineData(WorldState.MAP_WIDTH, WorldState.MAP_HEIGHT),
        InlineData(-1, WorldState.MAP_HEIGHT)]
        public void CellOutsideGrid_ShouldBeImpossible(int x, int y)
        {
            var world = AWorld.GivenWorld()
                .WithPlayer(1, 1);
            var map = new Map(world, 0);

            var state = map.GetCell(new Point(x, y));
            Assert.Equal(CellState.Impossible, state);
        }

        [Fact]
        public void CellWithRabbits_ShouldBeImpossible()
        {
            var world = AWorld.GivenWorld()
                .WithPlayer(1, 1)
                .WithPlayer(5, 5)
                .WithPlayer(7, 8)
                .WithPlayer(15, 12);
            var map = new Map(world, 0);

            Assert.Equal(CellState.Impossible, map.GetCell(new Point(5, 5)));
            Assert.Equal(CellState.Impossible, map.GetCell(new Point(7, 8)));
            Assert.Equal(CellState.Impossible, map.GetCell(new Point(15, 12)));
        }

        [Fact]
        public void CellAroundRabbit_ShouldBeRiskBaffe()
        {
            var world = AWorld.GivenWorld()
                .WithPlayer(1, 1)
                .WithPlayer(5, 5);
            var map = new Map(world, 0);

            Assert.Equal(CellState.RiskBaffe, map.GetCell(new Point(7, 5)));
            Assert.Equal(CellState.RiskBaffe, map.GetCell(new Point(6, 6)));
            Assert.Equal(CellState.RiskBaffe, map.GetCell(new Point(5, 7)));
            Assert.Equal(CellState.RiskBaffe, map.GetCell(new Point(4, 6)));
            Assert.Equal(CellState.RiskBaffe, map.GetCell(new Point(3, 5)));
            Assert.Equal(CellState.RiskBaffe, map.GetCell(new Point(4, 4)));
            Assert.Equal(CellState.RiskBaffe, map.GetCell(new Point(5, 3)));
            Assert.Equal(CellState.RiskBaffe, map.GetCell(new Point(6, 4)));
        }
    }
}