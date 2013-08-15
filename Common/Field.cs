using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Field<TYPE>
    {
        protected TYPE[,] field;
        public readonly int Width;
        public readonly int Height;

        public Field(int width, int height)
        {
            Width = width;
            Height = height;
            field = new TYPE[width, height];
        }

        public TYPE this[int x, int y]
        {
            get
            {
                return field[x, y];
            }
        }

        public bool IsInRange(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void CopyTo(Field<TYPE> other)
        {
            Array.Copy(field, other.field, Width * Height);
        }

        public bool TransformDirection(int dir, int x, int y, out int tx, out int ty)
        {
            return TransformDirection((Direction)dir, x, y, out tx, out ty);
        }

        public bool TransformDirection(Direction dir, int x, int y, out int tx, out int ty)
        {
            tx = x;
            ty = y;
            switch (dir)
            {
                case Direction.Center: tx += 0; ty += 0; break;
                case Direction.Right: tx += 1; ty += 0; break;
                case Direction.UpperRight: tx += 1; ty += -1; break;
                case Direction.DownerRight: tx += 0; ty += 1; break;
                case Direction.Left: tx += -1; ty += 0; break;
                case Direction.DownerLeft: tx += -1; ty += 1; break;
                case Direction.UpperLeft: tx += 0; ty += -1; break;
                default: throw new Exception();
            }
            return !IsInRange(tx, ty);
        }
    }
}
