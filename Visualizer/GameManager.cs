using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Visualizer
{
    class GameManager
    {
        public GenerateGameField Field;
        public GameAI[] AI;
        public string[] Name;
        public int[] ExtraPoint;
        public int Player;
        public int Turn;
        public const int MaxTurn = 200;

        public GameManager(Random random)
        {
            Field = new GenerateGameField(7, random);
            AI = new GameAI[3];
            Name = new string[3];
            ExtraPoint = new int[3];
            Player = 0;
            Turn = 1;
        }

        public void Prepare()
        {
            for (int i = 0; i < 3; i++)
            {
                GameField temp = Field.GetGameFieldView();
                Name[i] = AI[i].Prepare(i, temp);
            }
        }

        public void NextTurn()
        {
            if (Turn > MaxTurn) return;
            Field.StartTurn(Player);
            GameField temp = Field.GetGameFieldView();
            ManagerCommander com = new ManagerCommander(this);
            AI[Player].Think(Turn, MaxTurn, Player, temp, com);
            Field.EndTurn(Player);
            if (!com.IsFinish) throw new Exception();
            if (++Player >= 3)
            {
                Player = 0;
                Turn++;
            }
        }

        public string GetPlayerInfo(int player)
        {
            return Name[0] + "\nVP:" + Field.GetTotalVictoryPoint(player).ToString() + "+" + ExtraPoint[player];
        }

        public bool IsGameOver()
        {
            return Turn > MaxTurn;
        }

        class ManagerCommander : Commander
        {
            GameManager parent;
            public bool IsMove;
            public bool IsBuild;
            public bool IsFinish;

            public ManagerCommander(GameManager parent)
            {
                this.parent = parent;
            }

            public void Move(int x, int y, Direction dir, int robot)
            {
                if (IsFinish || IsBuild) throw new Exception();
                if (!parent.Field.Move(parent.Player, x, y, dir, robot)) throw new Exception();
                IsMove = true;
            }

            public void Build(int x, int y, Terrain building)
            {
                if (IsFinish || IsBuild || IsMove) throw new Exception();
                if (!parent.Field.Build(parent.Player, x, y, building, ref parent.ExtraPoint[parent.Player])) throw new Exception();
                IsBuild = true;
            }

            public void Finish()
            {
                IsFinish = true;
            }
        }
    }
}
