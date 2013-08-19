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
