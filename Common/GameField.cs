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
            foreach(Point point in Iterator())
            {
                GameMass temp = new GameMass();
                temp.Player = -1;
                if (IsWastelandInitalize(point))
                {
                    temp.Terrain = Terrain.Wasteland;
                }
                this[point] = temp;
            }
        }

        private bool IsWastelandInitalize(Point point)
        {
            if (point.Y < 0 || point.Y >= Height) return false;
            if (point.Y < Size)
            {
                if (point.X < Size - 1 - point.Y || point.X >= Width) return false;
            }
            else
            {
                if (point.X < 0 || point.X >= Width + Size - 1 - point.Y) return false;
            }
            return true;
        }

        public bool Move(Point from, Direction dir, int robot)
        {
            Point to = TransformDirection(dir, from);
            if (!IsMove(from, to, dir, robot)) return false;
            GameMass fromMass = this[from], toMass = this[to];
            if (from == to)
            {
                fromMass.ActiveRobot -= robot;
                fromMass.WaitRobot += robot;
                this[from] = fromMass;
                return true;
            }
            if (fromMass.Player == toMass.Player)
            {
                fromMass.ActiveRobot -= robot;
                toMass.WaitRobot += robot;
            }
            else
            {
                fromMass.ActiveRobot -= robot;
                if (toMass.WaitRobot < robot)
                {
                    toMass.WaitRobot = robot - toMass.WaitRobot;
                    toMass.Player = fromMass.Player;
                }
                else
                {
                    toMass.WaitRobot -= robot;
                }
            }
            this[from] = fromMass;
            this[to] = toMass;
            return true;
        }

        public bool IsMove(Point from, Direction dir, int robot)
        {
            Point to = TransformDirection(dir, from);
            return IsMove(from, to, dir, robot);
        }

        public bool IsMove(Point from, Point to, Direction dir, int robot)
        {
            if (robot <= 0) return false;
            if (!IsInRange(to)) return false;
            GameMass fromMass = this[from], toMass = this[to];
            if (fromMass.ActiveRobot < robot) return false;
            if (!IsOutMove(from) && dir != Direction.Center) return false;
            if (!IsInMove(to, fromMass.Player == toMass.Player)) return false;
            return true;
        }

        public bool IsInMove(Point point, bool have)
        {
            if (have)
            {
                return this[point].Terrain != Terrain.Outside;
            }
            else
            {
                return this[point].Terrain == Terrain.Wasteland || this[point].Terrain == Terrain.Hole;
            }
        }

        public bool IsOutMove(Point point)
        {
            return this[point].Terrain != Terrain.Hole;
        }

        public bool Build(Point point, Terrain building, ref int extraPoint)
        {
            if (!IsBuild(point, building)) return false;
            if (building == Terrain.Town)
            {
                extraPoint += GetPrepareResource(point, this[point].Player) - GetNeedResource(building);
                foreach(Point temp in Adjoin(point))
                {
                    if (this[point].Player != this[temp].Player) continue;
                    if (this[temp].Terrain != Terrain.Wasteland) continue;
                    GameMass tempMass = this[temp];
                    tempMass.Terrain = Terrain.House;
                    this[temp] = tempMass;
                }
            }
            GameMass mass = this[point];
            mass.ActiveRobot -= GetNeedRobot(building);
            mass.Terrain = building;
            this[point] = mass;
            return true;
        }

        public bool IsBuild(Point point, Terrain building)
        {
            if (!IsProperTerrain(point, building)) return false;
            if (this[point].ActiveRobot < GetNeedRobot(building)) return false;
            if (GetPrepareResource(point, this[point].Player) < GetNeedResource(building)) return false;
            return true;
        }

        public bool IsProperTerrain(Point point, Terrain building)
        {
            if (this[point].Terrain == Terrain.Wasteland)
            {
                return (int)building >= 4 && (int)building <= 8;
            }
            else if (this[point].Terrain == Terrain.Hole)
            {
                return building == Terrain.Bridge;
            }
            else
            {
                return false;
            }
        }

        public int GetNeedRobot(Terrain ter)
        {
            switch (ter)
            {
                case Terrain.RobotMaker: return 50;
                case Terrain.AttackTower: return 25;
                case Terrain.Excavator: return 25;
                case Terrain.Bridge: return 15;
                case Terrain.House: return 10;
                case Terrain.Town: return 10;
                default: throw new Exception();
            }
        }

        public int GetNeedResource(Terrain ter)
        {
            switch (ter)
            {
                case Terrain.RobotMaker: return 4;
                case Terrain.AttackTower: return 5;
                case Terrain.Excavator: return 4;
                case Terrain.Bridge: return 4;
                case Terrain.House: return 4;
                case Terrain.Town: return 9;
                default: throw new Exception();
            }
        }

        public int GetPrepareResource(Point point, int player, bool latent = false)
        {
            int result = 0;
            foreach(Point temp in Adjoin(point, true))
            {
                if (!latent && player != this[temp].Player) continue;
                result += GetYieldResource(temp, player);
            }
            return result;
        }

        public int GetYieldResource(Point point, int player)
        {
            if (this[point].Terrain != Terrain.Wasteland) return 0;
            int result = 1;
            foreach (Point temp in Adjoin(point))
            {
                if (player != this[temp].Player) continue;
                if (!IsExcavator(temp)) continue;
                result++;
            }
            return result;
        }

        public bool IsExcavator(Point point)
        {
            return this[point].Terrain == Terrain.Excavator;
        }

        public int GetVictoryPoint(Point point)
        {
            if (this[point].Terrain == Terrain.Wasteland)
            {
                return 1;
            }
            else if (this[point].Terrain == Terrain.Hole || this[point].Terrain == Terrain.Outside)
            {
                return 0;
            }
            else
            {
                return 3;
            }
        }

        public int GetAddRobot(Point point)
        {
            if (this[point].Terrain == Terrain.Initial)
            {
                return 5;
            }
            else if (this[point].Terrain == Terrain.RobotMaker)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int GetTowerDamage(Point point, int player, bool estimate = false)
        {
            if (!estimate && this[point].Player == player) return 0;
            int result = 0;
            foreach (Point temp in TowerRange(point))
            {
                if (this[temp].Terrain != Terrain.AttackTower) continue;
                if (this[temp].Player == player)
                {
                    result += estimate ? 2 : 0;
                }
                else
                {
                    result += estimate ? 0 : 2;
                }
            }
            return result;
        }
    }
}
