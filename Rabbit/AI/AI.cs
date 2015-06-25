using System;
using System.Collections.Generic;
using System.Linq;
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

        protected Ai(int id)
        {
            this.Id = id;
            this.NextAi = () => this;
        }

        public Func<Ai> NextAi { get; protected set; }

        protected WorldState[] GenerateForPlayer(WorldState world, int playerId)
        {
            WorldState[] states = new WorldState[4];

            states[0] = world.ApplyAction(playerId, Direction.N);
            states[1] = world.ApplyAction(playerId, Direction.S);
            states[2] = world.ApplyAction(playerId, Direction.E);
            states[3] = world.ApplyAction(playerId, Direction.O);
            return states;
        }

        internal int FindClosestCompteur(WorldState world)
        {
            var players = world.Players;
            var compteurs = world.Compteurs;
            List<Tuple<int, int>>[] distances = new List<Tuple<int, int>>[compteurs.Count];

            for (var c = 0; c < compteurs.Count; c++)
            {
                var cptDist = new List<Tuple<int, int>>(players.Count);
                var compteur = compteurs[c];

                for (var p = 0; p < players.Count; p++)
                {
                    var player = players[p];
                    var dist = compteur.Pos.Dist(player.Pos);
                    cptDist.Add(new Tuple<int, int>(dist, p));
                }
                cptDist.Sort();
                distances[c] = cptDist;
            }

            int myCpt = -1;
            int minDist = int.MaxValue;

            for (int pos = 0; pos < players.Count; pos++)
            {
                for (var cpt = 0; cpt < distances.Length; cpt++)
                {
                    var cptDist = distances[cpt];
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
            var home = world.Caddies[this.Id].Pos;
            //TODO: utiliser un MoveTo ou on se preoccupe de prendre une baffe
            var direction = this.MoveTo(world, home, 
                state => !state.HasFlag(CellState.Impossible) && !state.HasFlag(CellState.RiskBaffe));
            return direction;
        }

        public Direction GoClosestCompteur(WorldState world)
        {
            var cpt = this.FindClosestCompteur(world);
            //TODO: utiliser un MoveTo ou on ne se preoccupe pas de prendre une baffe
            var direction = this.MoveTo(world, world.Compteurs[cpt].Pos, 
                state => !state.HasFlag(CellState.Impossible));
            return direction;
        }

        public Direction Decide(WorldState world)
        {
            this.Map = new Map(world, this.Id);
            return this.InnerDecide(world);
        }

        protected abstract Direction InnerDecide(WorldState world);
    }
}