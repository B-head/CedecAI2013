using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedecAI
{
    class Field<TYPE>
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
    }
}
