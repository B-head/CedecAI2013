using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class TestAI : GameAI
    {
        Random random;

        public TestAI()
        {
            random = new Random(0);
        }

        public string Prepare(int player, GameField field)
        {
            return "B_head:TestAI";
        }

        public void Think(int turn, int maxTurn, int player, GameField field, Commander commander)
        {
            if (!Build(player, field, commander))
            {
                Move(player, field, commander);
            }
            commander.Finish();
        }

        private void Move(int player, GameField field, Commander commander)
        {
            int w = field.Width, h = field.Height;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Direction dir = ToDirection(random.Next(6));
                    int robot = random.Next(field[x, y].ActiveRobot) + 1;
                    if (field.IsMove(x, y, dir, robot))
                    {
                        commander.Move(x, y, dir, robot);
                    }
                }
            }
        }

        private bool Build(int player, GameField field, Commander commander)
        {
            Terrain building = ToBuilding(random.Next(6));
            int w = field.Width, h = field.Height;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (field.IsBuild(x, y, building))
                    {
                        commander.Build(x, y, building);
                        return true;
                    }
                }
            }
            return false;
        }

        private Direction ToDirection(int i)
        {
            switch (i)
            {
                case 0: return Direction.DownerLeft;
                case 1: return Direction.DownerRight;
                case 2: return Direction.Left;
                case 3: return Direction.Right;
                case 4: return Direction.UpperLeft;
                case 5: return Direction.UpperRight;
                default: throw new Exception();
            }
        }

        private Terrain ToBuilding(int i)
        {
            switch (i)
            {
                case 0: return Terrain.RobotMaker;
                case 1: return Terrain.AttackTower;
                case 2: return Terrain.Excavator;
                case 3: return Terrain.Bridge;
                case 4: return Terrain.House;
                case 5: return Terrain.Town;
                default: throw new Exception();
            }
        }
    }
}
