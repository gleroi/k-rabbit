using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Rabbit.World;

namespace Rabbit.AI
{
    /// <summary>
    ///     Basic
    /// </summary>
    internal abstract class Ai
    {
        protected readonly int Id;
        protected Map Map;
        protected DistanceMap Distances;

        protected Ai(int id)
        {
            this.Id = id;
        }

        internal int FindClosestRabbit(WorldState world)
        {
            Dictionary<int, List<Tuple<int, int>>> distances =
                new Dictionary<int, List<Tuple<int, int>>>();
            var players = world.Players;
            var compteurs = world.Compteurs;
            for (var p = 0; p < players.Count; p++)
            {
                var player = players[p];
                for (var c = 0; c < compteurs.Count; c++)
                {
                    var compteur = compteurs[c];
                    var dist = compteur.Pos.Dist(player.Pos);
                    List<Tuple<int, int>> cptList;
                    if (distances.TryGetValue(c, out cptList))
                    {
                        cptList.Add(new Tuple<int, int>(dist, p));
                    }
                    else
                    {
                        distances.Add(c, new List<Tuple<int, int>> { new Tuple<int, int>(dist, p) });
                    }
                }
            }

            int myCpt = -1;
            int minDist = int.MaxValue;

            for (int pos = 0; pos < players.Count; pos++)
            {
                foreach (var cpt in distances.Keys)
                {
                    var cptDist = distances[cpt];
                    cptDist.Sort();
                    if (cptDist[pos].Item2 == this.Id && cptDist[pos].Item1 < minDist)
                    {
                        myCpt = cpt;
                        minDist = cptDist[pos].Item1;
                    }
                }
                if (myCpt != -1)
                {
                    break;
                }
            }
            return myCpt;
        }

        internal int FindClosestCompteur(WorldState world)
        {
            var players = world.Players;
            var compteurs = world.Compteurs;
            var distances = this.PlayerCompteurDistances(compteurs, players);

            var myCpt = this.ClosestCompteur(world, distances, this.Id, cpt => !cpt.IsOwned);
            if (myCpt == -1)
            {
                myCpt = this.ClosestCompteur(world, distances, this.Id, cpt => true);
            }
            return myCpt;
        }

        private int ClosestCompteur(WorldState world, List<Tuple<int, int>>[] distances, int playerId,
            Func<Compteur, bool> compteurPredicate)
        {
            int myCpt = -1;
            int minDist = int.MaxValue;
            var len = distances[0].Count;

            for (int pos = 0; pos < len; pos++)
            {
                for (var cpt = 0; cpt < distances.Length; cpt++)
                {
                    var compteur = world.Compteurs[cpt];
                    if (compteurPredicate(compteur))
                    {
                        var cptDist = distances[cpt];
                        if (cptDist[pos].Item2 == playerId && cptDist[pos].Item1 < minDist)
                        {
                            myCpt = cpt;
                            minDist = cptDist[pos].Item1;
                        }
                    }
                }
                if (myCpt != -1)
                {
                    break;
                }
            }
            return myCpt;
        }

        private List<Tuple<int, int>>[] PlayerCompteurDistances(List<Compteur> compteurs, List<Player> players)
        {
            List<Tuple<int, int>>[] distances = new List<Tuple<int, int>>[compteurs.Count];

            for (var c = 0; c < compteurs.Count; c++)
            {
                var cptDist = new List<Tuple<int, int>>(players.Count);
                var compteur = compteurs[c];

                for (var p = 0; p < players.Count; p++)
                {
                    var player = players[p];
                    int dist = 0;
                    if (p == this.Id)
                    {
                        dist = this.Distances.Cost(compteur.Pos);
                    }
                    else
                    {
                        dist = player.Pos.Dist(compteur.Pos);
                    }
                    cptDist.Add(new Tuple<int, int>(dist, p));
                }
                cptDist.Sort();
                distances[c] = cptDist;
            }
            return distances;
        }

        public Direction MoveToByShortestPath(WorldState world, Point cpos)
        {
            var direction = this.Distances.MoveTo(cpos);

            if (!direction.HasValue)
            {
                direction = this.MoveTo(world, cpos, state => !state.HasFlag(CellState.Impossible));
            }
            return direction.GetValueOrDefault(Direction.E);
        }

        public Direction MoveTo(WorldState world, Point cpos, Func<CellState, bool> strategy)
        {
            var me = world.Players[this.Id].Pos;
            var bestmoves = GetMoveInOrder(cpos, me);


            var direction = FirstBest(bestmoves, me, strategy);

            Log.Debug("Player {0} best move is {1}", this.Id, direction);
            
            if (!direction.HasValue)
            {
                Log.Debug("Player {0} has no safe moves", this.Id, String.Join(",", bestmoves));

                direction = FirstBest(bestmoves, me, state => !state.HasFlag(CellState.Impossible));

                Log.Debug("Player {0} second best move is {1}", this.Id, direction);

            }
            var move = direction.GetValueOrDefault(Direction.E);

            Log.Debug("Player {0} move is {1}", this.Id, move);

            return move;
        }

        private Direction? FirstBest(List<Direction> bestmoves, Point me, Func<CellState, bool> predicate)
        {
            foreach (var direction in bestmoves)
            {
                var nextPos = me.Move(direction);
                var state = this.Map.GetCell(nextPos);
                if (predicate(state))
                {
                    return direction;
                }
            }
            return null;
        }

        private List<Direction> GetMoveInOrder(Point cpos, Point me)
        {
            var dirs = new List<Tuple<double, Direction>>();

            for (int i = 0; i < 4; i++)
            {
                var direction = (Direction)i;
                var next = me.Move(direction);
                dirs.Add(new Tuple<double, Direction>(cpos.Dist(next), direction));
            }
            dirs.Sort();

            Log.Debug("Player {0} best moves are {1}", this.Id, String.Join(",", dirs));

            return dirs.Select(t => t.Item2).ToList();
        }

        public Direction GoHome(WorldState world)
        {
            // utiliser un MoveTo ou on se preoccupe de prendre une baffe
            this.Distances = new DistanceMap(world, this.Id, 3);
            this.Distances.BuildAllPath();

            var home = world.Caddies[this.Id].Pos;
            var direction = this.MoveToByShortestPath(world, home);
            return direction;
        }

        public Direction GoClosestCompteur(WorldState world)
        {
            // utiliser un MoveTo ou on ne se preoccupe pas de prendre une baffe
            this.Distances = new DistanceMap(world, this.Id, 2);
            this.Distances.BuildAllPath();

            var cpt = this.FindClosestCompteur(world);
            var direction = this.MoveToByShortestPath(world, world.Compteurs[cpt].Pos);
            return direction;
        }

        public int GetClosestCompteurDist(WorldState world)
        {
            this.Distances = new DistanceMap(world, this.Id, 2);
            this.Distances.BuildAllPath();

            return this.FindClosestCompteur(world);
        }

        public Direction Decide(WorldState world)
        {
            this.Map = new Map(world, this.Id);
            return this.InnerDecide(world);
        }

        protected abstract Direction InnerDecide(WorldState world);
    }
}