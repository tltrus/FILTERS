using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp.Classes
{
    internal class AlphaBeta
    {
        public List<Point> filtered_points = new List<Point>();
        public List<double> filteredData = new List<double>();
        // период дискретизации (измерений), process variation, noise variation
        double dt = 0.5;
        double xk_1 = 0, vk_1 = 0, a = 0.5, b = 0.1;
        double xk, vk, rk;

        public AlphaBeta()
        {

        }

        public double Correct(double xm)
        {
            xk = xk_1 + vk_1 * dt;
            vk = vk_1;

            rk = xm - xk;

            xk += a * rk;
            vk += b * rk / dt;

            xk_1 = xk;
            vk_1 = vk;
            return xk_1;
        }
    }
}
