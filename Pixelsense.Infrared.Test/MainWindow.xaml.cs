using Microsoft.Surface.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Pixelsense.Infrared.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SurfaceWindow
    {
        InfraredDevice device;
        public MainWindow()
        {
            InitializeComponent();
            device = new InfraredDevice(new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle);
            device.InfraredInputAdded += device_InfraredInputAdded;
            device.InfraredInputRemoved += device_InfraredInputRemoved;
            device.InfraredInputLocationChanged += device_InfraredInputLocationChanged;
        }

        void device_InfraredInputLocationChanged(object sender, InfraredInputEventArgs e)
        {
            //Console.WriteLine("MOVED->Led{0}\tTime:{1}\tLocation{2}", e.InfraredInput.Id, DateTime.Now.ToString("mm:ss:fff"), e.InfraredInput.Center);
        }

        Stopwatch sw = new Stopwatch();

        void device_InfraredInputRemoved(object sender, InfraredInputEventArgs e)
        {
            sw.Stop();

            this.Dispatcher.Invoke(()=>Background = GetBrush((int)sw.ElapsedMilliseconds));
            sw.Reset();
        }
        void device_InfraredInputAdded(object sender, InfraredInputEventArgs e)
        {
            sw.Start();
        }

        Color[] _colors = new Color[] { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Cyan, Colors.Purple, Colors.White };
        private SolidColorBrush GetBrush(int val)
        {
            const int start = 50;
            const int interval = 100;    // Minimum possible interval seems to be 50, but differentiating between < 30ms is difficult.

            int step = (val - start) / interval;

            return step >= 0 && step < _colors.Length
                ? new SolidColorBrush(_colors[step])
                : new SolidColorBrush(Colors.Black);
        }
    }
}
