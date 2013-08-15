using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class DispositionRobot
    {
        public readonly int Player;
        List<Order> orders;

        public DispositionRobot(int player)
        {
            Player = player;
            orders = new List<Order>();
        }

        public Order this[int index]
        {
            get
            {
                return orders[index];
            }
        }

        public void AddMoveOrder(int priority, int x, int y, int robot, bool once)
        {
            OrderMove temp = new OrderMove(priority, x, y, robot, once);
            orders.Add(temp);
        }

        public void AddBuildOrder(int priority, int x, int y, Terrain building)
        {
            OrderBuild temp = new OrderBuild(priority, x, y, building);
            orders.Add(temp);
        }

        public void Clear()
        {
            orders.Clear();
        }

        public void Dispose(GameField field, Commander commander)
        {
            orders.Sort();
            for (int i = orders.Count - 1; i >= 0; i--)
            {
                if (orders[i].Execute(field, commander))
                {
                    orders.RemoveAt(i);
                }
            }
            commander.Finish();
        }
    }
}
