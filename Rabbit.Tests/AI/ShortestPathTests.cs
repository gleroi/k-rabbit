using System.Collections.Generic;
using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.AI
{
    public class ShortestPathTests
    {
        private static List<Direction> WhenMoving(
            WorldState world, Point depart, Point destination, out Point lastPosition, int cost = 0,
            int maxIteration = int.MinValue)
        {
            var distance = destination.Dist(depart);

            maxIteration = maxIteration < distance ? distance : maxIteration;

            var directions = new List<Direction>();
            int i = 0;
            lastPosition = depart;
            while (lastPosition != destination && i < maxIteration)
            {
                var map = new DistanceMap(world, 0, cost, null);
                map.BuildAllPath();
                var dir = map.MoveTo(destination);
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

            Point lastPosition;
            List<Direction> directions = WhenMoving(world, new Point(5,5), new Point(x, y), out lastPosition);
            Assert.Equal(new List<Direction> {expectedDirection}, directions);
        }

        [Fact]
        public void LongMoveEastNorth_ShouldSucceed()
        {
            var depart = new Point(5, 5);
            var world = AWorld.GivenWorld()
                .WithPlayer(depart.X, depart.Y);
            
            var destination = new Point(10, 3);
            Point lastPosition;
            List<Direction> directions = WhenMoving(world, depart, destination, out lastPosition);

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

            var destination = new Point(0, 7);
            Point lastPosition;
            List<Direction> directions = WhenMoving(world, depart, destination, out lastPosition);

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

            var destination = new Point(11, 5);
            Point lastPosition;
            List<Direction> directions = WhenMoving(world, depart, destination, out lastPosition, 3, 20);

            Assert.Equal(destination, lastPosition);
            Assert.Equal(new[]
            {
                Direction.N, Direction.N, Direction.N, 
                Direction.E, Direction.E, Direction.E, Direction.E,
                Direction.S, Direction.E,
                Direction.S, Direction.E, 
                Direction.S
            }, directions);
        }
    }
}