using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;

namespace Visualizer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        GameManager manager;
        DistanceMap distance;
        BuildPriority priority;
        MassInformation[,] mass;
        int selectX;
        int selectY;

        public MainWindow()
        {
            InitializeComponent();
            manager = new GameManager(new Random());
            manager.AI[0] = new ColonizeAI();
            manager.AI[1] = new ColonizeAI();
            manager.AI[2] = new ColonizeAI();
            manager.Prepare();
            int w = manager.Field.Width, h = manager.Field.Height;
            mass = new MassInformation[w, h];
            selectX = w / 2;
            selectY = h / 2;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    MassInformation temp = new MassInformation();
                    temp.MouseDown += CreateMassSelectHandler(x, y);
                    mass[x, y] = temp;
                    FieldInfo.Children.Add(temp);
                    Canvas.SetLeft(temp, (x + y / 2.0) * temp.Width);
                    Canvas.SetTop(temp, y * temp.Height);
                }
            }
            UpdateInfo();
        }

        private MouseButtonEventHandler CreateMassSelectHandler(int x, int y)
        {
            return (sender, e) => 
            { 
                selectX = x;
                selectY = y;
                UpdateInfo();
            };
        }

        private void UpdateInfo()
        {
            TurnInfo.Text = manager.GetTurnInfo();
            Player1Info.Text = manager.GetPlayerInfo(0);
            Player2Info.Text = manager.GetPlayerInfo(1);
            Player3Info.Text = manager.GetPlayerInfo(2);
            distance = new DistanceMap(manager.Field, manager.Player, new Common.Point { X = selectX, Y = selectY });
            priority = new BuildPriority(manager.Field, manager.Player, new Common.Point { X = selectX, Y = selectY }, 255);
            GameField field = manager.Field;
            int w = field.Width, h = field.Height;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    mass[x, y].UpdateSelected(x == selectX && y == selectY);
                    mass[x, y].UpdateTerrain(field[x, y].Player, field[x, y].Terrain);
                    mass[x, y].UpdateTextInfo(BuildMassInfo(x, y));
                }
            }
        }

        private string BuildMassInfo(int x, int y)
        {
            StringBuilder result = new StringBuilder();
            GameField field = manager.Field;
            Common.Point point = new Common.Point { X = x, Y = y };
            if (ShowRobot.IsChecked == true)
            {
                result.AppendLine("Rb:" + field[x, y].WaitRobot.ToString());
            }
            if (ShowResource.IsChecked == true)
            {
                result.AppendLine("Rs:" + field.GetPrepareResource(point, manager.Player, true).ToString() + "(" + field.GetPrepareResource(point, manager.Player).ToString() + ")");
            }
            if (ShowTowerDamage.IsChecked == true)
            {
                result.AppendLine("Td:" + field.GetTowerDamage(point, manager.Player, true) + "(" + field.GetTowerDamage(point, manager.Player) + ")");
            }
            if (ShowDistanse.IsChecked == true)
            {
                result.AppendLine("Dt:" + distance[x, y].ToString());
            }
            if (ShowPriority.IsChecked == true)
            {
                result.AppendLine("Bp:" + priority[x, y].ToString());
            }
            return result.ToString();
        }

        private void RunTurn()
        {
            manager.NextTurn();
            Dispatcher.BeginInvoke((Action)CompleteTurn, System.Windows.Threading.DispatcherPriority.Background);
        }

        private void CompleteTurn()
        {
            UpdateInfo();
            if (AutoNext.IsChecked == true && !manager.IsGameOver())
            {
                Task task = new Task((Action)RunTurn);
                task.Start();
                return;
            }
            NextButton.IsEnabled = true;
            AutoNext.IsChecked = false;
        }

        private void NextTurnHandler(object sender, RoutedEventArgs e)
        {
            if (!NextButton.IsEnabled) return;
            NextButton.IsEnabled = false;
            Task task = new Task((Action)RunTurn);
            task.Start();
        }

        private void ShowChangeHandler(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
        }
    }
}
