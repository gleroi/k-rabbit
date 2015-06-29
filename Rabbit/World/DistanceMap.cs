using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.World
{
    internal struct StarPoint : IComparable<StarPoint>
    {
        public readonly int Estimation;
        public readonly Point Position;
        public readonly int Cost;

        public StarPoint(int estimation, Point p, int cost)
        {
            this.Estimation = estimation;
            this.Position = p;
            this.Cost = cost;
        }

        public int CompareTo(StarPoint other)
        {
            var dist = this.Estimation.CompareTo(other.Estimation);
            if (dist == 0)
            {
                var a = this.Position.X.CompareTo(other.Position.X);
                if (a == 0)
                {
                    return this.Position.Y.CompareTo(other.Position.Y);
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
        private Dictionary<Point, StarPoint> cameFrom;
        private readonly Point depart;

        public DistanceMap(WorldState world, int me)
        {
            this.World = world;
            this.Me = me;
            var mePt = this.World.Players[this.Me].Pos;
            this.depart = new Point(mePt.X + 1, mePt.Y + 1);
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

        private StarPoint CreatePoint(Point p, Point origin, StarPoint prev)
        {
            var cost = prev.Cost + this.Data[p.X, p.Y];
            var estimation = cost + origin.Dist(p);
            return new StarPoint(estimation, p, cost);
        }

        public void BuildAllPath()
        {
            this.cameFrom = new Dictionary<Point, StarPoint>();
            SortedSet<StarPoint> toBeTreated = new SortedSet<StarPoint>();
            SortedSet<StarPoint> visited = new SortedSet<StarPoint>();
            toBeTreated.Add(this.CreatePoint(this.depart, this.depart, new StarPoint(0, this.depart, 0)));

            while (toBeTreated.Count > 0)
            {
                var node = toBeTreated.First();
                toBeTreated.Remove(node);
                visited.Add(node);

                this.AddNext(toBeTreated, visited, this.cameFrom, node, this.depart, Direction.N);
                this.AddNext(toBeTreated, visited, this.cameFrom, node, this.depart, Direction.E);
                this.AddNext(toBeTreated, visited, this.cameFrom, node, this.depart, Direction.S);
                this.AddNext(toBeTreated, visited, this.cameFrom, node, this.depart, Direction.O);
            }
        }

        public int Cost(Point destination)
        {
            Point current = new Point(destination.X + 1, destination.Y + 1);
            return this.cameFrom[current].Cost;
        }

        public Direction? MoveTo(Point destination)
        {
            Point current = new Point(destination.X + 1, destination.Y + 1);
            Point next = current;
            while (current != this.depart)
            {
                next = current;
                current = this.cameFrom[current].Position;
            }

            if (next.X - current.X > 0)
            {
                return Direction.E;
            }
            if (next.X - current.X < 0)
            {
                return Direction.O;
            }
            if (next.Y - current.Y > 0)
            {
                return Direction.S;
            }
            if (next.Y - current.Y < 0)
            {
                return Direction.N;
            }
            return null;
        }

        private bool HasBest(SortedSet<StarPoint> set, StarPoint node)
        {
            var currentDist = node.Cost;
            return set.Any(n => n.Position == node.Position && n.Cost <= currentDist);
        }

        private void AddIfBest(
            SortedSet<StarPoint> set, Dictionary<Point, StarPoint> cameFrom,
            StarPoint node, StarPoint source)
        {
            if (!this.HasBest(set, node))
            {
                cameFrom[node.Position] = source;
                set.Add(node);
            }
        }

        private void AddNext(
            SortedSet<StarPoint> toBeTreated, SortedSet<StarPoint> visited,
            Dictionary<Point, StarPoint> cameFrom,
            StarPoint current, Point destination, Direction dir)
        {
            var next = current.Position.Move(dir);

            if (!DistanceMap.IsInBound(next) || this.Data[next.X, next.Y] == int.MaxValue)
            {
                return;
            }

            var tpl = this.CreatePoint(next, destination, current);
            if (!this.HasBest(visited, tpl))
            {
                this.AddIfBest(toBeTreated, cameFrom, tpl, current);
            }
        }

        private static bool IsInBound(Point pt)
        {
            return !(pt.X < 0 || pt.Y < 0 || pt.X >= DistanceMap.MAP_WIDTH || pt.Y >= DistanceMap.MAP_HEIGHT);
        }
    }
}