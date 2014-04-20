using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Common
{
    public struct GameMass
    {
        public int Player;
        public Terrain Terrain;
        public int ActiveRobot;
        public int WaitRobot;
    }

    public struct Point : IEquatable<Point>
    {
        public int X;
        public int Y;

        public override string ToString()
        {
            return String.Format("{0}, {1}", X, Y);
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                return Equals((Point)obj);
            }
            return false;
        }

        public bool Equals(Point other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public static bool operator ==(Point lhs, Point rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Point lhs, Point rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public struct TownPlan
    {
        public readonly int VictoryPoint;
        public readonly int SpendRobot;
        public readonly Point Excavator;
        public readonly ReadOnlyCollection<Point> Town;

        public TownPlan(int victoryPoint, int spendRobot, Point excavator, ReadOnlyCollection<Point> town)
        {
            VictoryPoint = victoryPoint;
            SpendRobot = spendRobot;
            Excavator = excavator;
            Town = town;
        }
    }

    public enum Terrain
    {
        Outside = 0,
        Wasteland,
        Hole,
        Initial,
        RobotMaker = 4,
        AttackTower,
        Excavator,
        House,
        Town,
        Bridge = 9,
    }

    public enum Direction
    {
        Center = 0,
        Right = 1,
        DownerRight,
        DownerLeft,
        Left,
        UpperLeft,
        UpperRight,
    }

    public interface ICommander
    {
        bool IsMove { get; }
        bool IsBuild { get; }
        bool IsFinish { get; }
        int Player { get; }
        void Move(int x, int y, Direction dir, int robot);
        void Build(int x, int y, Terrain building);
        void Finish();
    }
}
