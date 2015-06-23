using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.World.Parser
{
    public class ParserTests
    {
        [Fact]
        public void ReadExpected_ShouldSucceed()
        {
            var parser = new WorldParser(",");
            var c = parser.Read(',');
            Assert.Equal(',', c);
        }

        [Fact]
        public void ReadString_ShouldSucceed()
        {
            var parser = new WorldParser("abcdefhhhhh");
            var str = parser.ReadString("abcdef");
            Assert.Equal("abcdef", str);
        }

        [Fact]
        public void ReadUnexpected_ShouldThrow()
        {
            var parser = new WorldParser("abc");
            Assert.Throws<UnexpectedTokenException>(() => parser.Read('?'));
            Assert.Equal('a', parser.Current);
        }

    }
}
