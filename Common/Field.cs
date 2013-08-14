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
            return x < 0 || x >= Width || y < 0 || y >= Height;
        }

        public void CopyTo(Field<TYPE> other)
        {
            Array.Copy(field, other.field, Width * Height);
        }
    }
}
