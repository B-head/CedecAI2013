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
    /// MassInformation.xaml の相互作用ロジック
    /// </summary>
    public partial class MassInformation : UserControl
    {
        static readonly BitmapImage[] hole;
        static readonly BitmapImage[] attack;
        static readonly BitmapImage[] bridge;
        static readonly BitmapImage[] excavator;
        static readonly BitmapImage[] house;
        static readonly BitmapImage[] initial;
        static readonly BitmapImage[] robotmaker;
        static readonly BitmapImage[] town;

        static MassInformation()
        {
            hole = CreateBitmapImage("hole18.png");
            attack = CreateBitmapImage("attack0.png", "attack1.png", "attack2.png");
            bridge = CreateBitmapImage("bridge0.png", "bridge1.png", "bridge2.png");
            excavator = CreateBitmapImage("excavator0.png", "excavator1.png", "excavator2.png");
            house = CreateBitmapImage("house0.png", "house1.png", "house2.png");
            initial = CreateBitmapImage("largerobotmaker0.png", "largerobotmaker1.png", "largerobotmaker2.png");
            robotmaker = CreateBitmapImage("robotmaker0.png", "robotmaker1.png", "robotmaker2.png");
            town = CreateBitmapImage("town0.png", "town1.png", "town2.png");
        }

        public MassInformation()
        {
            InitializeComponent();
        }

        private static BitmapImage[] CreateBitmapImage(params string[] strs)
        {
            BitmapImage[] result = new BitmapImage[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                result[i] = new BitmapImage(new Uri("/Resources/" + strs[i], UriKind.Relative));
            }
            return result;
        }

        public void UpdateTerrain(int player, Terrain ter)
        {
            switch (player)
            {
                case 0:
                    Background = new SolidColorBrush(Color.FromRgb(255, 128, 128));
                    break;
                case 1:
                    Background = new SolidColorBrush(Color.FromRgb(128, 128, 255));
                    break;
                case 2:
                    Background = new SolidColorBrush(Color.FromRgb(128, 255, 128));
                    break;
                default:
                    Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                    break;
            }
            if (ter == Terrain.Outside)
            {
                Opacity = 0.3;
            }
            else
            {
                Opacity = 1;
            }
            switch (ter)
            {
                case Terrain.AttackTower:
                    TerrainImage.Source = attack[player];
                    break;
                case Terrain.Bridge:
                    TerrainImage.Source = bridge[player];
                    break;
                case Terrain.Excavator:
                    TerrainImage.Source = excavator[player];
                    break;
                case Terrain.Hole:
                    TerrainImage.Source = hole[0];
                    break;
                case Terrain.House:
                    TerrainImage.Source = house[player];
                    break;
                case Terrain.Initial:
                    TerrainImage.Source = initial[player];
                    break;
                case Terrain.RobotMaker:
                    TerrainImage.Source = robotmaker[player];
                    break;
                case Terrain.Town:
                    TerrainImage.Source = town[player];
                    break;
                default:
                    TerrainImage.Source = null;
                    break;
            }
        }

        public void UpdateTextInfo(string info)
        {
            TextInfo.Text = info;
        }
    }
}
