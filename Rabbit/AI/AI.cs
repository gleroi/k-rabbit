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

        protected Direction MoveTo(WorldState world, Point cpos)
        {
            var me = world.Players[this.Id].Pos;
            var bestmoves = GetMoveInOrder(cpos, me);

            foreach (var direction in bestmoves)
            {
                var nextPos = me.Move(direction);
                var state = this.Map.GetCell(nextPos);
                if (!state.HasFlag(CellState.Impossible) && !state.HasFlag(CellState.RiskBaffe))
                {
                    return direction;
                }
            }
            var dir = bestmoves.FirstOrDefault(d =>
            {
                var nextPos = me.Move(d);
                var state = this.Map.GetCell(nextPos);
                return !state.HasFlag(CellState.Impossible);
            });
            return dir;
        }

        private static List<Direction> GetMoveInOrder(Point cpos, Point me)
        {
            List<Direction> goods = new List<Direction>();
            var bads = new List<Direction>();

            var horizontal = cpos.X - me.X;
            if (horizontal > 0)
            {
                goods.Add(Direction.E);
                bads.Add(Direction.O);
            }
            else if (horizontal < 0)
            {
                goods.Add(Direction.O);
                bads.Add(Direction.E);
            }
            else
            {
                bads.Add(Direction.E);
                bads.Add(Direction.O);
            }

            var vertical = cpos.Y - me.Y;
            if (vertical > 0)
            {
                goods.Add(Direction.N);
                bads.Add(Direction.S);
            }
            else if (vertical < 0)
            {
                goods.Add(Direction.S);
                bads.Add(Direction.N);
            }
            else
            {
                bads.Add(Direction.S);
                bads.Add(Direction.N);
            }

            goods.AddRange(bads);
            return goods;
        }

        public Direction GoHome(WorldState world)
        {
            var home = world.Caddies[this.Id].Pos;
            var direction = this.MoveTo(world, home);
            return direction;
        }

        public Direction GoClosestCompteur(WorldState world)
        {
            var cpt = this.FindClosestCompteur(world);
            var direction = this.MoveTo(world, world.Compteurs[cpt].Pos);
            return direction;
        }

        public abstract Direction Decide(WorldState world);
    }
}