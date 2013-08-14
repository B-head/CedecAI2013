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

        public bool Move(int player, int fromX, int fromY, Direction dir, int robot)
        {
            if (!IsMove(player, fromX, fromY, dir, robot)) return false;
            int toX, toY;
            TransformDirection(dir, fromX, fromY, out toX, out toY);
            GameMass from = field[fromX, fromY], to = field[toX, toY];
            if (to.Player == player)
            {
                from.ActiveRobot -= robot;
                to.WaitRobot += robot;
            }
            else
            {
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

        public bool IsMove(int player, int fromX, int fromY, Direction dir, int robot)
        {
            if (robot <= 0) return false;
            int toX, toY;
            TransformDirection(dir, fromX, fromY, out toX, out toY);
            if (IsInRange(toX, toY)) return false;
            GameMass from = field[fromX, fromY], to = field[toX, toY];
            if (from.Player != player) return false;
            if (from.ActiveRobot < robot) return false;
            if (from.Ter == Terrain.Hole) return false;
            if (to.Player == player)
            {
                if (to.Ter == Terrain.Outside) return false;
            }
            else
            {
                if (to.Ter != Terrain.Wasteland && to.Ter != Terrain.Hole) return false;
            }
            return true;
        }

        public bool Build(int player, int x, int y, Terrain building, ref int extraPoint)
        {
            if (!IsBuild(player, x, y, building)) return false;
            int resource, robot;
            GetRequirement(building, out resource, out robot);
            if (building == Terrain.Town)
            {
                extraPoint += GetPrepareResource(x, y) - resource;
            }
            field[x, y].ActiveRobot -= robot;
            field[x, y].Ter = building;
            return true;
        }

        public bool IsBuild(int player, int x, int y, Terrain building)
        {
            if (field[x, y].Player != player) return false;
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
            for (int i = 1; i < 13; i++)
            {
                TransformTowerRange(i, x, y, out tx, out ty);
                if (IsInRange(tx, ty)) continue;
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
                TransformAdjoin(i, x, y, out tx, out ty);
                if (IsInRange(tx, ty)) continue;
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
                if (IsInRange(tx, ty)) continue;
                if (field[tx, ty].Player != player) continue;
                if (field[tx, ty].Ter != Terrain.Excavator) continue;
                result++;
            }
            return result;
        }

        public void TransformDirection(Direction dir, int x, int y, out int tx, out int ty)
        {
            tx = x;
            ty = y;
            switch (dir)
            {
                case Direction.Right: tx += 1; ty += 0; break;
                case Direction.UpperRight: tx += 1; ty += -1; break;
                case Direction.DownerRight: tx += 0; ty += 1; break;
                case Direction.Left: tx += -1; ty += 0; break;
                case Direction.DownerLeft: tx += -1; ty += 1; break;
                case Direction.UpperLeft: tx += 0; ty += -1; break;
                default: throw new Exception();
            }
        }

        public void TransformAdjoin(int i, int x, int y, out int tx, out int ty)
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
    }
}
