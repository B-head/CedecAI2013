using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class TownPlanning : IEnumerable<TownPlan>
    {
        List<TownPlan> plan;

        public TownPlanning(GameField field)
        {
            plan = new List<TownPlan>();
            foreach (Point point in field.Iterator())
            {
                if (field[point].Terrain != Terrain.Wasteland) continue;
                SearchPlan(field, point, 0, new int[0]);
            }
            foreach (TownPlan tp in plan)
            {
                TestTownPlan(field, tp);
            }
        }

        private void SearchPlan(GameField field, Point excavator, int dir, int[] townDir)
        {
            for (int i = dir; i < 12; i++)
            {
                Point town = GameField.TransformSitePropose(i, excavator);
                if (!field.IsInRange(town)) continue;
                if (!IsPlanLiberate(i, townDir)) continue;
                if (!field.IsLiberate(town)) continue;
                int[] temp = new int[townDir.Length + 1];
                townDir.CopyTo(temp, 0);
                temp[temp.Length - 1] = i;
                SearchPlan(field, excavator, i + 1, temp);
            }
            if (townDir.Length > 0)
            {
                plan.Add(ToTownPlan(field, excavator, townDir));
            }
        }

        //なんという俺ルールなコードｗ
        private bool IsPlanLiberate(int i, int[] townDir)
        {
            foreach (int temp in townDir)
            {
                if (i % 2 == 1 && temp % 2 == 1)
                {
                    if (Math.Abs(temp - i) < 4 || Math.Abs(temp + 12 - i) < 4) return false;
                }
                else
                {
                    if (Math.Abs(temp - i) < 5 || Math.Abs(temp + 12 - i) < 5) return false;
                }
            }
            return true;
        }

        private TownPlan ToTownPlan(GameField field, Point excavator, int[] townDir)
        {
            Point[] town = new Point[townDir.Length];
            int victory = 3, spend = 25;
            for (int i = 0; i < townDir.Length; i++)
            {
                Point temp = GameField.TransformSitePropose(townDir[i], excavator);
                town[i] = temp;
                spend += 10;
                victory += townDir[i] % 2 == 0 ? 18 : 21;
            }
            return new TownPlan(victory, spend, excavator, Array.AsReadOnly(town));
        }

        private void TestTownPlan(GameField field, TownPlan tp)
        {
            bool[,] test = new bool[field.Width,field.Height];
            foreach (Point town in tp.Town)
            {
                foreach (Point point in field.Adjoin(town))
                {
                    if (point.Equals(tp.Excavator)) continue;
                    if (test[point.X, point.Y]) throw new Exception();
                    test[point.X, point.Y] = true;
                }
            }
        }

        public IEnumerable<TownPlan> ForbidPoint(Point point, GameField field)
        {
            List<TownPlan> result = new List<TownPlan>();
            foreach (TownPlan p in plan)
            {
                if (p.Excavator == point) continue;
                if (!IsForbidPointInTown(p.Town, point, field)) continue;
                result.Add(p);
            }
            return result;
        }

        private bool IsForbidPointInTown(ReadOnlyCollection<Point> towns, Point point, GameField field)
        {
            foreach (Point town in towns)
            {
                foreach (Point temp in field.Adjoin(town, true))
                {
                    if(temp == point) return false;
                }
            }
            return true;
        }

        public IEnumerator<TownPlan> GetEnumerator()
        {
            return plan.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
