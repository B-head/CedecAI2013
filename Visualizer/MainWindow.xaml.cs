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
        MassInformation[,] mass;

        public MainWindow()
        {
            InitializeComponent();
            manager = new GameManager(new Random());
            manager.AI[0] = new TestAI();
            manager.AI[1] = new TestAI();
            manager.AI[2] = new TestAI();
            manager.Prepare();
            int w = manager.Field.Width, h = manager.Field.Height;
            mass = new MassInformation[w, h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    MassInformation temp = new MassInformation();
                    mass[x, y] = temp;
                    FieldInfo.Children.Add(temp);
                    Canvas.SetLeft(temp, (x + y / 2.0) * temp.Width);
                    Canvas.SetTop(temp, y * temp.Height);
                }
            }
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            Player1Info.Text = manager.GetPlayerInfo(0);
            Player2Info.Text = manager.GetPlayerInfo(1);
            Player3Info.Text = manager.GetPlayerInfo(2);
            GameField field = manager.Field;
            int w = field.Width, h = field.Height;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    mass[x, y].UpdateTerrain(field[x, y].Player, field[x, y].Ter);
                    mass[x, y].UpdateTextInfo(BuildMassInfo(x, y));
                }
            }
        }

        private string BuildMassInfo(int x, int y)
        {
            StringBuilder result = new StringBuilder();
            GameField field = manager.Field;
            if (ShowRobot.IsChecked == true)
            {
                result.AppendLine("Rb:" + field[x, y].WaitRobot.ToString());
            }
            if (ShowResource.IsChecked == true)
            {
                result.AppendLine("Rs:" + field.GetPrepareResource(x, y).ToString() + "(" + field.GetYieldResource(x, y).ToString() + ")");
            }
            if (ShowTowerDamage.IsChecked == true)
            {
                result.AppendLine("Td:" + field.GetEstimateTowerDamage(field[x, y].Player, x, y) + "(" + field.GetTowerDamage(manager.Player, x, y).ToString() + ")");
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
                Task.Run((Action)RunTurn);
                return;
            }
            NextButton.IsEnabled = true;
            AutoNext.IsChecked = false;
        }

        private void NextTurnHandler(object sender, RoutedEventArgs e)
        {
            if (!NextButton.IsEnabled) return;
            NextButton.IsEnabled = false;
            Task.Run((Action)RunTurn);
        }

        private void ShowChangeHandler(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
        }
    }
}
