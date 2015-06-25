using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.World
{
    class DistanceMap
    {
        private const int MAP_HEIGHT = WorldState.MAP_HEIGHT + 2;
        private const int MAP_WIDTH = WorldState.MAP_WIDTH + 2;

        private readonly int[,] Data = new int[MAP_WIDTH, MAP_HEIGHT];
        private readonly int Me;
        private readonly Point Objective;

        private readonly WorldState World;

        public DistanceMap(WorldState world, int me, Point objective)
        {
            this.World = world;
            this.Me = me;
            this.Objective = objective;
            this.BuildDistanceMap();
        }

        void BuildDistanceMap()
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                this.Data[x, 0] = int.MaxValue;
                this.Data[x, MAP_HEIGHT - 1] = int.MaxValue;
            }
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                this.Data[0, y] = int.MaxValue;
                this.Data[MAP_WIDTH - 1, y] = int.MaxValue;
            }

            for (int y = 0; y < MAP_HEIGHT - 1; y++)
            {
                for (int x = 0; x < MAP_WIDTH - 1; x++)
                {
                    Data[x, y] = this.Objective.Dist(new Point(x, y));
                }
            }
        }

        public void AddRiskBaffeAtCost(int cost)
        {
            var len = this.World.Players.Count;
            for (int p = 0; p < len; p++)
            {
                if (p != this.Me)
                {
                    var player = this.World.Players[p].Pos;
                    var arounds = Map.PointsAround(player);
                    foreach (var pos in arounds)
                    {
                        var val = Data[pos.X, pos.Y];
                        if (val < int.MaxValue)
                        {
                            Data[pos.X, pos.Y] = val + cost;
                        }
                    }
                }
            }
        }

    }
}
