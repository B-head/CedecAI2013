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
                        field[x, y].Ter = Terrain.Hole;
                        result++;
                    }
                }
            }
            return result;
        }

        private void SymmetryCopy()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (field[x, y].Ter == Terrain.Outside) continue;
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
            field[x, y].Ter = Terrain.Initial;
            field[x, y].Player = player[0];
            field[y, FuncA(x, y)].Ter = Terrain.Initial;
            field[y, FuncA(x, y)].Player = player[1];
            field[FuncA(x, y), x].Ter = Terrain.Initial;
            field[FuncA(x, y), x].Player = player[2];
            return JoiningCount(new bool[Width, Height], x, y);
        }

        private int FuncA(int x, int y)
        {
            return (Size - 1) * 3 - x - y;
        }

        private int JoiningCount(bool[,] settled, int x, int y)
        {
            if (!IsInRange(x, y)) return 0;
            if (settled[x, y]) return 0;
            if (field[x, y].Ter == Terrain.Outside || field[x, y].Ter == Terrain.Hole) return 0;
            int result = 1, tx, ty;
            settled[x, y] = true;
            for (int i = 1; i < 7; i++)
            {
                TransformDirection(i, x, y, out tx, out ty);
                result += JoiningCount(settled, tx, ty);
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
