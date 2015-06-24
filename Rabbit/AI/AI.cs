using System;
using Rabbit.World;

namespace Rabbit.AI
{
    /// <summary>
    ///     Basic
    /// </summary>
    internal abstract class Ai
    {
        protected readonly int Id;

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

        protected Direction MoveTo(WorldState world, Point cpos)
        {
            var me = world.Players[this.Id].Pos;

            var dir = cpos.X - me.X;
            if (dir == 0)
            {
                dir = cpos.Y - me.Y;
                if (dir > 0)
                {
                    return Direction.S;
                }
                return Direction.N;
            }
            if (dir > 0)
            {
                return Direction.E;
            }
            return Direction.O;
        }

        public abstract Direction Decide(WorldState world);
    }
}