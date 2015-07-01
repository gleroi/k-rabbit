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
            WorldState newWorld = WorldState.Create(this.World.Round, nplayers, ncompteurs, this.World.Caddies);

            var player = this.World.Players[iplayer];
            if (this.IsValidMove(iplayer, npos))
            {
                var nplayer = new Player(player.Id, npos.X, npos.Y, player.Score, PlayerState.Playing);
                newWorld.Players[iplayer] = nplayer;
                
                this.HandleCompteurRules(newWorld, iplayer);
                this.HandleBaffe(newWorld, iplayer);
            }
            else
            {
                newWorld.Players[iplayer] = new Player(iplayer, player.Pos.X, player.Pos.Y, player.Score - 5,
                                                       PlayerState.Stunned);
            }
            this.World = newWorld;

            return this.World;
        }

        private void HandleBaffe(WorldState newWorld, int iplayer)
        {
        }

        private void  HandleCompteurRules(WorldState newWorld, int iplayer)
        {
            var player = this.World.Players[iplayer];
            var nplayer = newWorld.Players[iplayer];

            if (player.HasCompteur)
            {
                var home = newWorld.Caddies[iplayer];
                if (nplayer.Pos == home.Pos)
                {
                    nplayer.IncreaseScore(30);
                    newWorld.Compteurs.Remove(newWorld.Compteurs[player.CompteurId]);
                    newWorld.CheckCompteur(ref nplayer);
                }
                else
                {
                    newWorld.Compteurs[player.CompteurId] = new Compteur(nplayer.Pos.X, nplayer.Pos.Y);
                    nplayer.UpdateCompteur(player.CompteurId);
                }
            }
            else
            {
                newWorld.CheckCompteur(ref nplayer);
                if (nplayer.HasCompteur)
                {
                    nplayer.IncreaseScore(1);
                }
            }
            newWorld.Players[iplayer] = nplayer;
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