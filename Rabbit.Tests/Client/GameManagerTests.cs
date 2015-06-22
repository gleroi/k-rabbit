using Rabbit.Client;
using Xunit;

namespace Rabbit.Tests.Client
{
    public class GameManagerTests
    {
        //[Fact]
        public void LifeCyle_ShouldSucceed()
        {
            var manager = new GameManager();

            var gameId = manager.Create(Config.TeamId, Config.Secret);
            manager.StartGame(gameId, Config.TeamId, Config.Secret);
            manager.StopGame(gameId, Config.TeamId, Config.Secret);
        }
    }
}
