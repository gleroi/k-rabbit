using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.World
{
    public class World_MoveCompteur
    {
        [Theory,
        InlineData(8, 9, Direction.E),
        InlineData(10, 9, Direction.O),
        InlineData(9, 8, Direction.N),
        InlineData(9, 10, Direction.S)]
        public void MoveOnCompteur_ShouldHasCompteur(int x, int y, Direction direction)
        {
            var world = AWorld.WithOnePlayerAt(x, y);
            Assert.False(world.Players[0].HasCompteur);

            var nworld = world.ApplyAction(0, direction);

            APlayer.Is(nworld.Players[0], 9, 9, 0, PlayerState.Playing);
            Assert.True(nworld.Players[0].HasCompteur);
        }

        [Theory,
        InlineData(10, 9, Direction.E),
        InlineData(8, 9, Direction.O),
        InlineData(9, 10, Direction.N),
        InlineData(9, 8, Direction.S)]
        public void MoveWithCompteur_ShouldHasCompteur(int x, int y, Direction direction)
        {
            var world = AWorld.WithOnePlayerAt(9, 9);
            ACompteur.Is(world.Compteurs[0], 9, 9);
            Assert.True(world.Players[0].HasCompteur);

            var nworld = world.ApplyAction(0, direction);

            APlayer.Is(nworld.Players[0], x, y, 0, PlayerState.Playing);
            ACompteur.Is(nworld.Compteurs[0], x, y);
            Assert.True(nworld.Players[0].HasCompteur);
        }

        [Fact]
        public void HaveCompteur_ShouldKeepIt() {
            var world = AWorld.WithOnePlayerAt(11, 9);

            ACompteur.Is(world.Compteurs[0], 9, 9);
            Assert.Equal(1, world.Players[0].CompteurId);

            world = world.ApplyAction(0, Direction.O);

            Assert.Equal(1, world.Players[0].CompteurId);
            ACompteur.Is(world.Compteurs[0], 9, 9);
            ACompteur.Is(world.Compteurs[1], 10, 9);

            world = world.ApplyAction(0, Direction.O);

            Assert.Equal(0, world.Players[0].CompteurId);
            ACompteur.Is(world.Compteurs[0], 9, 9);
            ACompteur.Is(world.Compteurs[1], 9, 9);

            world = world.ApplyAction(0, Direction.O);

            Assert.Equal(0, world.Players[0].CompteurId);
            ACompteur.Is(world.Compteurs[0], 8, 9);
            ACompteur.Is(world.Compteurs[1], 9, 9);

            world = world.ApplyAction(0, Direction.O);
            Assert.Equal(0, world.Players[0].CompteurId);
            ACompteur.Is(world.Compteurs[0], 7, 9);
            ACompteur.Is(world.Compteurs[1], 9, 9);

        }
    }
}
