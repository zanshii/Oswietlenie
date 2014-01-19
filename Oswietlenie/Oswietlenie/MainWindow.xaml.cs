using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Oswietlenie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    //obrot w lewo
                    break;
                case Key.D:
                    //obrot w prawo
                    break;
                case Key.Z:
                    //material rozpraszajacy (drewno)
                    break;
                case Key.X:
                    //material posredni (plastik)
                    break;
                case Key.C:
                    //material odbijajacy (szklo)
                    break;

                case Key.Escape:
                    Application.Current.Shutdown();
                    break;
            }
        }
    }
}
