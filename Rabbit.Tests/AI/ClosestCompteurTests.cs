using Rabbit.AI;
using Xunit;

namespace Rabbit.Tests.AI
{
    public class ClosestCompteurTests
    {
        [Theory,
         InlineData(5, 6, 7, 6, 0),
         InlineData(7, 6, 5, 6, 1),
         InlineData(1, 4, 1, 8, 0)]
        public void OnePlayer_ReturnsClosest(int x1, int y1, int x2, int y2, int expectedCompteur)
        {
            var world = AWorld.GivenWorld()
                .WithCompteur(x1, y1)
                .WithCompteur(x2, y2)
                .WithPlayer(1, 6);
            var ai = new GoCompteur(0);

            var idCompteur = ai.FindClosestCompteur(world);
            Assert.Equal(expectedCompteur, idCompteur);
        }

        [Theory,
         InlineData(1, 4, 6, 1, 1),
         InlineData(1, 4, 4, 1, 1),
         InlineData(3, 1, 6, 1, 0)]
        public void TwoPlayer_ReturnsClosestAndFirst(int x1, int y1, int x2, int y2, int expectedCompteur)
        {
            var world = AWorld.GivenWorld()
                .WithPlayer(1, 1)
                .WithPlayer(1, 6)
                .WithCompteur(x1, y1)
                .WithCompteur(x2, y2);

            var ai = new GoCompteur(0);
            Assert.Equal(expectedCompteur, ai.FindClosestCompteur(world));
        }
    }
}