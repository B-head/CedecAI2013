using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class DistanceMap : Field<byte>
    {
        readonly GameField gameField;
        readonly Point center;
        public const byte Impossible = byte.MaxValue;

        public DistanceMap(GameField gameField, int player, Point center)
            : base(gameField.Width, gameField.Height)
        {
            this.gameField = gameField;
            this.center = center;
            Initalize();
            Investigate(0, player, center.X, center.Y);
        }

        private void Initalize()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    field[x, y] = Impossible;
                }
            }
        }

        private void Investigate(byte distanse, int player, int x, int y)
        {
            if (!IsInRange(x, y)) return;
            if (field[x, y] <= distanse) return;
            if (gameField[x, y].Terrain == Terrain.Outside) return;
            if (gameField[x, y].Player != player && 
                gameField[x, y].Terrain != Terrain.Wasteland && gameField[x, y].Terrain != Terrain.Hole) return;
            field[x, y] = distanse;
            if (gameField[x, y].Terrain == Terrain.Hole && distanse > 0) return;
            int tx, ty;
            for (int i = 1; i < 7; i++)
            {
                TransformDirection(i, x, y, out tx, out ty);
                Investigate((byte)(distanse + 1), player, tx, ty);
            }
        }

        public Direction ApproachTerget(Point point)
        {
            return ApproachTerget(point.X, point.Y);
        }

        public Direction ApproachTerget(int x, int y)
        {
            int tx, ty;
            for (int i = 1; i < 7; i++)
            {
                if (TransformDirection(i, x, y, out tx, out ty)) continue;
                if (gameField[tx, ty].Terrain == Terrain.Hole && field[tx, ty] != 0) continue;
                if (field[x, y] - 1 == field[tx, ty])
                {
                    return (Direction)i;
                }
            }
            return Direction.Center;
        }
    }
}
