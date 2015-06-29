using System.Collections.Generic;
using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.AI
{
    public class ShortestPathTests
    {
        private static List<Direction> WhenMoving(
            WorldState world, DistanceMap map, Point depart, Point destination, out Point lastPosition,
            int maxIteration = int.MinValue)
        {
            var distance = destination.Dist(depart);

            maxIteration = maxIteration < distance ? distance : maxIteration;

            var directions = new List<Direction>();
            int i = 0;
            lastPosition = depart;
            while (lastPosition != destination && i < maxIteration)
            {
                var dir = map.MoveTo(lastPosition, destination);
                directions.Add(dir.Value);

                var next = lastPosition.Move(dir.Value);
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
         InlineData(4, 5, Direction.O)]
        public void BasicMove_ShouldSucceed(int x, int y, Direction expectedDirection)
        {
            var world = AWorld.GivenWorld()
                .WithPlayer(5, 5);

            var map = new DistanceMap(world, 0);

            var direction = map.MoveTo(new Point(5, 5), new Point(x, y));
            Assert.Equal(expectedDirection, direction);
        }

        [Fact]
        public void LongMoveEastNorth_ShouldSucceed()
        {
            var depart = new Point(5, 5);
            var world = AWorld.GivenWorld()
                .WithPlayer(depart.X, depart.Y);
            var map = new DistanceMap(world, 0);
            
            var destination = new Point(10, 3);
            Point lastPosition;
            List<Direction> directions = WhenMoving(world, map, depart, destination, out lastPosition);

            Assert.Equal(destination, lastPosition);
            Assert.Equal(new[]
            {
                Direction.N, Direction.N,
                Direction.E, Direction.E, Direction.E, Direction.E, Direction.E
            },
                         directions);
        }

        [Fact]
        public void LongMoveWestSouth_ShouldSucceed()
        {
            var depart = new Point(5, 5);
            var world = AWorld.GivenWorld()
                .WithPlayer(depart.X, depart.Y);
            var map = new DistanceMap(world, 0);

            var destination = new Point(0, 7);
            Point lastPosition;
            List<Direction> directions = WhenMoving(world, map, depart, destination, out lastPosition);

            Assert.Equal(destination, lastPosition);
            Assert.Equal(new[]
            {
                Direction.O, Direction.O, Direction.O, Direction.O, Direction.O,
                Direction.S, Direction.S,
            },
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
            var map = new DistanceMap(world, 0);
            map.AddRiskBaffeAtCost(4);

            var destination = new Point(11, 5);
            Point lastPosition;
            List<Direction> directions = WhenMoving(world, map, depart, destination, out lastPosition, 20);

            Assert.Equal(destination, lastPosition);
            Assert.Equal(new[]
            {
                Direction.N, Direction.E,
                Direction.N, Direction.E,
                Direction.N, Direction.E, Direction.E,
                Direction.S, Direction.E,
                Direction.S, Direction.E, 
                Direction.S
            }, directions);
        }
    }
}