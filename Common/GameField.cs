using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CedecAI
{
    class GameField : Field<GameMass>
    {
        public readonly int Size;

        public GameField(int size)
            : base(size * 2 + 1, size * 2 + 1)
        {
            Size = size;
            Initialize();
        }

        private void Initialize()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    field[x, y].Player = -1;
                    if (y < 1 || y >= Height - 1) continue;
                    if (y <= Size)
                    {
                        if (x < Size - y + 1 || x >= Width - 1) continue;
                    }
                    else
                    {
                        if (x < 1 || x >= Width - 1 + Size - y) continue;
                    }
                    field[x, y].Ter = Terrain.Wasteland;
                }
            }
        }

        public static GameField ParseText(TextReader read, out int turn, out int maxTurn, out int playerTurn)
        {
            string[] line;
            line = read.ReadLine().Split(' ');
            turn = int.Parse(line[0]);
            maxTurn = int.Parse(line[1]);
            playerTurn = int.Parse(line[2]);
            line = read.ReadLine().Split(' ');
            GameField result = new GameField(int.Parse(line[0]));
            int count = int.Parse(line[1]);
            for (int i = 0; i < count; i++)
            {
                line = read.ReadLine().Split(' ');
                int x = int.Parse(line[0]), y = int.Parse(line[1]), player = int.Parse(line[2]), robot = int.Parse(line[3]);
                Terrain ter = ParseTopography(line[5], line[6]);
                result.field[x, y] = new GameMass { Player = player, Ter = ter, WaitRobot = robot };
            }
            return result;
        }

        private static Terrain ParseTopography(string ter, string building)
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

        public void SetActive(int player)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (field[x, y].Player != player) continue;
                    field[x, y].ActiveRobot += field[x, y].WaitRobot;
                    field[x, y].WaitRobot = 0;
                }
            }
        }

        public void SetWait(int player)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (field[x, y].Player != player) continue;
                    field[x, y].WaitRobot += field[x, y].ActiveRobot;
                    field[x, y].ActiveRobot = 0;
                }
            }
        }

        public void TowerAttacking(int player)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (field[x, y].Player == player) continue;
                    if (field[x, y].WaitRobot <= 0) continue;
                    field[x, y].WaitRobot -= GetTowerDamage(x, y);
                }
            }
        }

        public bool Move(int player, int fromX, int fromY, int toX, int toY, int robot)
        {
            GameMass from = field[fromX, fromY], to = field[toX, toY];
            if (from.Player != player) return false;
            if (from.ActiveRobot < robot) return false;
            if (from.Ter == Terrain.Hole) return false;
            if (to.Player == player)
            {
                if (to.Ter == Terrain.Outside) return false;
                from.ActiveRobot -= robot;
                to.WaitRobot += robot;
            }
            else
            {
                if (to.Ter != Terrain.Wasteland && to.Ter != Terrain.Hole) return false;
                from.ActiveRobot -= robot;
                if (to.WaitRobot < robot)
                {
                    to.WaitRobot = robot - to.WaitRobot;
                    to.Player = player;
                }
                else
                {
                    to.WaitRobot -= robot;
                }
            }
            field[fromX, fromY] = from;
            field[toX, toY] = to;
            return true;
        }

        public bool Build(int player, int x, int y, Terrain ter, ref int extraPoint)
        {
            if (field[x, y].Player != player) return false;
            if (field[x, y].Ter == Terrain.Wasteland && ter == Terrain.Bridge) return false;
            if (field[x, y].Ter == Terrain.Hole && ter != Terrain.Bridge) return false;
            int resource, robot;
            GetRequirement(ter, out resource, out robot);
            if (field[x, y].ActiveRobot < robot) return false;
            int pr = GetPrepareResource(x, y);
            if (pr < resource) return false;
            if (ter == Terrain.Town)
            {
                extraPoint += pr - resource;
            }
            field[x, y].ActiveRobot -= robot;
            field[x, y].Ter = ter;
            return true;
        }

        public void GetRequirement(Terrain ter, out int resource, out int robot)
        {
            switch (ter)
            {
                case Terrain.RobotMaker: resource = 4; robot = 50; break;
                case Terrain.AttackTower: resource = 5; robot = 25; break;
                case Terrain.Excavator: resource = 4; robot = 25; break;
                case Terrain.Bridge: resource = 4; robot = 15; break;
                case Terrain.House: resource = 4; robot = 10; break;
                case Terrain.Town: resource = 9; robot = 10; break;
                default: throw new Exception();
            }
        }

        public int GetPrepareResource(int x, int y)
        {
            int result = 0, player = field[x, y].Player, tx, ty;
            for (int i = 0; i < 7; i++)
            {
                TransformAdjoin(i, x, y, out tx, out ty);
                if (field[tx, ty].Player != player) continue;
                result += GetYieldResource(tx, ty);
            }
            return result;
        }

        public int GetYieldResource(int x, int y)
        {
            if (field[x, y].Ter != Terrain.Wasteland) return 0;
            int result = 1, player = field[x, y].Player, tx, ty;
            for (int i = 1; i < 7; i++)
            {
                TransformAdjoin(i, x, y, out tx, out ty);
                if (field[tx, ty].Player != player) continue;
                if (field[tx, ty].Ter != Terrain.Excavator) continue;
                result++;
            }
            return result;
        }

        public int GetTowerDamage(int x, int y)
        {
            int result = 0, player = field[x, y].Player, tx, ty;
            for (int i = 1; i < 13; i++)
            {
                TransformTowerRange(i, x, y, out tx, out ty);
                if (field[tx, ty].Ter != Terrain.AttackTower) continue;
                if (field[tx, ty].Player == player) continue;
                result += 2;
            }
            return result;
        }

        public void TransformAdjoin(int i, int x, int y, out int tx, out int ty)
        {
            tx = x;
            ty = y;
            switch (i)
            {
                case 0: tx += 0; ty += 0; break;
                case 1: tx += 1; ty += 0; break;
                case 2: tx += 1; ty += 1; break;
                case 3: tx += 0; ty += 1; break;
                case 4: tx += -1; ty += 0; break;
                case 5: tx += -1; ty += -1; break;
                case 6: tx += 0; ty += -1; break;
                default: throw new Exception();
            }
        }

        public void TransformTowerRange(int i, int x, int y, out int tx, out int ty)
        {
            tx = x;
            ty = y;
            switch (i)
            {
                case 0: tx += 0; ty += 0; break;
                case 1: tx += 1; ty += 0; break;
                case 2: tx += 1; ty += -1; break;
                case 3: tx += 0; ty += 1; break;
                case 4: tx += -1; ty += 0; break;
                case 5: tx += -1; ty += 1; break;
                case 6: tx += 0; ty += -1; break;
                case 7: tx += 2; ty += 0; break;
                case 8: tx += 2; ty += -2; break;
                case 9: tx += 0; ty += 2; break;
                case 10: tx += -2; ty += 0; break;
                case 11: tx += -2; ty += 2; break;
                case 12: tx += 0; ty += -2; break;
                default: throw new Exception();
            }
        }

        public void ToRedress(ref int x, ref int y)
        {
            x -= Size;
            y -= Size;
        }

        public void FromRedress(ref int x, ref int y)
        {
            x += Size;
            y += Size;
        }
    }
}
