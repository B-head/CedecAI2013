using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class BuildPriority : Field<byte>
    {
        public readonly Point Focus;
        public readonly int MaxPriority;

        public BuildPriority(GameField gameField, int player, Point focus, int maxPriority)
            : base(gameField.Width, gameField.Height)
        {
            Focus = focus;
            MaxPriority = maxPriority;
            SearchPriority(gameField, player);
        }

        private void SearchPriority(GameField gameField, int player)
        {
            DistanceMap distance = new DistanceMap(gameField, player, Focus);
            SortedSet<BuildResource> open = new SortedSet<BuildResource>();
            BuildResource[,] resourceField = new BuildResource[gameField.Width, gameField.Height];
            foreach (Point point in gameField.Iterator())
            {
                if (gameField[point].Terrain != Terrain.Wasteland && gameField[point].Terrain != Terrain.Hole) continue;
                BuildResource temp = new BuildResource(point, gameField[point].Terrain == Terrain.Hole, distance[point], gameField.GetPrepareResource(point, player, true));
                open.Add(temp);
                resourceField[point.X, point.Y] = temp;
            }
            int nextPriority = MaxPriority;
            while (open.Count > 0)
            {
                BuildResource current = open.Min;
                open.Remove(current);
                if (current.Resource < 4) continue;
                this[current.Point] = (byte)nextPriority--;
                if(nextPriority <= 0) break;
                if (current.Hole) continue;
                foreach (Point point in Adjoin(current.Point))
                {
                    BuildResource temp = resourceField[point.X, point.Y];
                    if (temp == null) continue;
                    if (!open.Remove(temp)) continue;
                    temp.Resource--;
                    open.Add(temp);
                }
            }
        }

        class BuildResource : IComparable<BuildResource>
        {
            public readonly Point Point;
            public readonly bool Hole;
            public readonly int Distance;
            public int Resource;

            public BuildResource(Point point, bool hole, int distance, int resource)
            {
                this.Point = point;
                Hole = hole;
                Distance = distance;
                Resource = resource;
            }

            public int CompareTo(BuildResource other)
            {
                int r = Resource.CompareTo(other.Resource);
                if (r != 0) return r;
                if (!Hole && other.Hole) return 1;
                if (Hole && !other.Hole) return -1;
                int d = Distance.CompareTo(other.Distance);
                if (d != 0) return d;
                int x = Point.X - other.Point.X;
                if (x != 0) return x;
                return Point.Y - other.Point.Y;
            }
        }
    }
}
