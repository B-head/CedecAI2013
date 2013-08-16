using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{

    public class SearchNearRobot
    {
        Queue<Point> queue;
        GameField field;
        DistanceMap distance;
        int player;

        public SearchNearRobot(GameField field, int player, int x, int y)
        {
            queue = new Queue<Point>();
            queue.Enqueue(new Point { X = x, Y = y });
            this.field = field;
            distance = new DistanceMap(field, player, x, y);
            this.player = player;
        }

        //todo 重複して探査している。
        public bool Next(out int x, out int y, out Direction dir)
        {
            while (queue.Count > 0)
            {
                Point point = queue.Dequeue();
                int tx, ty;
                for (int i = 1; i < 7; i++)
                {
                    if (field.TransformDirection(i, point.X, point.Y, out tx, out ty)) continue;
                    int temp = distance[point.X, point.Y] + 1;
                    if (temp == distance[tx, ty] && field[tx, ty].Ter != Terrain.Hole)
                    {
                        queue.Enqueue(new Point { X = tx, Y = ty });
                    }
                }
                if (field[point.X, point.Y].Player == player && field[point.X, point.Y].ActiveRobot > 0)
                {
                    x = point.X;
                    y = point.Y;
                    dir = distance.ApproachTerget(x, y);
                    return false;
                }
            }
            x = -1;
            y = -1;
            dir = Direction.Center;
            return true;
        }
    }
}
