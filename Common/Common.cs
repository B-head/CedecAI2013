using System;
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
        public Terrain Ter;
        public int ActiveRobot;
        public int WaitRobot;
    }

    public struct IntPoint
    {
        public int X;
        public int Y;
    }

    public enum Terrain
    {
        Outside,
        Wasteland,
        Hole,
        Initial,
        RobotMaker,
        AttackTower,
        Excavator,
        Bridge,
        House,
        Town,
    }

    public enum Direction
    {
        Center,
        Right,
        DownerRight,
        DownerLeft,
        Left,
        UpperLeft,
        UpperRight,
    }

    public interface GameAI
    {
        string Prepare(int player, GameField field);
        void Think(int turn, int maxTurn, int player, GameField field, Commander commander);
    }

    public interface Commander
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
