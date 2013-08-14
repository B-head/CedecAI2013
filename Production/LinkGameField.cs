using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common;

namespace Production
{
    class LinkGameField : GameField
    {
        public LinkGameField(int size)
            : base(size)
        {

        }

        public static LinkGameField ParseText(out int turn, out int maxTurn, out int playerTurn)
        {
            string[] line;
            if (Console.ReadLine() != "START") throw new Exception();
            line = Console.ReadLine().Split(' ');
            turn = int.Parse(line[0]);
            maxTurn = int.Parse(line[1]);
            playerTurn = int.Parse(line[2]);
            line = Console.ReadLine().Split(' ');
            LinkGameField result = new LinkGameField(int.Parse(line[0]));
            int count = int.Parse(line[1]);
            for (int i = 0; i < count; i++)
            {
                line = Console.ReadLine().Split(' ');
                int x = int.Parse(line[0]), y = int.Parse(line[1]), player = int.Parse(line[2]), robot = int.Parse(line[3]);
                result.FromRedress(ref x, ref y);
                Terrain ter = ParseTerrain(line[5], line[6]);
                if (player == playerTurn)
                {
                    result.field[x, y] = new GameMass { Player = player, Ter = ter, ActiveRobot = robot };
                }
                else
                {
                    result.field[x, y] = new GameMass { Player = player, Ter = ter, WaitRobot = robot };
                }
            }
            if (Console.ReadLine() != "EOS") throw new Exception();
            return result;
        }

        private static Terrain ParseTerrain(string ter, string building)
        {
            switch (ter)
            {
                case "wasteland":
                case "settlement":
                    return Terrain.Wasteland;
                case "base":
                    switch (building)
                    {
                        case "initial": return Terrain.Initial;
                        case "robotmaker": return Terrain.RobotMaker;
                        case "tower": return Terrain.AttackTower;
                        case "excavator": return Terrain.Excavator;
                        case "bridge": return Terrain.Bridge;
                        case "house": return Terrain.House;
                        case "town": return Terrain.Town;
                        default: throw new Exception();
                    }
                case "hole":
                    return Terrain.Hole;
                default:
                    throw new Exception();
            }
        }

        public Commander GetCommander()
        {
            return new LinkCommander(this);
        }

        private void ToRedress(ref int x, ref int y)
        {
            x -= Size - 1;
            y -= Size - 1;
        }

        private void FromRedress(ref int x, ref int y)
        {
            x += Size - 1;
            y += Size - 1;
        }

        class LinkCommander : Commander
        {
            LinkGameField parent;

            public LinkCommander(LinkGameField parent)
            {
                this.parent = parent;
            }

            public void Move(int x, int y, Direction dir, int robot)
            {
                string temp = GenerateDirectionCode(dir);
                parent.ToRedress(ref x, ref y);
                Console.WriteLine("move {0} {1} {2} {3}", x, y, temp, robot);
            }

            public void Build(int x, int y, Terrain building)
            {
                string temp = GenerateBuildingCode(building);
                parent.ToRedress(ref x, ref y);
                Console.WriteLine("build {0} {1} {2}", x, y, temp);
            }

            public void Finish()
            {
                Console.WriteLine("finish");
            }

            private string GenerateDirectionCode(Direction dir)
            {
                switch (dir)
                {
                    case Direction.Right: return "r";
                    case Direction.UpperRight: return "ur";
                    case Direction.DownerRight: return "dr";
                    case Direction.Left: return "l";
                    case Direction.UpperLeft: return "ul";
                    case Direction.DownerLeft: return "dl";
                    default: throw new Exception();
                }
            }

            private string GenerateBuildingCode(Terrain building)
            {
                switch (building)
                {
                    case Terrain.Initial: return "initial";
                    case Terrain.RobotMaker: return "robotmaker";
                    case Terrain.AttackTower: return "tower";
                    case Terrain.Excavator: return "excavator";
                    case Terrain.Bridge: return "bridge";
                    case Terrain.House: return "house";
                    case Terrain.Town: return "town";
                    default: throw new Exception();
                }
            }
        }
    }
}
