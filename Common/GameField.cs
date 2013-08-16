using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Common
{
    public class GameField : Field<GameMass>
    {
        public readonly int Size;

        public GameField(int size)
            : base(size * 2 - 1, size * 2 - 1)
        {
            Size = size;
            Initialize();
        }

        protected void Initialize()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    field[x, y].Player = -1;
                    if (y < 0 || y >= Height) continue;
                    if (y < Size)
                    {
                        if (x < Size - 1 - y || x >= Width) continue;
                    }
                    else
                    {
                        if (x < 0 || x >= Width + Size - 1 - y) continue;
                    }
                    field[x, y].Ter = Terrain.Wasteland;
                }
            }
        }

        public bool Move(int fromX, int fromY, Direction dir, int robot)
        {
            if (!IsMove(fromX, fromY, dir, robot)) return false;
            int toX, toY;
            TransformDirection(dir, fromX, fromY, out toX, out toY);
            int player = field[fromX, fromY].Player;
            if (field[toX, toY].Player == player)
            {
                field[fromX, fromY].ActiveRobot -= robot;
                field[toX, toY].WaitRobot += robot;
            }
            else
            {
                field[fromX, fromY].ActiveRobot -= robot;
                if (field[toX, toY].WaitRobot < robot)
                {
                    field[toX, toY].WaitRobot = robot - field[toX, toY].WaitRobot;
                    field[toX, toY].Player = player;
                }
                else
                {
                    field[toX, toY].WaitRobot -= robot;
                }
            }
            return true;
        }

        public bool IsMove(int fromX, int fromY, Direction dir, int robot)
        {
            if (robot <= 0) return false;
            int toX, toY;
            if (TransformDirection(dir, fromX, fromY, out toX, out toY)) return false;
            int player = field[fromX, fromY].Player;
            if (field[fromX, fromY].ActiveRobot < robot) return false;
            if (field[fromX, fromY].Ter == Terrain.Hole) return false;
            if (field[toX, toY].Player == player)
            {
                if (field[toX, toY].Ter == Terrain.Outside) return false;
            }
            else
            {
                if (field[toX, toY].Ter != Terrain.Wasteland && field[toX, toY].Ter != Terrain.Hole) return false;
            }
            return true;
        }

        public bool Build(int x, int y, Terrain building, ref int extraPoint)
        {
            if (!IsBuild(x, y, building)) return false;
            int resource, robot;
            GetRequirement(building, out resource, out robot);
            if (building == Terrain.Town)
            {
                extraPoint += GetPrepareResource(x, y) - resource;
                int tx, ty, player = field[x,y].Player;
                for (int i = 1; i < 7; i++)
                {
                    if (TransformDirection(i, x, y, out tx, out ty)) continue;
                    if (field[tx, ty].Player != player) continue;
                    if (field[tx, ty].Ter != Terrain.Wasteland) continue;
                    field[tx, ty].Ter = Terrain.House;
                }
            }
            field[x, y].ActiveRobot -= robot;
            field[x, y].Ter = building;
            return true;
        }

        public bool IsBuild(int x, int y, Terrain building)
        {
            if (field[x, y].Ter != Terrain.Wasteland && field[x, y].Ter != Terrain.Hole) return false;
            if (field[x, y].Ter == Terrain.Wasteland && building == Terrain.Bridge) return false;
            if (field[x, y].Ter == Terrain.Hole && building != Terrain.Bridge) return false;
            int resource, robot;
            GetRequirement(building, out resource, out robot);
            if (field[x, y].ActiveRobot < robot) return false;
            if (GetPrepareResource(x, y) < resource) return false;
            return true;
        }

        public void StartTurn(int player)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (field[x, y].Player == player)
                    {
                        field[x, y].WaitRobot += GetAddRobot(x, y);
                        field[x, y].ActiveRobot += field[x, y].WaitRobot;
                        field[x, y].WaitRobot = 0;
                    }
                    else
                    {
                        if (field[x, y].WaitRobot <= 0) continue;
                        field[x, y].WaitRobot -= GetTowerDamage(player, x, y);
                        if (field[x, y].WaitRobot < 0) field[x, y].WaitRobot = 0;
                    }
                }
            }
        }

        public void EndTurn(int player)
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

        public int GetTotalVictoryPoint(int player)
        {
            int result = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (field[x, y].Player != player) continue;
                    result += GetVictoryPoint(x, y);
                }
            }
            return result;
        }

        public int GetVictoryPoint(int x, int y)
        {
            if (field[x, y].Ter == Terrain.Wasteland)
            {
                return 1;
            }
            else if (field[x, y].Ter == Terrain.Hole || field[x, y].Ter == Terrain.Outside)
            {
                return 0;
            }
            else
            {
                return 3;
            }
        }

        public int GetAddRobot(int x, int y)
        {
            if (field[x, y].Ter == Terrain.Initial)
            {
                return 5;
            }
            else if (field[x, y].Ter == Terrain.RobotMaker)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int GetTowerDamage(int player, int x, int y)
        {
            if (field[x, y].Player == player) return 0;
            int result = 0, tx, ty;
            for (int i = 0; i < 12; i++)
            {
                if (TransformTowerRange(i, x, y, out tx, out ty)) continue;
                if (field[tx, ty].Ter != Terrain.AttackTower) continue;
                if (field[tx, ty].Player != player) continue;
                result += 2;
            }
            return result;
        }

        public int GetPrepareResource(int x, int y)
        {
            int result = 0, player = field[x, y].Player, tx, ty;
            for (int i = 0; i < 7; i++)
            {
                if (TransformDirection(i, x, y, out tx, out ty)) continue;
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
                if (TransformDirection(i, x, y, out tx, out ty)) continue;
                if (field[tx, ty].Player != player) continue;
                if (field[tx, ty].Ter != Terrain.Excavator) continue;
                result++;
            }
            return result;
        }

        public bool TransformTowerRange(int i, int x, int y, out int tx, out int ty)
        {
            tx = x;
            ty = y;
            switch (i)
            {
                case 0: tx += 1; ty += 0; break;
                case 1: tx += 1; ty += -1; break;
                case 2: tx += 0; ty += 1; break;
                case 3: tx += -1; ty += 0; break;
                case 4: tx += -1; ty += 1; break;
                case 5: tx += 0; ty += -1; break;
                case 6: tx += 2; ty += 0; break;
                case 7: tx += 2; ty += -2; break;
                case 8: tx += 0; ty += 2; break;
                case 9: tx += -2; ty += 0; break;
                case 10: tx += -2; ty += 2; break;
                case 11: tx += 0; ty += -2; break;
                default: throw new Exception();
            }
            return !IsInRange(tx, ty);
        }
    }
}
