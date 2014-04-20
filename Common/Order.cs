using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public abstract class Order : IComparable<Order>
    {
        public readonly int Priority;
        public readonly Point Point;

        public Order(int priority, Point point)
        {
            this.Priority = priority;
            this.Point = point;
        }

        public abstract void Execute(GameField field, ICommander commander);

        protected bool IsForbidMove(ICommander commander)
        {
            return commander.IsBuild || commander.IsFinish;
        }

        protected bool IsForbidBuild(ICommander commander)
        {
            return commander.IsMove || commander.IsBuild || commander.IsFinish;
        }

        protected void NearRobotMove(Point point, int robot, GameField field, ICommander commander)
        {
            int moveCount = 0;
            foreach (Point temp in field.NearIterator(point, commander.Player))
            {
                if (field[temp].ActiveRobot <= 0) continue;
                if (field[temp].Terrain == Terrain.Hole && !point.Equals(temp)) continue;
                int moveRobot = Math.Min(field[temp].ActiveRobot, robot - moveCount);
                Direction dir = field.ApproachPoint(temp, point);
                if (!field.Move(temp, dir, moveRobot)) throw new Exception();
                if (dir != Direction.Center)
                {
                    commander.Move(temp.X, temp.Y, dir, moveRobot);
                }
                moveCount += moveRobot;
                if (moveCount >= robot) break;
            }
        }

        public int CompareTo(Order other)
        {
            int p = Priority.CompareTo(other.Priority);
            if (p != 0) return p;
            int x = this.Point.X.CompareTo(other.Point.X);
            if (x != 0) return x;
            return this.Point.Y.CompareTo(other.Point.Y);
        }
    }

    public class OrderMove : Order
    {
        public readonly int Robot;

        public OrderMove(int priority, Point point, int robot)
            : base(priority, point)
        {
            this.Robot = robot;
        }

        public override void Execute(GameField field, ICommander commander)
        {
            if (IsForbidMove(commander)) return;
            NearRobotMove(this.Point, Robot, field, commander);
        }
    }

    public class OrderBuild : Order
    {
        public readonly Terrain Building;

        public OrderBuild(int priority, Point point, Terrain building)
            : base(priority, point)
        {
            this.Building = building;
        }

        public override void Execute(GameField field, ICommander commander)
        {
            if (IsForbidBuild(commander)) return;
            if (!field.IsBuild(this.Point, Building)) return;
            commander.Build(this.Point.X, this.Point.Y, Building);
        }
    }
}
