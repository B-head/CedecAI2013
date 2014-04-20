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
            var forbid = planning.ForbidPoint(new Point { X = 6, Y = 6 }, field);
            int max = forbid.Max(p => p.SpendRobot);
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
            if (field.GetPrepareResource(plan.Excavator, player, true) >= 4 && field[plan.Excavator].Terrain == Terrain.Wasteland)
            {
                disposition.AddBuildOrder(1000, plan.Excavator, Terrain.Excavator);
                disposition.AddMoveOrder(100, plan.Excavator, GameField.GetNeedRobot(Terrain.Excavator));
            }
            foreach (Point town in plan.Town)
            {
                if (field.GetPrepareResource(town, player, true) < 9) continue;
                disposition.AddBuildOrder(1000, town, Terrain.Town);
                disposition.AddMoveOrder(100, town, GameField.GetNeedRobot(Terrain.Town));
            }
            DistanceMap distance = new DistanceMap(field, player, InitialPoint);
            BuildPriority priority = new BuildPriority(field, player, InitialPoint, 99);
            foreach (Point point in field.Iterator())
            {
                if (field.GetPrepareResource(point, player, true) < 4) continue;
                if (!field.IsAdjoinTerritory(point, player)) continue;
                if (point == new Point { X = 6, Y = 6 }) continue;
                Terrain building;
                switch(field[point].Terrain)
                {
                    case Terrain.Wasteland: building = Terrain.House; break;
                    case Terrain.Hole: building = Terrain.Bridge; break;
                    default: continue;
                }
                disposition.AddBuildOrder(priority[point] * 10, point, building);
                disposition.AddMoveOrder(priority[point], point, GameField.GetNeedRobot(building));
            }
            foreach (OrderBuild build in disposition.EnumerateOrder<OrderBuild>())
            {
                disposition.AddSecureResource(build.Priority / 10 - 1, build.Point, player, field);
            }
            disposition.AddSecureGround(150, 100, player, field);
            disposition.AddKamikaze(0, player, field);
            disposition.Dispose(field, commander);
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
