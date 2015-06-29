using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.World
{
    internal class DistancePointComparer : IComparer<Tuple<int, Point>>
    {
        public int Compare(Tuple<int, Point> x, Tuple<int, Point> y)
        {
            var dist = x.Item1.CompareTo(y.Item1);
            if (dist == 0)
            {
                var a = x.Item2.X.CompareTo(y.Item2.X);
                if (a == 0)
                {
                    return x.Item2.Y.CompareTo(y.Item2.Y);
                }
                return a;
            }
            return dist;
        }
    }

    internal class DistanceMap
    {
        private const int MAP_HEIGHT = WorldState.MAP_HEIGHT + 2;
        private const int MAP_WIDTH = WorldState.MAP_WIDTH + 2;

        private readonly int[,] Data = new int[DistanceMap.MAP_WIDTH, DistanceMap.MAP_HEIGHT];
        private readonly int Me;

        private readonly WorldState World;

        public DistanceMap(WorldState world, int me)
        {
            this.World = world;
            this.Me = me;
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

            for (int y = 1; y < DistanceMap.MAP_HEIGHT - 1; y++)
            {
                for (int x = 1; x < DistanceMap.MAP_WIDTH - 1; x++)
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
                        if (DistanceMap.IsInBound(pos))
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
        }

        private Tuple<int, Point> Tpl(Point p, Point origin, Tuple<int, Point> prev)
        {
            var prevDist = prev.Item1 - origin.Dist(prev.Item2);
            return new Tuple<int, Point>(prevDist + this.Data[p.X, p.Y] + origin.Dist(p), p);
        }

        public Direction? MoveTo(Point from, Point to)
        {
            var depart = new Point(from.X + 1, from.Y + 1);
            var destination = new Point(to.X + 1, to.Y + 1);

            var comparer = new DistancePointComparer();
            Dictionary<Point, Tuple<int, Point>> cameFrom = new Dictionary<Point, Tuple<int, Point>>();
            SortedSet<Tuple<int, Point>> toBeTreated = new SortedSet<Tuple<int, Point>>(comparer);
            SortedSet<Tuple<int, Point>> visited = new SortedSet<Tuple<int, Point>>(comparer);
            toBeTreated.Add(this.Tpl(destination, destination, new Tuple<int, Point>(0, destination)));

            while (toBeTreated.Count > 0)
            {
                var node = toBeTreated.First();
                toBeTreated.Remove(node);
                visited.Add(node);

                if (node.Item2 == depart)
                {
                    return this.ReconstructPath(cameFrom, depart, destination);
                }

                this.AddNext(toBeTreated, visited, cameFrom, node, destination, Direction.N);
                this.AddNext(toBeTreated, visited, cameFrom, node, destination, Direction.E);
                this.AddNext(toBeTreated, visited, cameFrom, node, destination, Direction.S);
                this.AddNext(toBeTreated, visited, cameFrom, node, destination, Direction.O);
            }
            return null;
        }

        private Direction? ReconstructPath(
            Dictionary<Point, Tuple<int, Point>> cameFrom, Point depart, Point destination)
        {
            Point current = depart;
            Point prev = cameFrom[depart].Item2;

            if (prev.X - current.X > 0)
            {
                return Direction.E;
            }
            if (prev.X - current.X < 0)
            {
                return Direction.O;
            }
            if (prev.Y - current.Y > 0)
            {
                return Direction.S;
            }
            if (prev.Y - current.Y < 0)
            {
                return Direction.N;
            }
            return null;
        }

        private bool HasBest(SortedSet<Tuple<int, Point>> visited, Tuple<int, Point> node, Point destination)
        {
            var currentDist = node.Item1 - destination.Dist(node.Item2);
            return visited.Any(n => n.Item2 == node.Item2
                                    && (node.Item1 - destination.Dist(node.Item2)) <= currentDist);
        }

        private void AddIfBest(
            SortedSet<Tuple<int, Point>> set, Dictionary<Point, Tuple<int, Point>> cameFrom,
            Tuple<int, Point> node, Tuple<int, Point> source, Point destination)
        {
            if (!this.HasBest(set, node, destination))
            {
                cameFrom[node.Item2] = source;
                set.Add(node);
            }
        }

        private void AddNext(
            SortedSet<Tuple<int, Point>> toBeTreated, SortedSet<Tuple<int, Point>> visited,
            Dictionary<Point, Tuple<int, Point>> cameFrom,
            Tuple<int, Point> node, Point destination, Direction dir)
        {
            var pt = node.Item2.Move(dir);

            if (!DistanceMap.IsInBound(pt) || this.Data[pt.X, pt.Y] == int.MaxValue)
            {
                return;
            }

            var tpl = this.Tpl(pt, destination, node);
            if (!this.HasBest(visited, tpl, destination))
            {
                this.AddIfBest(toBeTreated, cameFrom, tpl, node, destination);
            }
        }

        private static bool IsInBound(Point pt)
        {
            return !(pt.X < 0 || pt.Y < 0 || pt.X >= DistanceMap.MAP_WIDTH || pt.Y >= DistanceMap.MAP_HEIGHT);
        }
    }
}