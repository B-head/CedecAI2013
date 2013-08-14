using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Production
{
    class Program
    {
        static void Main(string[] args)
        {
            int turn, maxTurn, player;
            GameAI ai = new TestAI();
            LinkGameField field = LinkGameField.ParseText(out turn, out maxTurn, out player);
            Console.WriteLine(ai.Prepare(player, field));
            for (int i = 0; i < maxTurn; i++)
            {
                field = LinkGameField.ParseText(out turn, out maxTurn, out player);
                ai.Think(turn, maxTurn, player, field, field.GetCommander());
            }
        }
    }
}
