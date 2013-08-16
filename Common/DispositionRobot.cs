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
            AddMoveOrder(priority, point.X, point.Y, robot, once);
        }

        public void AddMoveOrder(int priority, int x, int y, int robot, bool once)
        {
            OrderSecure temp = new OrderSecure(priority, x, y, robot, once);
            Add(temp);
        }

        public void AddBuildOrder(int priority, Point point, Terrain building, bool once)
        {
            AddBuildOrder(priority, point.X, point.Y, building, once);
        }

        public void AddBuildOrder(int priority, int x, int y, Terrain building, bool once)
        {
            OrderBuild temp = new OrderBuild(priority, x, y, building, once);
            Add(temp);
        }

        public void AddSecureResource(int priority, Point point, GameField field)
        {
            AddSecureResource(priority, point.X, point.Y, field);
        }

        public void AddSecureResource(int priority, int x, int y, GameField field)
        {
            int tx, ty;
            for (int i = 1; i < 7; i++)
            {
                if (field.TransformDirection(i, x, y, out tx, out ty)) continue;
                if (field[tx, ty].Ter != Terrain.Wasteland || field[tx, ty].Player == Player) continue;
                AddMoveOrder(priority, tx, ty, field[tx, ty].WaitRobot + 1, true);
            }
        }

        public void AddSecureGround(int borderRobot, int priority, Point initial, GameField field)
        {
            SearchSecureGround(borderRobot, priority, initial, field, new bool[field.Width, field.Height]);
        }

        private void SearchSecureGround(int borderRobot, int priority, Point point, GameField field, bool[,] searched)
        {
            searched[point.X, point.Y] = true;
            if (field[point].Player != Player && field[point].Ter == Terrain.Wasteland)
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
                SearchSecureGround(borderRobot, priority, temp, field, searched);
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

        public void Dispose(GameField field, Commander commander)
        {
            Sort();
            for (int i = Count - 1; i >= 0; i--)
            {
                if (this[i].Execute(field, commander))
                {
                    RemoveAt(i);
                }
            }
            commander.Finish();
        }
    }
}
