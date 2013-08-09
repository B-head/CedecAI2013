using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedecAI
{
    struct GameMass
    {
        public int Player;
        public Terrain Ter;
        public int ActiveRobot;
        public int WaitRobot;
    }

    enum Terrain
    {
        Outside,
        Wasteland,
        Hole,
        Initial,
        RobotMaker,
        AttackTower,
        Excavator,
        Bridge,
        House,
        Town,
    }
}
