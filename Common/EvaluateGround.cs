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

        public int GetMaxEvaluate()
        {
            int result = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (field[x, y] == Obstacle) continue;
                    result = Math.Max(result, field[x, y]);
                }
            }
            return result;
        }

        //todo 重複して探査している。
        public void GetNearEvaluate(int ev, int player, int cx, int cy, out int nx, out int ny)
        {
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point { X = cx, Y = cy });
            DistanceMap distance = new DistanceMap(gameField, player, cx, cy);
            while (queue.Count > 0)
            {
                Point point = queue.Dequeue();
                int tx, ty;
                for (int i = 1; i < 7; i++)
                {
                    if (TransformDirection(i, point.X, point.Y, out tx, out ty)) continue;
                    if (distance[point.X,point.Y] + 1 == distance[tx,ty])
                    {
                        queue.Enqueue(new Point { X = tx, Y = ty });
                    }
                }
                if (field[point.X, point.Y] == ev)
                {
                    nx = point.X;
                    ny = point.Y;
                    return;
                }
            }
            nx = -1;
            ny = -1;
        }
    }
}
