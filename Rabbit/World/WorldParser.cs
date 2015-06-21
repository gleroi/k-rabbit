using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit.World
{
    class WorldParser
    {
        int Index = 0;
        string Data;

        public WorldParser(string data)
        {
            this.Data = data;
        }

        internal char Current { get { return this.Data[this.Index]; } }

        void Inc()
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
                throw new UnexpectedTokenException("0..9", digit.ToString());

            this.Inc();
            return (digit - '0');
        }

        internal int ReadInt()
        {
            string data = String.Empty;
            while (IsInt() || this.Current == '-')
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
            ReadString("worldstate::");
            int round = ReadInt();
            Read(';');
            var players = ReadPlayers();
            Read(';');
            var compteurs = ReadCompteurs();
            Read(';');
            var caddies = ReadCaddies();
            Read(';');

            return WorldState.Create(round, players, compteurs, caddies);
        }
        
        private List<Player> ReadPlayers()
        {
            var players = new List<Player>(6);

            while (Current != ';')
            {
                ReadString("joueur");
                int id = ReadInt();
                Read(',');
                int x = ReadInt();
                Read(',');
                int y = ReadInt();
                Read(',');
                int score = ReadInt();
                Read(',');
                PlayerState state = ReadState();
                players.Add(new Player(x, y, score, state));
                if (Current == ':')
                {
                    Read(':');
                }
            }

            return players;
        }

        private PlayerState ReadState()
        {
            var data = String.Empty;
            while (Current != ';' && Current != ':')
            {
                data += this.Current;
                this.Inc();
            }
            var state = (PlayerState)Enum.Parse(typeof(PlayerState), data, true);
            return state;
        }

        private List<Compteur> ReadCompteurs()
        {
            var compteurs = new List<Compteur>(8);
            while (Current != ';')
            {
                int x = ReadInt();
                Read(',');
                int y = ReadInt();
                compteurs.Add(new Compteur(x, y));
                if (Current == ':')
                {
                    Read(':');
                }
            }
            return compteurs;
        }

        private List<Caddy> ReadCaddies()
        {
            var caddies = new List<Caddy>(8);
            while (Current != ';')
            {
                ReadString("joueur");
                int i = ReadInt();
                Read(',');
                int x = ReadInt();
                Read(',');
                int y = ReadInt();
                caddies.Add(new Caddy(x, y));
                if (Current == ':')
                {
                    Read(':');
                }
            }
            return caddies;
        }


    }
}
