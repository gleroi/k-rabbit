using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.AI;
using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.AI
{
    public class TreeTests
    {
        [Fact]
        public void TwoPlayer_TreeGeneration()
        {
            var worldstate =
                "worldstate::0;1,0,0,0,playing:2,15,11,0,playing;" +
                "4,4:6,6;" +
                "1,0,0:2,15,11;";
            var parser = new WorldParser(worldstate);
            var world = parser.Parse();

            Tree tree = Tree.Generate(world, 0, 11);
            Assert.NotNull(tree);
            Assert.Equal(0, tree.MetaScore(0));
        }
    }
}
