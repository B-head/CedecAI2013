using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Field<T>
    {
        protected T[,] field;
        public readonly int Width;
        public readonly int Height;

        public Field(int width, int height)
        {
            Width = width;
            Height = height;
            field = new T[width, height];
        }

        public T this[int x, int y]
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

        public T this[Point point]
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

        public void CopyTo(Field<T> other)
        {
            Array.Copy(field, other.field, Width * Height);
        }

        public bool IsInRange(Point point)
        {
            return point.X >= 0 && point.X < Width && point.Y >= 0 && point.Y < Height;
        }

        public static Point TransformDirection(int dir, Point point)
        {
            return TransformDirection((Direction)dir, point);
        }

        public static Point TransformDirection(Direction dir, Point point)
        {
            Point result = point;
            switch (dir)
            {
                case Direction.Center: result.X += 0; result.Y += 0; break;
                case Direction.Right: result.X += 1; result.Y += 0; break;
                case Direction.UpperRight: result.X += 1; result.Y += -1; break;
                case Direction.DownerRight: result.X += 0; result.Y += 1; break;
                case Direction.Left: result.X += -1; result.Y += 0; break;
                case Direction.DownerLeft: result.X += -1; result.Y += 1; break;
                case Direction.UpperLeft: result.X += 0; result.Y += -1; break;
                default: throw new Exception();
            }
            return result;
        }

        public static Point TransformTowerRange(int i, Point point)
        {
            Point result = point;
            switch (i)
            {
                case 0: result.X += 1; result.Y += 0; break;
                case 1: result.X += 1; result.Y += -1; break;
                case 2: result.X += 0; result.Y += 1; break;
                case 3: result.X += -1; result.Y += 0; break;
                case 4: result.X += -1; result.Y += 1; break;
                case 5: result.X += 0; result.Y += -1; break;
                case 6: result.X += 2; result.Y += 0; break;
                case 7: result.X += 2; result.Y += -2; break;
                case 8: result.X += 0; result.Y += 2; break;
                case 9: result.X += -2; result.Y += 0; break;
                case 10: result.X += -2; result.Y += 2; break;
                case 11: result.X += 0; result.Y += -2; break;
                default: throw new Exception();
            }
            return result;
        }

        public static Point TransformSitePropose(int i, Point point)
        {
            Point result = point;
            switch (i)
            {
                case 0: result.X += 1; result.Y += 0; break;
                case 1: result.X += 1; result.Y += 1; break;
                case 2: result.X += 0; result.Y += 1; break;
                case 3: result.X += -1; result.Y += 2; break;
                case 4: result.X += -1; result.Y += 1; break;
                case 5: result.X += -2; result.Y += 1; break;
                case 6: result.X += -1; result.Y += 0; break;
                case 7: result.X += -1; result.Y += -1; break;
                case 8: result.X += 0; result.Y += -1; break;
                case 9: result.X += 1; result.Y += -2; break;
                case 10: result.X += 1; result.Y += -1; break;
                case 11: result.X += 2; result.Y += -1; break;
                default: throw new Exception();
            }
            return result;
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
