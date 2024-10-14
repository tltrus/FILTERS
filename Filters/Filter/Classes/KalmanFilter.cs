

using System.Windows;

namespace WpfApp.Classes
{
    // Base on: https://habr.com/ru/articles/140274/

    internal class KalmanFilter
    {
        public List<Point> filtered_points = new List<Point>();
        public List<double> filteredData = new List<double>();
        public double X0 { get; set; } // predicted state
        public double P0 { get; set; } // predicted covariance

        public double F { get; set; } // factor of real value to previous real value
        public double Q { get; set; } // measurement noise
        public double H { get; set; } // factor of measured value to real value
        public double R { get; set; } // environment noise

        public double State { get; set; }
        public double Covariance { get; set; }

        public KalmanFilter(double q, double r, double f = 1, double h = 1)
        {
            Q = q;
            R = r;
            F = f;
            H = h;
        }

        public void SetState(double state, double covariance)
        {
            State = state;
            Covariance = covariance;
        }

        public void Correct(double data)
        {
            //time update - prediction
            X0 = F * State;
            P0 = F * Covariance * F + Q;

            //measurement update - correction
            var K = H * P0 / (H * P0 * H + R);
            State = X0 + K * (data - H * X0);
            Covariance = (1 - K * H) * P0;
        }
    }
}
