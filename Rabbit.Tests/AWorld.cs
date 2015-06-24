using Rabbit.World;
using Xunit;

namespace Rabbit.Tests
{
    internal static class AWorld
    {
        public static WorldState WithOnePlayerAt(int x, int y)
        {
            var parser = new WorldParser("worldstate::1;1," + x + "," + y + ",0,playing;9,9:11,9;1,0,5;");
            var world = parser.Parse();

            Assert.NotNull(world);
            Assert.Equal(1, world.Players.Count);

            var prevPlayer = world.Players[0];
            APlayer.Is(prevPlayer, x, y, 0, PlayerState.Playing);
            return world;
        }

        public static WorldState GivenWorld()
        {
            return new WorldState();
        }

        public static WorldState WithPlayer(this WorldState world, int x, int y)
        {
            world.Players.Add(new Player(x, y, 0, PlayerState.Playing));
            return world;
        }

        public static WorldState WithCompteur(this WorldState world, int x, int y)
        {
            world.Compteurs.Add(new Compteur(x, y));
            return world;
        }
    }
}