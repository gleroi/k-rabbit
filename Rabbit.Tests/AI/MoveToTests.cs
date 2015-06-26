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
    public class MoveToTests
    {

        static bool MoveStrategy(CellState state)
        {
            return !state.HasFlag(CellState.Impossible) && !state.HasFlag(CellState.RiskBaffe);
        } 

        private static List<Direction> WhenMoving(WorldState world, TestAi ai, Point depart, Point destination, out Point lastPosition, int maxIteration = int.MinValue)
        {
            var distance = destination.Dist(depart);

            maxIteration = maxIteration < distance ? distance : maxIteration;

            var directions = new List<Direction>();
            int i = 0;
            lastPosition = depart;
            while (lastPosition != destination && i < maxIteration)
            {
                var dir = ai.MoveTo(world, destination, MoveStrategy);
                directions.Add(dir);

                var next = lastPosition.Move(dir);
                world.Players[0] = new Player(0, next.X, next.Y, 0, PlayerState.Playing);
                lastPosition = next;
                i++;
            }
            return directions;
        }

        [Theory,
        InlineData(5, 4, Direction.N),
        InlineData(5, 6, Direction.S),
        InlineData(6, 5, Direction.E),
        InlineData(4, 5, Direction.O),]
        public void BasicMove_ShouldSucceed(int x, int y, Direction expectedDirection)
        {
            var world = AWorld.GivenWorld()
                .WithPlayer(5, 5);

            var ai = new TestAi(0, world);

            var direction = ai.MoveTo(world, new Point(x, y), MoveStrategy);
            Assert.Equal(expectedDirection, direction);
        }

        [Fact]
        public void LongMoveEastNorth_ShouldSucceed()
        {
            var depart = new Point(5, 5);
            var world = AWorld.GivenWorld()
                .WithPlayer(depart.X, depart.Y);
            var ai = new TestAi(0, world);

            var destination = new Point(10, 3);
            Point lastPosition;
            List<Direction> directions = WhenMoving(world, ai, depart, destination, out lastPosition);

            Assert.Equal(destination, lastPosition);
            Assert.Equal(new Direction[] { 
                Direction.N, Direction.N ,
                Direction.E, Direction.E, Direction.E, Direction.E, Direction.E }, 
                directions);
        }

        [Fact]
        public void LongMoveWestSouth_ShouldSucceed()
        {
            var depart = new Point(5, 5);
            var world = AWorld.GivenWorld()
                .WithPlayer(depart.X, depart.Y);
            var ai = new TestAi(0, world);

            var destination = new Point(0, 7);
            Point lastPosition;
            List<Direction> directions = WhenMoving(world, ai, depart, destination, out lastPosition);

            Assert.Equal(destination, lastPosition);
            Assert.Equal(new Direction[] { 
                Direction.S, Direction.S,
                Direction.O, Direction.O, Direction.O, Direction.O, Direction.O } ,
                directions);
        }

        [Fact]
        public void MoveEast_WithObstacle_ShouldWork()
        {
            var depart = new Point(5, 5);
            var obstacle = new Point(8, 5);
            var world = AWorld.GivenWorld()
                .WithPlayer(depart.X, depart.Y)
                .WithPlayer(obstacle.X, obstacle.Y);
            var ai = new TestAi(0, world);

            var destination = new Point(11, 5);
            Point lastPosition;
            List<Direction> directions = WhenMoving(world, ai, depart, destination, out lastPosition, 20);

            Assert.Equal(destination, lastPosition);
            Assert.Equal(new Direction[] { 
                Direction.N, Direction.E,
                Direction.N, Direction.E,
                Direction.N, Direction.E, Direction.E, 
                Direction.E, Direction.E,
                Direction.S, Direction.S, Direction.S },
                directions);
        }
    }
}
