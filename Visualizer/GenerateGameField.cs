using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Visualizer
{
    class GenerateGameField : GameField
    {
        public GenerateGameField(int size, Random random)
            : base(size)
        {
            Generate(random);
        }

        public GameField GetGameFieldView()
        {
            GameField result = new GameField(Size);
            CopyTo(result);
            return result;
        }

        public int GetTotalVictoryPoint(int player)
        {
            int result = 0;
            foreach(Point point in Iterator())
            {
                if (this[point].Player != player) continue;
                result += GetVictoryPoint(point);
            }
            return result;
        }

        public void StartTurn(int player)
        {
            foreach(Point point in Iterator())
            {
                GameMass temp = this[point];
                if (temp.Player == player)
                {
                    temp.WaitRobot += GetAddRobot(point);
                    temp.ActiveRobot += temp.WaitRobot;
                    temp.WaitRobot = 0;
                }
                else if (temp.WaitRobot > 0)
                {
                    temp.WaitRobot -= GetTowerDamage(point, player);
                    if (temp.WaitRobot < 0) temp.WaitRobot = 0;
                }
                this[point] = temp;
            }
        }

        public void EndTurn(int player)
        {
            foreach(Point point in Iterator())
            {
                GameMass temp = this[point];
                if (temp.Player != player) continue;
                temp.WaitRobot += temp.ActiveRobot;
                temp.ActiveRobot = 0;
                this[point] = temp;
            }
        }

        private void Generate(Random random)
        {
            int holeCount = RandomizeHole(random);
            SymmetryCopy();
            int tileCount = RandomizeInitial(random);
            if (holeCount > 15 || tileCount < (Size * (Size - 1) - holeCount) * 3 / 2)
            {
                Initialize();
                Generate(random);
            }
        }

        private int RandomizeHole(Random random)
        {
            int result = 0;
            for (int x = 0; x < Size; x++)
            {
                for (int y = Size; y < Height; y++)
                {
                    if (random.Next(5) == 0)
                    {
                        field[x, y].Terrain = Terrain.Hole;
                        result++;
                    }
                }
            }
            return result;
        }

        private void SymmetryCopy()
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = Size; y < Height; y++)
                {
                    field[y, FuncA(x, y)] = field[x, y];
                    field[FuncA(x, y), x] = field[x, y];
                }
            }
        }

        private int RandomizeInitial(Random random)
        {
            int x = random.Next(0, Size), y = random.Next(Size, Height);
            int[] player = { 0, 1, 2 };
            Shuffle(player, random);
            field[x, y].Terrain = Terrain.Initial;
            field[x, y].Player = player[0];
            field[y, FuncA(x, y)].Terrain = Terrain.Initial;
            field[y, FuncA(x, y)].Player = player[1];
            field[FuncA(x, y), x].Terrain = Terrain.Initial;
            field[FuncA(x, y), x].Player = player[2];
            return JoiningCount(new bool[Width, Height], new Point { X = x, Y = y });
        }

        private int FuncA(int x, int y)
        {
            return (Size - 1) * 3 - x - y;
        }

        private int JoiningCount(bool[,] settled, Point point)
        {
            if (!IsInRange(point)) return 0;
            if (this[point].Terrain == Terrain.Outside || this[point].Terrain == Terrain.Hole) return 0;
            if (settled[point.X, point.Y]) return 0;
            settled[point.X, point.Y] = true;
            int result = 1;
            foreach(Point temp in Adjoin(point))
            {
                result += JoiningCount(settled, temp);
            }
            return result;
        }

        private void Shuffle<T>(T[] array, Random rand)
        {
            for (int i = 0; i < array.Length; i++)
            {
                int a = rand.Next(i, array.Length);
                T temp = array[i];
                array[i] = array[a];
                array[a] = temp;
            }
        }
    }
}
