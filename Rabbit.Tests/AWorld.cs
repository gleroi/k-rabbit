using Rabbit.World;

namespace Rabbit.Tests
{
    internal static class AWorld
    {
        public static WorldState WithOnePlayerAt(int x, int y)
        {
            var world = AWorld.GivenWorld()
                .WithPlayer(x, y)
                .WithCaddy(0, 5)
                .WithCompteur(9, 9)
                .WithCompteur(11, 9);
            return world;
        }

        public static WorldState GivenWorld()
        {
            return new WorldState();
        }

        public static WorldState WithPlayer(this WorldState world, int x, int y)
        {
            var id = world.Players.Count;
            world.Players.Add(new Player(id, x, y, 0, PlayerState.Playing));
            return world;
        }

        public static WorldState WithCompteur(this WorldState world, int x, int y)
        {
            world.Compteurs.Add(new Compteur(x, y));
            return world;
        }

        public static WorldState WithCaddy(this WorldState world, int x, int y)
        {
            world.Caddies.Add(new Caddy(x, y));
            return world;
        }
    }
}