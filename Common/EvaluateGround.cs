using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class EvaluateGround : Field<byte>
    {
        GameField gameField;
        public const byte Obstacle = byte.MaxValue;

        public EvaluateGround(GameField gameField)
            : base(gameField.Width, gameField.Height)
        {
            this.gameField = gameField;
            Initalize();
            AllEvaluate();
        }

        private void Initalize()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (gameField[x, y].Ter != Terrain.Wasteland)
                    {
                        field[x, y] = Obstacle;
                    }
                }
            }
        }

        private void AllEvaluate()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (field[x, y] == Obstacle) continue;
                    field[x, y] = (byte)Evaluate(x, y);
                }
            }
        }

        private int Evaluate(int x, int y)
        {
            int result = 0, tx, ty;
            for (int i = 0; i < 12; i++)
            {
                if (TransformSitePropose(i, x, y, out tx, out ty)) continue;
                if (!PertEvaluate(tx, ty)) continue;
                if(i <6)
                {
                    result += 2;
                }
                else
                {
                    result += 3;
                }
            }
            return result;
        }

        private bool PertEvaluate(int x, int y)
        {
            int tx, ty;
            for (int i = 0; i < 7; i++)
            {
                if (TransformDirection(i, x, y, out tx, out ty)) return false;
                if (field[tx, ty] == Obstacle) return false;
            }
            return true;
        }

        public bool TransformSitePropose(int i, int x, int y, out int tx, out int ty)
        {
            tx = x;
            ty = y;
            switch (i)
            {
                case 0: tx += 1; ty += 0; break;
                case 1: tx += 1; ty += -1; break;
                case 2: tx += 0; ty += 1; break;
                case 3: tx += -1; ty += 0; break;
                case 4: tx += -1; ty += 1; break;
                case 5: tx += 0; ty += -1; break;
                case 6: tx += 1; ty += 1; break;
                case 7: tx += -1; ty += 2; break;
                case 8: tx += -2; ty += 1; break;
                case 9: tx += -1; ty += -1; break;
                case 10: tx += 1; ty += -2; break;
                case 11: tx += 2; ty += -1; break;
                default: throw new Exception();
            }
            return !IsInRange(tx, ty);
        }
    }
}
