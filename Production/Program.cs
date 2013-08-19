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
            IGameAI ai = new ColonizeAI();
            LinkGameField field = LinkGameField.ParseText();
            Console.WriteLine(ai.Prepare(field.Player, field));
            for (int i = 0; i < field.MaxTurn; i++)
            {
                field = LinkGameField.ParseText();
                ai.Think(field.Turn, field.MaxTurn, field.Player, field, field.GetCommander());
            }
        }
    }
}
