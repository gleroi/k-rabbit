using System;
using System.Collections.Generic;

namespace Rabbit.World
{
    internal class WorldParser
    {
        private int Index;
        private readonly string Data;

        public WorldParser(string data)
        {
            this.Data = data;
        }

        internal char Current
        {
            get { return this.Data[this.Index]; }
        }

        private void Inc()
        {
            this.Index += 1;
        }

        internal char Read(char expected)
        {
            var readed = this.Current;
            if (readed != expected)
            {
                throw new UnexpectedTokenException(expected, readed);
            }
            this.Inc();
            return readed;
        }

        internal char ReadChar()
        {
            var c = this.Current;
            this.Inc();
            return c;
        }

        internal bool IsInt()
        {
            var digit = this.Current;
            return !(digit < '0' || digit > '9');
        }

        internal int ReadDigit()
        {
            var digit = this.Current;
            if (!this.IsInt())
            {
                throw new UnexpectedTokenException("0..9", digit.ToString());
            }

            this.Inc();
            return (digit - '0');
        }

        internal int ReadInt()
        {
            string data = String.Empty;
            while (this.IsInt() || this.Current == '-')
            {
                data += this.Current;
                this.Inc();
            }
            int parsed = Int32.Parse(data);
            return parsed;
        }

        internal string ReadString(string expected)
        {
            int len = expected.Length;
            for (int i = 0; i < len; i++)
            {
                this.Read(expected[i]);
            }
            return expected;
        }

        public WorldState Parse()
        {
            this.ReadString("worldstate::");
            int round = this.ReadInt();
            this.Read(';');
            var players = this.ReadPlayers();
            this.Read(';');
            var compteurs = this.ReadCompteurs();
            this.Read(';');
            var caddies = this.ReadCaddies();
            this.Read(';');

            return WorldState.Create(round, players, compteurs, caddies);
        }

        private List<Player> ReadPlayers()
        {
            var players = new List<Player>(6);
            int i = 0;

            while (this.Current != ';')
            {
                int id = this.ReadInt();
                this.Read(',');
                int x = this.ReadInt();
                this.Read(',');
                int y = this.ReadInt();
                this.Read(',');
                int score = this.ReadInt();
                this.Read(',');
                PlayerState state = this.ReadState();
                players.Add(new Player(i, x, y, score, state));
                i += 1;
                if (this.Current == ':')
                {
                    this.Read(':');
                }
            }

            return players;
        }

        private PlayerState ReadState()
        {
            var data = String.Empty;
            while (this.Current != ';' && this.Current != ':')
            {
                data += this.Current;
                this.Inc();
            }
            var state = (PlayerState) Enum.Parse(typeof (PlayerState), data, true);
            return state;
        }

        private List<Compteur> ReadCompteurs()
        {
            var compteurs = new List<Compteur>(8);
            while (this.Current != ';')
            {
                int x = this.ReadInt();
                this.Read(',');
                int y = this.ReadInt();
                compteurs.Add(new Compteur(x, y));
                if (this.Current == ':')
                {
                    this.Read(':');
                }
            }
            return compteurs;
        }

        private List<Caddy> ReadCaddies()
        {
            var caddies = new List<Caddy>(8);
            while (this.Current != ';')
            {
                int i = this.ReadInt();
                this.Read(',');
                int x = this.ReadInt();
                this.Read(',');
                int y = this.ReadInt();
                caddies.Add(new Caddy(x, y));
                if (this.Current == ':')
                {
                    this.Read(':');
                }
            }
            return caddies;
        }
    }
}
