using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.World.Parser
{
    public class ReadDigitTests
    {

        [Fact]
        public void Given5_ShouldSucceed()
        {
            var parser = new WorldParser("5");

            int five = parser.ReadDigit();
            Assert.Equal(5, five);
        }

        [Fact]
        public void Given0_9_ShouldSucceed() 
        {
            var parser = new WorldParser("0123456789");

            for (int i = 0; i < 10; i++)
            {
                int read = parser.ReadDigit();
                Assert.Equal(i, read);
            }
        }
    }
}
