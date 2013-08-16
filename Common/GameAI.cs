using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public interface GameAI
    {
        string Prepare(int player, GameField field);
        void Think(int turn, int maxTurn, int player, GameField field, Commander commander);
    }

    public class ColonizeAI : GameAI
    {
        DispositionRobot disposition;
        TownPlanning planning;
        Point InitialPoint;

        public string Prepare(int player, GameField field)
        {
            disposition = new DispositionRobot(player);
            InitialPoint = field.GetInitialPoint(player);
            planning = new TownPlanning(field);
            int maxVictory = planning.Max(p => p.VictoryPoint);
            var maxPlans = planning.Where(p => p.VictoryPoint == maxVictory);
            DistanceMap distance = new DistanceMap(field, player, InitialPoint);
            foreach (Point point in distance.NearIterator())
            {
                if (!maxPlans.Any(p => p.Excavator.Equals(point))) continue;
                TownPlan plan = maxPlans.First(p => p.Excavator.Equals(point));
                disposition.AddBuildOrder(300, plan.Excavator, Terrain.Excavator, false);
                foreach (Point town in plan.Town)
                {
                    disposition.AddBuildOrder(200, town, Terrain.Town, false);
                }
                break;
            }
            return "B_head:ColonizeAI";
        }

        public void Think(int turn, int maxTurn, int player, GameField field, Commander commander)
        {
            foreach (OrderBuild build in disposition.EnumerateOrder<OrderBuild>())
            {
                disposition.AddSecureResource(build.Priority + 1, build.X, build.Y, field);
            }
            DistanceMap distance = new DistanceMap(field, player, InitialPoint);
            foreach (Point point in field.Iterator())
            {
                if (field.GetPrepareResource(point.X, point.Y) < 4) continue;
                disposition.AddBuildOrder(-distance[point], point, Terrain.House, true);
            }
            disposition.AddSecureGround(100, 0, InitialPoint, field);
            disposition.Dispose(field, commander);
        }
    }

    public class KamikazeAI : GameAI
    {
        DispositionRobot disposition;

        public string Prepare(int player, GameField field)
        {
            disposition = new DispositionRobot(player);
            return "B_head:KamikazeAI";
        }

        public void Think(int turn, int maxTurn, int player, GameField field, Commander commander)
        {
        }
    }

    class InvadeAI : GameAI
    {
        public string Prepare(int player, GameField field)
        {
            throw new NotImplementedException();
        }

        public void Think(int turn, int maxTurn, int player, GameField field, Commander commander)
        {
            throw new NotImplementedException();
        }
    }

    class GovernAI : GameAI
    {
        public string Prepare(int player, GameField field)
        {
            throw new NotImplementedException();
        }

        public void Think(int turn, int maxTurn, int player, GameField field, Commander commander)
        {
            throw new NotImplementedException();
        }
    }

    class CompanyAI : GameAI
    {
        public string Prepare(int player, GameField field)
        {
            throw new NotImplementedException();
        }

        public void Think(int turn, int maxTurn, int player, GameField field, Commander commander)
        {
            throw new NotImplementedException();
        }
    }
}
