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
        Right,
        UpperRight,
        DownerRight,
        Left,
        UpperLeft,
        DownerLeft,
    }

    public interface GameAI
    {
        string Prepare(int player, GameField field);
        void Think(int turn, int maxTurn, int player, GameField field, Commander com);
    }

    public interface Commander
    {
        void Move(int x, int y, Direction dir, int robot);
        void Build(int x, int y, Terrain building);
        void Finish();
    }
}
