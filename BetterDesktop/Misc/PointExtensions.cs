using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterDesktop.Misc
{
    internal static class PointExtensions
    {

        /// <summary>Find the distance between two points.</summary>
        public static float Distance(this Point a, Point b)
        {
            // https://g.co/kgs/qw5msR
            var xMinX = b.X - a.X;
            var yMinY = b.Y - a.Y;
            return MathF.Sqrt(MathF.Pow(xMinX, 2) + MathF.Pow(yMinY, 2));
        }

    }
}
