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

        internal Point[] Around()
        {
            var result = new Point[8];

            result[0] = new Point(this.X + 2, this.Y);
            result[1] = new Point(this.X + 1, this.Y + 1);
            result[2] = new Point(this.X, this.Y + 2);
            result[3] = new Point(this.X - 1, this.Y + 1);
            result[4] = new Point(this.X - 2, this.Y);
            result[5] = new Point(this.X - 1, this.Y - 1);
            result[6] = new Point(this.X, this.Y - 2);
            result[7] = new Point(this.X + 1, this.Y - 1);
            return result;
        }

        internal Point[] Adjacents()
        {
            var result = new Point[4];

            result[0] = new Point(this.X, this.Y - 1);
            result[1] = new Point(this.X, this.Y + 1);
            result[2] = new Point(this.X + 1, this.Y);
            result[3] = new Point(this.X - 1, this.Y);
            return result;
        }
        
        /// <summary>
        /// Manhattan distance
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal int Dist(Point p)
        {
            return Math.Abs(p.X - this.X) + Math.Abs(p.Y - this.Y);
        }

        static int Square(int x) { return x * x; }

        internal double EuclidianDist(Point p)
        {
            return Math.Sqrt(Square(p.X - this.X) + Square(p.Y - this.Y));
        }

        public override string ToString()
        {
            return "(" + this.X + ", " + this.Y + ")";
        }
    }

    internal struct Player
    {
        public readonly Point Pos;
        public int Score { get; private set; }
        public readonly PlayerState State;
        public readonly int Id;

        public Point Caddy { get; private set; }

        public bool HasCompteur
        {
            get { return this.CompteurId != -1; }
        }

        public int CompteurId { get; private set; }

        public Player(int id, int x, int y, int score, PlayerState state)
            : this()
        {
            this.Id = id;
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


        internal void IncreaseScore(int inc)
        {
            this.Score += inc;
        }
    }

    internal struct Compteur
    {
        public readonly Point Pos;
        public int PlayerId { get; private set; }

        public bool IsOwned { get { return this.PlayerId != -1; } }
        public Compteur(int x, int y)
            : this()
        {
            this.Pos = new Point(x, y);
            this.PlayerId = -1;
        }

        public void UpdatePlayer(int id)
        {
            this.PlayerId = id;
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
        Impossible = 2,
        RiskBaffe = 4,
        Baffable = 8,
    }

    internal class WorldState
    {
        public readonly int Round;
        public readonly List<Player> Players;
        public readonly List<Compteur> Compteurs;
        public readonly List<Caddy> Caddies;

        public const int MAP_WIDTH = 16;
        public const int MAP_HEIGHT = 13;

        public int RemainingRounds
        {
            get { return 51 - this.Round; }
        }

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
                var compteurId = this.Compteurs.FindIndex(cp => cp.Pos == pos);
                player.UpdateCompteur(compteurId);
                if (compteurId != -1)
                {
                    var compteur = this.Compteurs[compteurId];
                    compteur.UpdatePlayer(player.Id);
                    this.Compteurs[compteurId] = compteur;
                }
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
                var nplayer = new Player(player.Id, npos.X, npos.Y, player.Score, PlayerState.Playing);
                // compteur
                if (player.HasCompteur)
                {
                    var home = this.Caddies[iplayer];
                    if (nplayer.Pos == home.Pos)
                    {
                        nplayer.IncreaseScore(30);
                        ncompteurs.Remove(ncompteurs[player.CompteurId]);
                        this.CheckCompteur(ref nplayer);
                    }
                    else
                    {
                        ncompteurs[player.CompteurId] = new Compteur(npos.X, npos.Y);
                        nplayer.UpdateCompteur(player.CompteurId);
                    }
                }
                else
                {
                    this.CheckCompteur(ref nplayer);
                    if (nplayer.HasCompteur)
                    {
                        nplayer.IncreaseScore(1);
                    }
                }
                // baffe

                nplayers[iplayer] = nplayer;
            }
            else
            {
                nplayers[iplayer] = new Player(iplayer, player.Pos.X, player.Pos.Y, player.Score - 5, PlayerState.Stunned);
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