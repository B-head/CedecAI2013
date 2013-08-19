using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public interface IGameAI
    {
        string Prepare(int player, GameField field);
        void Think(int turn, int maxTurn, int player, GameField field, ICommander commander);
    }

    public class ColonizeAI : IGameAI
    {
        DispositionRobot disposition;
        TownPlanning planning;
        TownPlan plan;
        Point InitialPoint;

        public string Prepare(int player, GameField field)
        {
            disposition = new DispositionRobot(player);
            InitialPoint = field.GetInitialPoint(player);
            planning = new TownPlanning(field);
            int max = planning.Max(p => p.SpendRobot);
            var maxPlans = planning.Where(p => p.SpendRobot == max);
            foreach (Point point in field.NearIterator(InitialPoint, player))
            {
                if (!maxPlans.Any(p => p.Excavator == point)) continue;
                plan = maxPlans.First(p => p.Excavator == point);
                break;
            }
            return "B_head:Colonize";
        }

        public void Think(int turn, int maxTurn, int player, GameField field, ICommander commander)
        {
            DistanceMap distance = new DistanceMap(field, player, InitialPoint);
            Point fieldCenter = new Point { X = 6, Y = 6 };
            if (field.GetPrepareResource(plan.Excavator, player, true) >= 4 && plan.Excavator != fieldCenter)
            {
                disposition.AddBuildOrder(200, plan.Excavator, Terrain.Excavator, true);
            }
            foreach (Point town in plan.Town)
            {
                if (field.GetPrepareResource(town, player, true) < 9) continue;
                if (town == fieldCenter) continue;
                disposition.AddBuildOrder(200, town, Terrain.Town, true);
            }
            foreach (Point point in field.NearIterator(InitialPoint, player))
            {
                if (field.GetPrepareResource(point, player, true) < 4) continue;
                if (field[point].Terrain == Terrain.Hole)
                {
                    disposition.AddBuildOrder(100 - distance[point], point, Terrain.Bridge, true);
                }
                else if (field[point].Terrain == Terrain.Wasteland)
                {
                    disposition.AddBuildOrder(100 - distance[point], point, Terrain.House, true);
                }
                else
                {
                    continue;
                }
                break;
            }
            foreach (OrderBuild build in disposition.EnumerateOrder<OrderBuild>())
            {
                disposition.AddSecureResource(build.Priority - 1, build.Point, field);
            }
            disposition.AddSecureGround(0, 0, InitialPoint, field);
            disposition.Dispose(field, commander);
        }
    }

    public class KamikazeAI : IGameAI
    {
        public string Prepare(int player, GameField field)
        {
            throw new NotImplementedException();
        }

        public void Think(int turn, int maxTurn, int player, GameField field, ICommander commander)
        {
            throw new NotImplementedException();
        }
    }

    public class InvadeAI : IGameAI
    {
        public string Prepare(int player, GameField field)
        {
            throw new NotImplementedException();
        }

        public void Think(int turn, int maxTurn, int player, GameField field, ICommander commander)
        {
            throw new NotImplementedException();
        }
    }

    public class GovernAI : IGameAI
    {
        public string Prepare(int player, GameField field)
        {
            throw new NotImplementedException();
        }

        public void Think(int turn, int maxTurn, int player, GameField field, ICommander commander)
        {
            throw new NotImplementedException();
        }
    }

    public class CompanyAI : IGameAI
    {
        public string Prepare(int player, GameField field)
        {
            throw new NotImplementedException();
        }

        public void Think(int turn, int maxTurn, int player, GameField field, ICommander commander)
        {
            throw new NotImplementedException();
        }
    }

    public class TestAI : IGameAI
    {
        Random random;

        public string Prepare(int player, GameField field)
        {
            random = new Random(0);
            return "B_head:Test";
        }

        public void Think(int turn, int maxTurn, int player, GameField field, ICommander commander)
        {
            if (!Build(player, field, commander))
            {
                Move(player, field, commander);
            }
            commander.Finish();
        }

        private void Move(int player, GameField field, ICommander commander)
        {
            foreach (Point point in field.Iterator())
            {
                Direction dir = (Direction)random.Next(1, 7);
                int robot = random.Next(field[point].ActiveRobot) + 1;
                if (field.IsMove(point, dir, robot))
                {
                    commander.Move(point.X, point.Y, dir, robot);
                }
            }
        }

        private bool Build(int player, GameField field, ICommander commander)
        {
            Terrain building = (Terrain)random.Next(4, 10);
            foreach (Point point in field.Iterator())
            {
                if (field.IsBuild(point, building))
                {
                    commander.Build(point.X, point.Y, building);
                    return true;
                }
            }
            return false;
        }
    }
}
