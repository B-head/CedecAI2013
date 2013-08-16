using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public abstract class Order : IComparable<Order>
    {
        public readonly int Priority;

        public Order(int priority)
        {
            this.Priority = priority;
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

        protected bool NearRobotMove(int x, int y, int robot, GameField field, Commander commander)
        {
            SearchNearRobot search = new SearchNearRobot(field, commander.Player, x, y);
            int moveCount = 0, tx, ty;
            Direction dir;
            while (moveCount < robot)
            {
                if (search.Next(out tx, out ty, out dir)) break;
                int moveRobot = Math.Min(field[tx, ty].ActiveRobot, robot - moveCount);
                if (!field.Move(tx, ty, dir, moveRobot)) throw new Exception();
                if (dir != Direction.Center)
                {
                    commander.Move(tx, ty, dir, moveRobot);
                }
                moveCount += moveRobot;
            }
            return false;
        }

        public int CompareTo(Order other)
        {
            return Priority - other.Priority;
        }
    }

    public class OrderSecure : Order
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Robot;
        public readonly bool Once;

        public OrderSecure(int priority, int x, int y, int robot, bool once)
            : base(priority)
        {
            this.X = x;
            this.Y = y;
            this.Robot = robot;
            this.Once = once;
        }

        public override bool Execute(GameField field, Commander commander)
        {
            if (IsForbidMove(commander)) return Once;
            if (field[X, Y].Ter != Terrain.Wasteland) return true;
            if (NearRobotMove(X, Y, Robot, field, commander)) return true;
            return Once;
        }
    }

    public class OrderBuild : Order
    {
        public readonly int X;
        public readonly int Y;
        public readonly Terrain Building;
        public readonly bool Once;

        public OrderBuild(int priority, int x, int y, Terrain building, bool once)
            : base(priority)
        {
            this.X = x;
            this.Y = y;
            this.Building = building;
            this.Once = once;
        }

        public override bool Execute(GameField field, Commander commander)
        {
            if (!IsForbidBuild(commander) && field.IsBuild(X, Y, Building))
            {
                commander.Build(X, Y, Building);
                return true;
            }
            if (IsForbidMove(commander)) return Once;
            int resource, robot;
            field.GetRequirement(Building, out resource, out robot);
            if (NearRobotMove(X, Y, robot + resource, field, commander)) return true;
            return Once;
        }
    }
}
