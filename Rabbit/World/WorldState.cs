using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit.World
{
    enum PlayerState
    {
        Playing,
        Stunned,
    }

    struct Point
    {
        public readonly int X;
        public readonly int Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }

        internal Point Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.N:
                    return new Point(this.X, this.Y + 1);
                case Direction.S:
                    return new Point(this.X, this.Y - 1);
                case Direction.E:
                    return new Point(this.X + 1, this.Y);
                case Direction.O:
                    return new Point(this.X - 1, this.Y);
                default:
                    throw new InvalidOperationException("unknown direction");
            }
        }
    }

    struct Player
    {
        public readonly Point Pos;
        public readonly int Score;
        public readonly PlayerState State;

        public Point Caddy { get; private set; }
        public bool HasCompteur { get; private set; }

        public Player(int x , int y, int score, PlayerState state)
            : this()
        {
            this.Pos = new Point(x, y);
            this.Score = score;
            this.State = state;
        }

        public void Update(Point caddy, bool hasCompteur)
        {
            this.Caddy = caddy;
            this.HasCompteur = hasCompteur;
        }
    }

    struct Compteur
    {
        public readonly Point Pos;

        public Compteur(int x , int y)
            : this()
        {
            this.Pos = new Point(x, y);
        }
    }

    struct Caddy
    {
        public readonly Point Pos;

        public Caddy(int x, int y)
        {
            this.Pos = new Point(x, y);
        }
    }

    [Flags]
    enum CellState
    {
        Nothing = 0,
        Player = 2,
        Compteur = 4,
        Caddy = 8,
    }

    class WorldState : ICloneable
    {
        public int Round { get; private set; }
        public List<Player> Players { get; private set; }
        public List<Compteur> Compteurs { get; private set; }
        public List<Caddy> Caddies { get; private set; }

        public const int MAP_WIDTH = 16;
        public const int MAP_HEIGHT = 12;

        private CellState[] Map = new CellState[MAP_WIDTH * MAP_HEIGHT];

        public WorldState(int round, List<Player> players, List<Compteur> compteurs, List<Caddy> caddies)
        {
            this.Round = round;
            this.Players = players;
            this.Compteurs = compteurs;
            this.Caddies = caddies;

            Initialize();
        }

        private void Initialize()
        {
            int len = this.Players.Count;
            for (int i = 0; i < len; i++)
            {
                var player = this.Players[i];
                var caddy = this.Caddies[i];
                var hasCompteur = this.Compteurs.Any(cp => cp.Pos == player.Pos);
                player.Update(caddy.Pos, hasCompteur);
                this.Players[i] = player;
            }
        }

        public WorldState ApplyAction(int iplayer, Direction direction)
        {
            var world = this.Clone() as WorldState;

            Point npos = GetNewPos(iplayer, direction);

            if (world.IsValidMove(iplayer, npos))
            {
                world.ApplyMove(iplayer, npos);
            }
            else
            {
                var player = world.Players[iplayer];
                world.Players[iplayer] = new Player(player.Pos.X, player.Pos.Y, player.Score, PlayerState.Stunned);
            }
            return world;
        }

        private Point GetNewPos(int player, Direction direction)
        {
            var cpos = this.Players[player].Pos;
            return cpos.Move(direction);
        }

        private bool IsValidMove(int player, Point npos)
        {
            if (npos.X < 0 || npos.Y < 0 || npos.X >= MAP_WIDTH || npos.Y >= MAP_HEIGHT)
            {
                return false;
            }
            return this.Players.All(p => p.Pos != npos);
        }

        private void ApplyMove(int player, Point npos)
        {
            var p = this.Players[player];
            this.Players[player] = new Player(npos.X, npos.Y, p.Score, PlayerState.Playing);
        }

        #region ICloneable Members

        public object Clone()
        {
            return new WorldState(this.Round, this.Players.ToList(), this.Compteurs.ToList(), this.Caddies.ToList());
        }

        #endregion
    }
}
