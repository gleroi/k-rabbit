using System;
using System.Net.Http;

namespace Rabbit.Client
{
    class GameManager
    {
        readonly HttpClient http;

        public GameManager()
        {
            this.http = new HttpClient();
        }

        const string BASE = "http://battle.gate.vm.gate.erdf.fr:8080/";

        private const string ENV = "test/";

        private const string CREATE = "createBattle?teamId={0}&secret={1}";

        private const string START = "startBattle?gameId={0}&teamId={1}&secret={2}";

        private const string STOP = "stopBattle?gameId={0}&teamId={1}&secret={2}";


        private string BuildUrl(string url)
        {
            return BASE + ENV + url;
        }

        private string RunQuery(string url)
        {
            var task = this.http.GetStringAsync(url);
            var result = task.Result;
            return result;
        }

        public int Create(int teamId, string secret)
        {
            var url = BuildUrl(String.Format(CREATE, teamId, secret));
            var result = RunQuery(url);
            Log.Write("Game {0} created", result);
            return int.Parse(result);
        }


        public void StartGame(int gameId, int teamId, string secret)
        {
            var url = BuildUrl(String.Format(START, gameId, teamId, secret));
            RunQuery(url);
            Log.Write("Game {0} started", gameId);
        }

        public void StopGame(int gameId, int teamId, string secret)
        {
            var url = BuildUrl(String.Format(STOP, gameId, teamId, secret));
            RunQuery(url);
            Log.Write("Game {0} stopped", gameId);
        }
    }
}
