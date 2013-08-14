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
                field.TransformTowerRange(i, x, y, out tx, out ty);
                if (field.IsInRange(tx, ty)) continue;
                if (field[tx, ty].Ter != Terrain.AttackTower) continue;
                if (field[tx, ty].Player == player) continue;
                result += 2;
            }
            return result;
        }
    }
}
