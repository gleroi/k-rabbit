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
    }

    struct Player
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Score;
        public readonly PlayerState State;

        public Player(int x , int y, int score, PlayerState state)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Score = score;
            this.State = state;
        }
    }

    struct Compteur
    {
        public readonly int X;
        public readonly int Y;

        public Compteur(int x , int y)
            : this()
        {
            this.X = x;
            this.Y = y;
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

    class WorldState
    {
        public int Round { get; private set; }
        public List<Player> Players { get; set; }
        public List<Compteur> Compteurs { get; private set; }
        public List<Caddy> Caddies;

        public WorldState(int round, List<Player> players, List<Compteur> compteurs, List<Caddy> caddies)
        {
            this.Round = round;
            this.Players = players;
            this.Compteurs = compteurs;
            this.Caddies = caddies;
        }

    }
}
