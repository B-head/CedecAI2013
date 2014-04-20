using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    static class AStar
    {
        public static Direction ApproachPoint(this GameField field, Point from, Point to)
        {
            SortedSet<RouteNode> open = new SortedSet<RouteNode>();
            RouteNode[,] nodeField = new RouteNode[field.Width, field.Height];
            int player = field[from].Player;
            RouteNode start = new RouteNode(from, to, 0, null);
            nodeField[from.X, from.Y] = start;
            open.Add(start);
            while (open.Count > 0)
            {
                RouteNode current = open.Min;
                open.Remove(current);
                current.Close = true;
                if (current.Point == to)
                {
                    return GetApproachingDirection(current);
                }
                if (!field.IsOutMove(current.Point)) continue;
                int goal = current.Goal + 1;
                foreach (Point point in field.Adjoin(current.Point))
                {
                    if (!field.IsInMove(point, field[point].Player == player)) continue;
                    RouteNode temp = nodeField[point.X, point.Y];
                    if (temp == null)
                    {
                        temp = new RouteNode(point, to, goal, current);
                        nodeField[point.X, point.Y] = temp;
                        open.Add(temp);
                        continue;
                    }
                    if (temp.Goal <= goal) continue;
                    if (!temp.Close) open.Remove(temp);
                    temp.Update(goal, current);
                    open.Add(temp);
                }
            }
            throw new Exception();
        }

        private static Direction GetApproachingDirection(RouteNode node)
        {
            Point prev = node.Point;
            while (node.Parent != null)
            {
                prev = node.Point;
                node = node.Parent;
            }
            for (int i = 0; i < 7; i++)
            {
                Point point = GameField.TransformDirection(i, node.Point);
                if (point == prev) return (Direction)i;
            }
            throw new Exception();
        }

        class RouteNode : IComparable<RouteNode>
        {
            public readonly Point Point;
            public readonly int Heuristic;
            public int Goal;
            public int Fitness;
            public RouteNode Parent;
            public bool Close;

            public RouteNode(Point from, Point to, int goal, RouteNode parent)
            {
                this.Point = from;
                Heuristic = HeuristicCost(from, to);
                Update(goal, parent);
            }

            private int HeuristicCost(Point from, Point to)
            {
                int x = from.X - to.X;
                int y = from.Y - to.Y;
                int a = Math.Max(Math.Abs(x), Math.Abs(y));
                int b = x + y;
                if (a > b)
                {
                    return a;
                }
                else
                {
                    return b;
                }
            }

            public void Update(int goal, RouteNode parent)
            {
                Goal = goal;
                Fitness = Goal + Heuristic;
                Parent = parent;
                Close = false;
            }

            public int CompareTo(RouteNode other)
            {
                int f = Fitness.CompareTo(other.Fitness);
                if (f != 0) return f;
                int x = Point.X - other.Point.X;
                if (x != 0) return x;
                return Point.Y - other.Point.Y;
            }
        }
    }
}
