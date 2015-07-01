using System.Collections.Generic;
using System.Linq;

namespace Rabbit.World
{
    internal class WorldSimulator
    {
        private WorldState World;

        public WorldSimulator(WorldState world)
        {
            this.World = world;
        }

        public WorldState ApplyAction(int iplayer, Direction direction)
        {
            Point npos = this.GetNewPos(iplayer, direction);

            var nplayers = new List<Player>(this.World.Players);
            var ncompteurs = new List<Compteur>(this.World.Compteurs);

            var player = this.World.Players[iplayer];

            if (this.IsValidMove(iplayer, npos))
            {
                var nplayer = new Player(player.Id, npos.X, npos.Y, player.Score, PlayerState.Playing);
                // compteur
                if (player.HasCompteur)
                {
                    var home = this.World.Caddies[iplayer];
                    if (nplayer.Pos == home.Pos)
                    {
                        nplayer.IncreaseScore(30);
                        ncompteurs.Remove(ncompteurs[player.CompteurId]);
                        this.World.CheckCompteur(ref nplayer);
                    }
                    else
                    {
                        ncompteurs[player.CompteurId] = new Compteur(npos.X, npos.Y);
                        nplayer.UpdateCompteur(player.CompteurId);
                    }
                }
                else
                {
                    this.World.CheckCompteur(ref nplayer);
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
                nplayers[iplayer] = new Player(iplayer, player.Pos.X, player.Pos.Y, player.Score - 5,
                                               PlayerState.Stunned);
            }
            this.World = WorldState.Create(this.World.Round, nplayers, ncompteurs, this.World.Caddies);
            return this.World;
        }

        private Point GetNewPos(int player, Direction direction)
        {
            var cpos = this.World.Players[player].Pos;
            return cpos.Move(direction);
        }

        private bool IsValidMove(int player, Point npos)
        {
            if (npos.X < 0 || npos.Y < 0 || npos.X >= WorldState.MAP_WIDTH || npos.Y >= WorldState.MAP_HEIGHT)
            {
                return false;
            }
            return this.World.Players.All(p => p.Pos != npos);
        }
    }
}