using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class DispositionRobot : List<Order>
    {
        public readonly int Player;

        public DispositionRobot(int player)
        {
            Player = player;
        }

        public void AddMoveOrder(int priority, Point point, int robot)
        {
            OrderMove temp = new OrderMove(priority, point, robot);
            Add(temp);
        }

        public void AddBuildOrder(int priority, Point point, Terrain building)
        {
            OrderBuild b = new OrderBuild(priority, point, building);
            Add(b);
        }

        public void AddSecureResource(int priority, Point point, int player, GameField field)
        {
            foreach(Point temp in field.Adjoin(point))
            {
                if (field[temp].Terrain != Terrain.Wasteland) continue;
                if (field[temp].Player == player) continue;
                AddMoveOrder(priority, temp, field[temp].WaitRobot + 1);
            }
        }

        public void AddSecureGround(int invadePriority, int guardPriority, int player, GameField field)
        {
            foreach (Point point in field.Iterator())
            {
                if (field[point].Terrain != Terrain.Wasteland) continue;
                if (!field.IsBoundary(point, player)) continue;
                if (field[point].Player == player)
                {
                    AddMoveOrder(guardPriority, point, 1);
                }
                else
                {
                    if (field[point].WaitRobot > 0) continue;
                    AddMoveOrder(invadePriority, point, 1);
                }
            }
        }

        public void AddKamikaze(int priority, int player, GameField field)
        {
            foreach (Point point in field.Iterator())
            {
                if (field[point].Terrain != Terrain.Wasteland) continue;
                if (field[point].Player == player)continue;
                AddMoveOrder(priority - field[point].WaitRobot, point, field[point].WaitRobot + 1);
            }
        }

        public IEnumerable<ORDER> EnumerateOrder<ORDER>() where ORDER : Order
        {
            int length = Count;
            for (int i = 0; i < length; i++)
            {
                if (this[i] is ORDER)
                {
                    yield return (ORDER)this[i];
                }
            }
        }

        public void Dispose(GameField field, ICommander commander)
        {
            Sort();
            bool isOrderBuild = false;
            for (int i = Count - 1; i >= 0; i--)
            {
                if (this[i] is OrderBuild)
                {
                    if (isOrderBuild) continue;
                    isOrderBuild = true;
                }
                this[i].Execute(field, commander);
            }
            Clear();
            commander.Finish();
        }
    }
}
