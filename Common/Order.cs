using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public abstract class Order : IComparable<Order>
    {
        public readonly int Priority;
        public readonly bool Once;
        public readonly Point Point;

        public Order(int priority, bool once, Point point)
        {
            this.Priority = priority;
            this.Once = once;
            this.Point = point;
        }

        public abstract bool Execute(GameField field, ICommander commander);

        protected bool IsForbidMove(ICommander commander)
        {
            return commander.IsBuild || commander.IsFinish;
        }

        protected bool IsForbidBuild(ICommander commander)
        {
            return commander.IsMove || commander.IsBuild || commander.IsFinish;
        }

        protected bool NearRobotMove(Point point, int robot, GameField field, ICommander commander)
        {
            int moveCount = 0;
            DistanceMap distance = new DistanceMap(field, commander.Player, point);
            foreach (Point temp in field.NearIterator(point, commander.Player))
            {
                if (field[temp].ActiveRobot <= 0) continue;
                if (field[temp].Terrain == Terrain.Hole && !point.Equals(temp)) continue;
                int moveRobot = Math.Min(field[temp].ActiveRobot, robot - moveCount);
                Direction dir = distance.ApproachTerget(temp);
                if (!field.Move(temp, dir, moveRobot)) throw new Exception();
                if (dir != Direction.Center)
                {
                    commander.Move(temp.X, temp.Y, dir, moveRobot);
                }
                moveCount += moveRobot;
                if (moveCount >= robot) break;
            }
            return false;
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

    public class OrderSecure : Order
    {
        public readonly int Robot;

        public OrderSecure(int priority, bool once, Point point, int robot)
            : base(priority, once, point)
        {
            this.Robot = robot;
        }

        public override bool Execute(GameField field, ICommander commander)
        {
            if (IsForbidMove(commander)) return false;
            if (field[this.Point].Terrain != Terrain.Wasteland) return true;
            if (NearRobotMove(this.Point, Robot, field, commander)) return true;
            return false;
        }
    }

    public class OrderBuild : Order
    {
        public readonly Terrain Building;

        public OrderBuild(int priority, bool once, Point point, Terrain building)
            : base(priority, once, point)
        {
            this.Building = building;
        }

        public override bool Execute(GameField field, ICommander commander)
        {
            if (field[this.Point].Terrain != Terrain.Wasteland && field[this.Point].Terrain != Terrain.Hole) return true;
            if (!IsForbidBuild(commander) && field.IsBuild(this.Point, Building))
            {
                commander.Build(this.Point.X, this.Point.Y, Building);
                return true;
            }
            if (IsForbidMove(commander)) return false;
            if (NearRobotMove(this.Point, field.GetNeedRobot(Building), field, commander)) return true;
            return false;
        }
    }
}
