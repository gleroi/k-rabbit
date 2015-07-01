using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.World;

namespace Rabbit.Server
{
    internal class WorldSimulator
    {
    //    public WorldState ApplyAction(int iplayer, Direction direction)
    //    {
    //            Point npos = this.GetNewPos(iplayer, direction);

    //            var nplayers = new List<Player>(this.Players);
    //            var ncompteurs = new List<Compteur>(this.Compteurs);

    //            var player = this.Players[iplayer];

    //            if (this.IsValidMove(iplayer, npos))
    //            {
    //                var nplayer = new Player(player.Id, npos.X, npos.Y, player.Score, PlayerState.Playing);
    //                // compteur
    //                if (player.HasCompteur)
    //                {
    //                    var home = this.Caddies[iplayer];
    //                    if (nplayer.Pos == home.Pos)
    //                    {
    //                        nplayer.IncreaseScore(30);
    //                        ncompteurs.Remove(ncompteurs[player.CompteurId]);
    //                        this.CheckCompteur(ref nplayer);
    //                    }
    //                    else
    //                    {
    //                        ncompteurs[player.CompteurId] = new Compteur(npos.X, npos.Y);
    //                        nplayer.UpdateCompteur(player.CompteurId);
    //                    }
    //                }
    //                else
    //                {
    //                    this.CheckCompteur(ref nplayer);
    //                    if (nplayer.HasCompteur)
    //                    {
    //                        nplayer.IncreaseScore(1);
    //                    }
    //                }
    //                // baffe

    //                nplayers[iplayer] = nplayer;
    //            }
    //            else
    //            {
    //                nplayers[iplayer] = new Player(iplayer, player.Pos.X, player.Pos.Y, player.Score - 5, PlayerState.Stunned);
    //            }
    //            return new WorldState(this.Round, nplayers, ncompteurs, this.Caddies);
    //        }

    //        private Point GetNewPos(int player, Direction direction)
    //        {
    //            var cpos = this.Players[player].Pos;
    //            return cpos.Move(direction);
    //        }

    //        private bool IsValidMove(int player, Point npos)
    //        {
    //            if (npos.X < 0 || npos.Y < 0 || npos.X >= MAP_WIDTH || npos.Y >= MAP_HEIGHT)
    //            {
    //                return false;
    //            }
    //            return this.Players.All(p => p.Pos != npos);
    //        }
    //    }
    }
}