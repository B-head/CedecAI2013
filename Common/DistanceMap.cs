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
            Investigate(0, player, center);
        }

        private void Initalize()
        {
            foreach(Point point in Iterator())
            {
                this[point] = Impossible;
            }
        }

        private void Investigate(byte distanse, int player, Point point)
        {
            if (!IsInRange(point)) return;
            if (this[point] <= distanse) return;
            if (gameField[point].Terrain == Terrain.Outside) return;
            if (gameField[point].Player != player &&
                gameField[point].Terrain != Terrain.Wasteland && gameField[point].Terrain != Terrain.Hole) return;
            this[point] = distanse;
            if (gameField[point].Terrain == Terrain.Hole && distanse > 0) return;
            foreach(Point temp in Adjoin(point))
            {
                Investigate((byte)(distanse + 1), player, temp);
            }
        }
    }
}
