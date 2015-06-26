using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Rabbit.World
{
    internal class DistanceMap
    {
        private const int MAP_HEIGHT = WorldState.MAP_HEIGHT + 2;
        private const int MAP_WIDTH = WorldState.MAP_WIDTH + 2;

        private readonly int[,] Data = new int[DistanceMap.MAP_WIDTH, DistanceMap.MAP_HEIGHT];
        private readonly int Me;
        private readonly Point Origin;

        private readonly WorldState World;

        public DistanceMap(WorldState world, int me)
        {
            this.World = world;
            this.Me = me;
            var pt = this.World.Players[this.Me].Pos;
            this.Origin = new Point(pt.X + 1, pt.Y + 1);
            this.BuildDistanceMap();
        }

        private void BuildDistanceMap()
        {
            for (int x = 0; x < DistanceMap.MAP_WIDTH; x++)
            {
                this.Data[x, 0] = int.MaxValue;
                this.Data[x, DistanceMap.MAP_HEIGHT - 1] = int.MaxValue;
            }
            for (int y = 0; y < DistanceMap.MAP_HEIGHT; y++)
            {
                this.Data[0, y] = int.MaxValue;
                this.Data[DistanceMap.MAP_WIDTH - 1, y] = int.MaxValue;
            }

            for (int y = 0; y < DistanceMap.MAP_HEIGHT - 1; y++)
            {
                for (int x = 0; x < DistanceMap.MAP_WIDTH - 1; x++)
                {
                    this.Data[x, y] = 1;
                }
            }
        }

        public void AddRiskBaffeAtCost(int cost)
        {
            var len = this.World.Players.Count;
            for (int p = 0; p < len; p++)
            {
                if (p != this.Me)
                {
                    var player = this.World.Players[p].Pos;
                    this.Data[player.X + 1, player.Y + 1] = int.MaxValue;
                    var arounds = Map.PointsAround(player);
                    foreach (var pos in arounds)
                    {
                        var val = this.Data[pos.X, pos.Y];
                        if (val < int.MaxValue)
                        {
                            this.Data[pos.X, pos.Y] = val + cost;
                        }
                    }
                }
            }
        }

        void BuildMoveMap()
        {
            Queue<Point> toBeTreated = new Queue<Point>();
            toBeTreated.Enqueue(this.Origin);
            int prev = 0;
            while (toBeTreated.Count > 0)
            {
                var pt = toBeTreated.Peek();

                for (int i = 0)

                toBeTreated.Dequeue();
            }
        }

        private Tuple<int, Point> Tpl(Point p, int prev)
        {
            return new Tuple<int, Point>(prev + this.Data[p.X, p.Y] + this.Origin.Dist(p), p);
        }

        public Direction? MoveTo(Point to)
        {
            var destination = new Point(to.X + 1, to.Y + 1);

            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
            SortedSet<Tuple<int, Point>> toBeTreated = new SortedSet<Tuple<int, Point>>();
            SortedSet<Tuple<int, Point>> visited = new SortedSet<Tuple<int, Point>>();
            toBeTreated.Add(this.Tpl(destination, 0));

            while (toBeTreated.Count > 0)
            {
                var node = toBeTreated.First();
                toBeTreated.Remove(node);
                if (node.Item2 == this.Origin)
                {
                    return ReconstructPath(cameFrom, this.Origin);
                }

                this.AddNext(toBeTreated, visited, cameFrom, node, Direction.N);
                this.AddNext(toBeTreated, visited, cameFrom, node, Direction.E);
                this.AddNext(toBeTreated, visited, cameFrom, node, Direction.S);
                this.AddNext(toBeTreated, visited, cameFrom, node, Direction.O);
            }
            return null;
        }

        private Direction? ReconstructPath(Dictionary<Point, Point> cameFrom, Point origin)
        {
            return Direction.E;
        }

        private bool HasBest(SortedSet<Tuple<int, Point>> visited, Tuple<int, Point> node)
        {
            return visited.Any(n => n.Item2 == node.Item2 && n.Item1 <= node.Item1);
        }

        private void AddIfBest(SortedSet<Tuple<int, Point>> visited, Dictionary<Point, Point> cameFrom, Tuple<int, Point> node, Point source)
        {
            if (!this.HasBest(visited, node))
            {
                cameFrom[node.Item2] = source;
                visited.Add(node);
            }
        }

        private void AddNext(SortedSet<Tuple<int, Point>> toBeTreated, SortedSet<Tuple<int, Point>> visited, Dictionary<Point, Point> cameFrom, Tuple<int, Point> node, Direction dir)
        {
            var pt = node.Item2.Move(dir);
            var tpl = this.Tpl(pt, node.Item1);
            if (!visited.Contains(tpl))
            {
                this.AddIfBest(toBeTreated, cameFrom, tpl, node.Item2);
            }
        }
    }
}