using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.World
{
    internal enum PlayerState
    {
        Playing,
        Stunned
    }

    public struct Point
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
                    return new Point(this.X, this.Y - 1);
                case Direction.S:
                    return new Point(this.X, this.Y + 1);
                case Direction.E:
                    return new Point(this.X + 1, this.Y);
                case Direction.O:
                    return new Point(this.X - 1, this.Y);
                default:
                    throw new InvalidOperationException("unknown direction");
            }
        }

        internal int Dist(Point p)
        {
            return Math.Abs(p.X - this.X) + Math.Abs(p.Y - this.Y);
        }
    }

    internal struct Player
    {
        public readonly Point Pos;
        public readonly int Score;
        public readonly PlayerState State;

        public Point Caddy { get; private set; }

        public bool HasCompteur
        {
            get { return this.CompteurId != -1; }
        }

        public int CompteurId { get; private set; }

        public Player(int x, int y, int score, PlayerState state)
            : this()
        {
            this.Pos = new Point(x, y);
            this.Score = score;
            this.State = state;
            this.CompteurId = -1;
        }

        public void UpdateCompteur(int compteur)
        {
            this.CompteurId = compteur;
        }

        public void UpdateCaddy(Point caddy)
        {
            this.Caddy = caddy;
        }
    }

    internal struct Compteur
    {
        public readonly Point Pos;

        public Compteur(int x, int y)
            : this()
        {
            this.Pos = new Point(x, y);
        }
    }

    internal struct Caddy
    {
        public readonly Point Pos;

        public Caddy(int x, int y)
        {
            this.Pos = new Point(x, y);
        }
    }

    [Flags]
    internal enum CellState
    {
        Nothing = 0,
        Player = 2,
        Compteur = 4,
        Caddy = 8
    }

    internal class WorldState
    {
        public int Round { get; }
        public List<Player> Players { get; }
        public List<Compteur> Compteurs { get; }
        public List<Caddy> Caddies { get; }

        public const int MAP_WIDTH = 16;
        public const int MAP_HEIGHT = 13;

        private CellState[] Map = new CellState[MAP_WIDTH*MAP_HEIGHT];

        private WorldState(int round, List<Player> players, List<Compteur> compteurs, List<Caddy> caddies)
        {
            this.Round = round;
            this.Players = players;
            this.Compteurs = compteurs;
            this.Caddies = caddies;
        }

        public WorldState()
        {
            this.Round = 1;
            this.Players = new List<Player>();
            this.Compteurs = new List<Compteur>();
            this.Caddies = new List<Caddy>();
        }

        public static WorldState Create(int round, List<Player> players, List<Compteur> compteurs, List<Caddy> caddies)
        {
            var world = new WorldState(round, players, compteurs, caddies);
            world.Initialize();
            return world;
        }

        private void Initialize()
        {
            int len = this.Players.Count;
            for (int i = 0; i < len; i++)
            {
                var player = this.Players[i];
                var caddy = this.Caddies[i];
                player.UpdateCaddy(caddy.Pos);
                this.CheckCompteur(ref player);
                this.Players[i] = player;
            }
        }

        private void CheckCompteur(ref Player player)
        {
            if (!player.HasCompteur)
            {
                var pos = player.Pos;
                var compteur = this.Compteurs.FindIndex(cp => cp.Pos == pos);
                player.UpdateCompteur(compteur);
            }
        }

        public WorldState ApplyAction(int iplayer, Direction direction)
        {
            Point npos = this.GetNewPos(iplayer, direction);

            var nplayers = new List<Player>(this.Players);
            var ncompteurs = new List<Compteur>(this.Compteurs);

            var player = this.Players[iplayer];

            if (this.IsValidMove(iplayer, npos))
            {
                var nplayer = new Player(npos.X, npos.Y, player.Score, PlayerState.Playing);
                if (player.HasCompteur)
                {
                    ncompteurs[player.CompteurId] = new Compteur(npos.X, npos.Y);
                    nplayer.UpdateCompteur(player.CompteurId);
                }
                else
                {
                    this.CheckCompteur(ref nplayer);
                }
                nplayers[iplayer] = nplayer;
            }
            else
            {
                nplayers[iplayer] = new Player(player.Pos.X, player.Pos.Y, player.Score, PlayerState.Stunned);
            }
            return new WorldState(this.Round, nplayers, ncompteurs, this.Caddies);
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
    }
}