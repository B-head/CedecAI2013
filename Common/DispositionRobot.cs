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

        public void AddMoveOrder(int priority, Point point, int robot, bool once)
        {
            OrderSecure temp = new OrderSecure(priority, once, point, robot);
            Add(temp);
        }

        public void AddBuildOrder(int priority, Point point, Terrain building, bool once)
        {
            OrderBuild temp = new OrderBuild(priority, once, point, building);
            Add(temp);
        }

        public void AddSecureResource(int priority, Point point, GameField field)
        {
            foreach(Point temp in field.Adjoin(point))
            {
                if (field[temp].Terrain != Terrain.Wasteland) continue;
                AddMoveOrder(priority, temp, field[temp].WaitRobot + 1, true);
            }
        }

        public void AddSecureGround(int priority, int borderRobot, Point initial, GameField field)
        {
            SearchSecureGround(priority, borderRobot, initial, field, new bool[field.Width, field.Height]);
        }

        private void SearchSecureGround(int priority, int borderRobot, Point point, GameField field, bool[,] searched)
        {
            searched[point.X, point.Y] = true;
            if (field[point].Player != Player && field[point].Terrain == Terrain.Wasteland)
            {
                if (field[point].WaitRobot <= borderRobot)
                {
                    AddMoveOrder(priority - field[point].WaitRobot, point, field[point].WaitRobot + 1, true);
                }
                return;
            }
            foreach (Point temp in field.Adjoin(point))
            {
                if (searched[temp.X, temp.Y]) continue;
                SearchSecureGround(priority, borderRobot, temp, field, searched);
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
            for (int i = Count - 1; i >= 0; i--)
            {
                if (this[i].Execute(field, commander) || this[i].Once)
                {
                    this[i] = null;
                }
            }
            commander.Finish();
            RemoveAll(o => o == null);
        }
    }
}
