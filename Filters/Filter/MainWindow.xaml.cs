using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp.Classes;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        Random rnd = new Random();
        DrawingVisual visual;
        DrawingContext dc;
        double width, height;
        Axis axis;
        Point mouse;
        int factor = 25;

        KalmanFilter kalman;
        List<double> signalDataX;
        List<double> signalDataY;

        AlphaBeta alphaBeta;


        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            width = g.Width;
            height = g.Height;

            mouse = new Point();
            visual = new DrawingVisual();

            axis = new Axis(width, height);
            axis.SetFactor(factor);

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            // Kalman filter
            
            signalDataX = new List<double>();
            signalDataY = new List<double>();
            for (double x = -20; x < 20; x += 0.1)
            {
                signalDataX.Add(x);
                double y = rnd.Next(-2, 3) * rnd.NextDouble();
                signalDataY.Add(y);
            }

            kalman = new KalmanFilter(f: 1, h: 1, q: 2, r: 15);
            kalman.SetState(signalDataY[0], 0); // Задаем начальные значение State и Covariance
            foreach (var d in signalDataY)
            {
                kalman.Correct(d);
                kalman.filteredData.Add(kalman.State);
            }

            // AlphaBeta filter
            alphaBeta = new AlphaBeta();
            foreach (var d in signalDataY)
            {
                var v = alphaBeta.Correct(d);
                alphaBeta.filteredData.Add(v);
            }

            Drawing();
        }

        private void g_MouseMove(object sender, MouseEventArgs e)
        {
            mouse = e.GetPosition(g);
            Drawing();
        }
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                factor -= 1;
            }
            else
            {
                // increasing
                factor += 1;
            }

            if (factor <= 0) factor = 1;
            if (factor >= 58) factor = 58;

            Drawing();
        }
        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                // axis drawing
                axis.Draw(dc, visual);

                axis.SetFactor(factor); // for mouse wheel

                var signal_points = new List<Point>();
                kalman.filtered_points.Clear();
                alphaBeta.filtered_points.Clear();
                for (int i = 0; i < signalDataX.Count; ++i)
                {
                    signal_points.Add(new Point(axis.Xto(signalDataX[i]), axis.Yto(signalDataY[i])));    // original signal
                    kalman.filtered_points.Add(new Point(axis.Xto(signalDataX[i]), axis.Yto(kalman.filteredData[i]))); // filtered signal
                    alphaBeta.filtered_points.Add(new Point(axis.Xto(signalDataX[i]), axis.Yto(alphaBeta.filteredData[i]))); // filtered signal
                }

                if (cbSignal.IsChecked == true) DrawLine(dc, Brushes.Red, signal_points, 0.7);
                if (cbKalman.IsChecked == true) DrawLine(dc, Brushes.Lime, kalman.filtered_points, 1);
                if (cbAlphaBeta.IsChecked == true) DrawLine(dc, Brushes.DodgerBlue, alphaBeta.filtered_points, 1);

                dc.Close();
                g.AddVisual(visual);
            }
        }
        private void DrawLine(DrawingContext dc, Brush brush, List<Point> points, double thickness = 1)
        {
            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = streamGeometry.Open())
            {
                geometryContext.BeginFigure(points[0], false, false);
                geometryContext.PolyLineTo(points, true, false);
            }
            dc.DrawGeometry(null, new Pen(brush, thickness), streamGeometry);
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e) => Drawing();
        private void timerTick(object sender, EventArgs e) => Drawing();
    }
}