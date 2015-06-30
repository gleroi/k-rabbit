using Rabbit.AI;
using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.AI
{
    public class TreeTests
    {
        [Fact]
        public void TwoPlayer_FarAway_TreeGeneration()
        {
            var worldstate =
                "worldstate::0;1,0,0,0,playing:2,15,11,0,playing;" +
                "4,4:6,6;" +
                "1,0,0:2,15,11;";
            var parser = new WorldParser(worldstate);
            var world = parser.Parse();

            Tree tree = Tree.Generate(world, 0, 10);
            Assert.NotNull(tree);
            Assert.Equal(0, tree.MetaScore(0));
        }

        [Fact]
        public void TwoPlayer_Close_TreeGeneration()
        {
            var worldstate =
                "worldstate::0;1,1,0,0,playing:2,1,3,0,playing;" +
                "3,3:4,3;" +
                "1,0,0:2,1,3;";
            var parser = new WorldParser(worldstate);
            var world = parser.Parse();

            Tree tree = Tree.Generate(world, 0, 10);
            Assert.NotNull(tree);
            Assert.Equal(0, tree.MetaScore(0));
        }
    }
}
