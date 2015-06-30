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
        public readonly int Score;

        public StarPoint(int estimation, Point p, int cost, int score)
        {
            this.Estimation = estimation;
            this.Position = p;
            this.Cost = cost;
            this.Score = score;
        }

        public int CompareTo(StarPoint other)
        {
            var dist = this.Estimation.CompareTo(other.Estimation);
            if (dist == 0)
            {
                var s = -this.Score.CompareTo(other.Score);
                if (s == 0)
                {
                    var a = this.Position.X.CompareTo(other.Position.X);
                    if (a == 0)
                    {
                        return this.Position.Y.CompareTo(other.Position.Y);
                    }
                    return a;
                }
                return s;
            }
            return dist;
        }
    }

    internal struct MapCell
    {
        public int Cost;
        public int Score;

        public MapCell(int cost, int score)
        {
            this.Cost = cost;
            this.Score = score;
        }
    }

    internal class DistanceMap
    {
        private const int MAP_HEIGHT = WorldState.MAP_HEIGHT;
        private const int MAP_WIDTH = WorldState.MAP_WIDTH;

        private readonly MapCell[,] Data = new MapCell[DistanceMap.MAP_WIDTH, DistanceMap.MAP_HEIGHT];
        private readonly int Me;

        private readonly WorldState World;
        private Dictionary<Point, StarPoint> cameFrom;
        private readonly Point depart;

        private const int BAFFE_SCORE = 2;
        private const int COMPTEUR_RAMASSE = 1;
        private const int INVALID_MOVE = -5;
        private const int COMPTEUR_HOME = 30;

        public DistanceMap(WorldState world, int me, int baffe, int? lastPlayerAttacked)
        {
            this.World = world;
            this.Me = me;
            var mePt = this.World.Players[this.Me].Pos;
            this.depart = new Point(mePt.X, mePt.Y);
            this.BuildDistanceMap(lastPlayerAttacked);
            this.AddRiskBaffeAtCost(baffe);
        }

        private void BuildDistanceMap(int? lastPlayerAttacked)
        {
            for (int y = 0; y < DistanceMap.MAP_HEIGHT; y++)
            {
                for (int x = 0; x < DistanceMap.MAP_WIDTH; x++)
                {
                    this.Data[x, y] = new MapCell(1, 0);
                }
            }
            for (int p = 0; p < this.World.Players.Count; p++)
            {
                var player = this.World.Players[p].Pos;
                var hasCompteur = this.World.Players[p].HasCompteur;
                var home = this.World.Caddies[p].Pos;
                if (p != this.Me)
                {
                    this.Data[player.X, player.Y] = new MapCell(5000, DistanceMap.INVALID_MOVE);
                    if (player != home)
                    {
                        if (lastPlayerAttacked.HasValue && lastPlayerAttacked.Value != p)
                        {
                            var adjacents = player.Adjacents();
                            foreach (var adj in adjacents)
                            {
                                if (this.IsInBound(adj))
                                {
                                    var cell = this.Data[adj.X, adj.Y];
                                    this.Data[adj.X, adj.Y] = new MapCell(cell.Cost, cell.Score + DistanceMap.BAFFE_SCORE);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (hasCompteur)
                    {
                        var cell = this.Data[home.X, home.Y];
                       this.Data[home.X, home.Y] = new MapCell(cell.Cost, cell.Score + COMPTEUR_HOME);
                    }
                }
            }
            for (int c = 0; c < this.World.Compteurs.Count; c++)
            {
                var cpt = this.World.Compteurs[c].Pos;
                var cell = this.Data[cpt.X, cpt.Y];
                this.Data[cpt.X, cpt.Y] = new MapCell(cell.Cost, cell.Score + COMPTEUR_RAMASSE);
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
                    var arounds = player.Around();
                    foreach (var pos in arounds)
                    {
                        if (this.IsInBound(pos))
                        {
                            var val = this.Data[pos.X, pos.Y];
                            this.Data[pos.X, pos.Y] = new MapCell(val.Cost + cost, val.Score);
                        }
                    }
                }
            }
        }

        private StarPoint CreatePoint(Point p, Point origin, StarPoint prev)
        {
            var cell = this.Data[p.X, p.Y];
            var cost = prev.Cost + cell.Cost;
            var estimation = cost + origin.Dist(p);
            return new StarPoint(estimation, p, cost, prev.Score + cell.Score);
        }

        public void BuildAllPath()
        {
            this.cameFrom = new Dictionary<Point, StarPoint>();
            SortedSet<StarPoint> toBeTreated = new SortedSet<StarPoint>();
            SortedSet<StarPoint> visited = new SortedSet<StarPoint>();
            toBeTreated.Add(this.CreatePoint(this.depart, this.depart, new StarPoint(0, this.depart, 0, 0)));

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
            StarPoint dest;
            int cost = 1024;
            if (this.cameFrom.TryGetValue(destination, out dest))
            {
                cost = dest.Cost;
            }
            else
            {
                Log.Error("Player {0} asks unknown cost from {1} to {2}", this.Me, this.depart, destination);
            }
            return cost;
        }

        public Direction? MoveTo(Point destination)
        {
            Point current = new Point(destination.X, destination.Y);
            Point next = current;
            List<Direction?> directions = new List<Direction?>();
            while (current != this.depart)
            {
                next = current;
                current = this.cameFrom[current].Position;
                directions.Add(DistanceMap.GetDirection(next, current));
            }

            Log.Info("Player {0} going to {1} selected path: {2}, score {3}", this.Me, destination.ToString(),
                     String.Join(",", directions.Select(d => d.ToString()).Reverse()),
                     this.cameFrom[destination].Score);

            return DistanceMap.GetDirection(next, current);
        }

        private static Direction? GetDirection(Point next, Point current)
        {
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

            if (!this.IsInBound(next))
            {
                return;
            }

            var tpl = this.CreatePoint(next, destination, current);
            if (!this.HasBest(visited, tpl))
            {
                this.AddIfBest(toBeTreated, cameFrom, tpl, current);
            }
        }

        private bool IsInBound(Point pt)
        {
            return !(pt.X < 0 || pt.Y < 0
                     || pt.X >= DistanceMap.MAP_WIDTH || pt.Y >= DistanceMap.MAP_HEIGHT);
        }
    }
}