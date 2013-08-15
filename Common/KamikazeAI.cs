using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class KamikazeAI : GameAI
    {
        DispositionRobot disposition;

        public string Prepare(int player, GameField field)
        {
            disposition = new DispositionRobot(player);
            return "B_head:KamikazeAI";
        }

        public void Think(int turn, int maxTurn, int player, GameField field, Commander commander)
        {
            int w = field.Width, h = field.Height;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (field[x, y].Player != player && field[x, y].Ter == Terrain.Wasteland)
                    {
                        disposition.AddMoveOrder(-field[x, y].WaitRobot, x, y, field[x, y].WaitRobot + 1, true);
                    }
                }
            }
            disposition.Dispose(field, commander);
        }
    }
}
