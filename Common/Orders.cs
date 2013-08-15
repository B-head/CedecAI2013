using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public abstract class Order : IComparable<Order>
    {
        int priority;

        public Order(int priority)
        {
            this.priority = priority;
        }

        public abstract bool Execute(GameField field, Commander commander);

        protected bool IsForbidMove(Commander commander)
        {
            return commander.IsBuild || commander.IsFinish;
        }

        protected bool IsForbidBuild(Commander commander)
        {
            return commander.IsMove || commander.IsBuild || commander.IsFinish;
        }

        protected void NearRobotMove(int x, int y, int robot, GameField field, Commander commander)
        {
            SearchNearRobot search = new SearchNearRobot(field, commander.Player, x, y);
            int moveCount = 0, tx, ty;
            Direction dir;
            while (moveCount < robot)
            {
                if (search.Next(out tx, out ty, out dir)) break;
                int moveRobot = Math.Min(field[tx, ty].ActiveRobot, robot - moveCount);
                field.Move(tx, ty, dir, moveRobot);
                if (dir != Direction.Center)
                {
                    commander.Move(tx, ty, dir, moveRobot);
                }
                moveCount += moveRobot;
            }
        }

        public int CompareTo(Order other)
        {
            return priority - other.priority;
        }
    }

    public class OrderMove : Order
    {
        int x;
        int y;
        int robot;
        bool once;

        public OrderMove(int priority, int x, int y, int robot, bool once)
            : base(priority)
        {
            this.x = x;
            this.y = y;
            this.robot = robot;
            this.once = once;
        }

        public override bool Execute(GameField field, Commander commander)
        {
            if (IsForbidMove(commander)) return once;
            NearRobotMove(x, y, robot, field, commander);
            return once;
        }
    }

    public class OrderBuild : Order
    {
        int x;
        int y;
        Terrain building;

        public OrderBuild(int priority, int x, int y, Terrain building)
            : base(priority)
        {
            this.x = x;
            this.y = y;
            this.building = building;
        }

        public override bool Execute(GameField field, Commander commander)
        {
            if (!IsForbidBuild(commander) || field.IsBuild(x, y, building))
            {
                commander.Build(x, y, building);
                return true;
            }
            if (IsForbidMove(commander)) return false;
            int resource, robot;
            field.GetRequirement(building, out resource, out robot);
            NearRobotMove(x, y, robot, field, commander);
            return false;
        }
    }
}
