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
            protected set
            {
                field[x, y] = value;
            }
        }

        public TYPE this[Point point]
        {
            get
            {
                return field[point.X, point.Y];
            }
            protected set
            {
                field[point.X, point.Y] = value;
            }
        }

        public void CopyTo(Field<TYPE> other)
        {
            Array.Copy(field, other.field, Width * Height);
        }

        public bool IsInRange(Point point)
        {
            return IsInRange(point.X, point.Y);
        }

        public bool IsInRange(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool TransformDirection(int dir, int x, int y, out int tx, out int ty)
        {
            return TransformDirection((Direction)dir, x, y, out tx, out ty);
        }

        public Point TransformDirection(int dir, Point from)
        {
            return TransformDirection((Direction)dir, from);
        }

        public Point TransformDirection(Direction dir, Point from)
        {
            Point result;
            TransformDirection((Direction)dir, from.X, from.Y, out result.X, out result.Y);
            return result;
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

        public Point TransformTowerRange(int i, Point from)
        {
            Point result;
            TransformTowerRange(i, from.X, from.Y, out result.X, out result.Y);
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

        public Point TransformSitePropose(int i, Point from)
        {
            Point result;
            TransformSitePropose(i, from.X, from.Y, out result.X, out result.Y);
            return result;
        }

        public bool TransformSitePropose(int i, int x, int y, out int tx, out int ty)
        {
            tx = x;
            ty = y;
            switch (i)
            {
                case 0: tx += 1; ty += 0; break;
                case 1: tx += 1; ty += 1; break;
                case 2: tx += 0; ty += 1; break;
                case 3: tx += -1; ty += 2; break;
                case 4: tx += -1; ty += 1; break;
                case 5: tx += -2; ty += 1; break;
                case 6: tx += -1; ty += 0; break;
                case 7: tx += -1; ty += -1; break;
                case 8: tx += 0; ty += -1; break;
                case 9: tx += 1; ty += -2; break;
                case 10: tx += 1; ty += -1; break;
                case 11: tx += 2; ty += -1; break;
                default: throw new Exception();
            }
            return !IsInRange(tx, ty);
        }

        public IEnumerable<Point> Adjoin(Point point, bool center = false)
        {
            for (int i = center ? 0 : 1; i < 7; i++)
            {
                Point temp = TransformDirection(i, point);
                if (!IsInRange(temp)) continue;
                yield return temp;
            }
        }

        public IEnumerable<Point> TowerRange(Point point)
        {
            for (int i = 0; i < 12; i++)
            {
                Point temp = TransformTowerRange(i, point);
                if (!IsInRange(temp)) continue;
                yield return temp;
            }
        }

        public IEnumerable<Point> SitePropose(Point point)
        {
            for (int i = 0; i < 12; i++)
            {
                Point temp = TransformSitePropose(i, point);
                if (!IsInRange(temp)) continue;
                yield return temp;
            }
        }

        public IEnumerable<Point> Iterator()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return new Point { X = x, Y = y };
                }
            }
        }
    }

}
