using System.Collections.Generic;
using System.Linq;

namespace Rabbit.World
{
    internal class Map
    {
        private const int MAP_HEIGHT = WorldState.MAP_HEIGHT + 2;
        private const int MAP_WIDTH = WorldState.MAP_WIDTH + 2;

        private readonly CellState[,] Data = new CellState[MAP_WIDTH, MAP_HEIGHT];
        private readonly int Id;
        private readonly WorldState State;

        public Map(WorldState state, int meId)
        {
            this.State = state;
            this.Id = meId;
            this.BuildMap();
        }

        private void BuildMap()
        {
            this.FillBorder();
            this.FillImpossibleAndRisk();
            FillBaffable();
        }

        private void FillBaffable()
        {
            var me = this.State.Players[this.Id].Pos;
            this.TryFillBaffable(me);
        }

        private void TryFillBaffable(Point move)
        {
            var x = move.X + 1;
            var y = move.Y + 1;

            if (this.HasRabbit(x + 2, y))
            {
                this.TryFill(x + 1, y, CellState.Baffable);
            }

            if (this.HasRabbit(x + 1, y + 1))
            {
                this.TryFill(x + 1, y, CellState.Baffable);
                this.TryFill(x, y + 1, CellState.Baffable);
            }
            if (this.HasRabbit(x, y + 2))
            {
                this.TryFill(x, y+1, CellState.Baffable);
            }

            if (this.HasRabbit(x - 1, y + 1))
            {
                this.TryFill(x, y+1, CellState.Baffable);
                this.TryFill(x - 1, y, CellState.Baffable);
            }

            if (this.HasRabbit(x - 2, y))
            {
                this.TryFill(x - 1, y, CellState.Baffable);
            }

            if (this.HasRabbit(x - 1, y - 1))
            {
                this.TryFill(x - 1, y, CellState.Baffable);
                this.TryFill(x, y-1,CellState.Baffable);
            }

            if (this.HasRabbit(x, y - 2))
            {
                this.TryFill(x, y-1,CellState.Baffable);
            }
            if (this.HasRabbit(x + 1, y - 1))
            {
                this.TryFill(x, y-1,CellState.Baffable);
                this.TryFill(x+1, y, CellState.Baffable);
            }
        }

        private bool HasRabbit(int xMap, int yMap)
        {
            var pos = new Point(xMap - 1, yMap - 1);
            return this.State.Players.Any(p => p.Pos == pos);
        }

        private void FillImpossibleAndRisk()
        {
            for (int p = 0; p < this.State.Players.Count; p++)
            {
                if (p != this.Id)
                {
                    var player = this.State.Players[p].Pos;
                    this.Data[player.X + 1, player.Y + 1] = CellState.Impossible;
                    this.FillAround(player, CellState.RiskBaffe);
                }
            }
        }

        public static Point[] PointsAround(Point player)
        {
            int x = player.X + 1;
            int y = player.Y + 1;

            var result = new Point[8];

            result[0] = new Point(x + 2, y);
            result[1] = new Point(x + 1, y + 1);
            result[2] = new Point(x, y + 2);
            result[3] = new Point(x - 1, y + 1);
            result[4] = new Point(x - 2, y);
            result[5] = new Point(x - 1, y - 1);
            result[6] = new Point(x, y - 2);
            result[7] = new Point(x + 1, y - 1);
            return result;
        }

        private void FillAround(Point player, CellState state)
        {
            int x = player.X + 1;
            int y = player.Y + 1;

            this.TryFill(x + 2, y, state);
            this.TryFill(x + 1, y + 1, state);
            this.TryFill(x, y + 2, state);
            this.TryFill(x - 1, y + 1, state);
            this.TryFill(x - 2, y, state);
            this.TryFill(x - 1, y - 1, state);
            this.TryFill(x, y - 2, state);
            this.TryFill(x + 1, y - 1, state);
        }

        private void TryFill(int x, int y, CellState state)
        {
            if (x >= 0 && x < MAP_WIDTH && y >= 0 && y < MAP_HEIGHT)
            {
                this.Data[x, y] |= state;
            }
        }

        private void FillBorder()
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                this.Data[x, 0] = CellState.Impossible;
                this.Data[x, MAP_HEIGHT - 1] = CellState.Impossible;
            }
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                this.Data[0, y] = CellState.Impossible;
                this.Data[MAP_WIDTH - 1, y] = CellState.Impossible;
            }
        }

        public CellState GetCell(Point current)
        {
            return Data[current.X + 1, current.Y + 1];
        }
    }
}