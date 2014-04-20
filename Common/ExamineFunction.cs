using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ExamineFunction
    {
        public static Point GetInitialPoint(this GameField field, int player)
        {
            foreach(Point point in field.Iterator())
            {
                if (field[point].Terrain == Terrain.Initial && field[point].Player == player)
                {
                    return point;
                }
            }
            throw new Exception();
        }

        public static bool IsLiberate(this GameField field, Point point)
        {
            int count = 0;
            foreach (Point temp in field.Adjoin(point, true))
            {
                if (field[temp].Terrain != Terrain.Wasteland) return false;
                count++;
            }
            return count >= 7;
        }

        public static bool IsAdjoinTerritory(this GameField field, Point point, int player)
        {
            foreach (Point temp in field.Adjoin(point))
            {
                if (field[temp].Terrain == Terrain.Hole) continue;
                if (field[temp].Terrain == Terrain.Outside) continue;
                if (field[temp].Player == player) return true;
            }
            return false;
        }

        public static bool IsBoundary(this GameField field, Point point, int player)
        {
            if (field[point].Player == player)
            {
                foreach (Point temp in field.Adjoin(point))
                {
                    if (field[temp].Terrain == Terrain.Hole) continue;
                    if (field[temp].Terrain == Terrain.Outside) continue;
                    if (field[temp].Player != player) return true;
                }
                return false;
            }
            else
            {
                foreach (Point temp in field.Adjoin(point))
                {
                    if (field[temp].Terrain == Terrain.Hole) continue;
                    if (field[temp].Terrain == Terrain.Outside) continue;
                    if (field[temp].Player == player) return true;
                }
                return false;
            }
        }

        public static Point GetNearComer(this GameField field, Point point)
        {
            int com = field.Size - 1;
            int x = point.X - com, y = point.Y - com;
            int a = Math.Max(Math.Abs(x), Math.Abs(y));
            int b = x + y;
            if (a > b)
            {
                if (x < 0)
                {
                    return new Point { X = 0, Y = com * 2 };
                }
                else
                {
                    return new Point { X = com * 2, Y = 0 };
                }
            }
            else if (Math.Abs(x) > Math.Abs(y))
            {
                if (x < 0)
                {
                    return new Point { X = 0, Y = com };
                }
                else
                {
                    return new Point { X = com * 2, Y = com };
                }
            }
            else
            {
                if (x < 0)
                {
                    return new Point { X = com, Y = 0 };
                }
                else
                {
                    return new Point { X = com, Y = com * 2 };
                }
            }
        }

        public static IEnumerable<Point> NearIterator(this GameField field, Point center, int player)
        {
            bool[,] searched = new bool[field.Width, field.Height];
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(center);
            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();
                foreach (Point point in field.Adjoin(current))
                {
                    if (searched[point.X, point.Y]) continue;
                    if (field.IsInMove(point, field[point].Player == player) && field.IsOutMove(point))
                    {
                        queue.Enqueue(point);
                        searched[point.X, point.Y] = true;
                    }
                }
                yield return current;
            }
        }
    }
}
