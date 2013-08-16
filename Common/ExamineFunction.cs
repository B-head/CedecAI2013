using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ExamineFunction
    {
        public static int GetEstimateTowerDamage(this GameField field, int player, int x, int y)
        {
            int result = 0, tx, ty;
            for (int i = 1; i < 13; i++)
            {
                if (field.TransformTowerRange(i, x, y, out tx, out ty)) continue;
                if (field[tx, ty].Ter != Terrain.AttackTower) continue;
                if (field[tx, ty].Player == player) continue;
                result += 2;
            }
            return result;
        }

        public static Point GetInitialPoint(this GameField field, int player)
        {
            int w = field.Width, h = field.Height;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (field[x, y].Ter == Terrain.Initial && field[x, y].Player == player)
                    {
                        return new Point { X = x, Y = y };
                    }
                }
            }
            throw new Exception();
        }

        public static bool IsLiberate(this GameField field, Point point)
        {
            for(int i=0;i<7;i++)
            {
                Point temp = field.TransformDirection(i, point);
                if (!field.IsInRange(temp)) return false;
                if (field[temp].Ter != Terrain.Wasteland) return false;
            }
            return true;
        }
    }
}
