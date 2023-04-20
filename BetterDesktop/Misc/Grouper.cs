using Avalonia;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace BetterDesktop.Misc
{
    /// <summary>Handle's the grouping of points.</summary>
    internal class Grouper
    {
        /// <summary>Group points into rectangles.</summary>
        public List<Rectangle> GroupPoints(Point[] points, float maxDistance)
        {
            // TODO: refine algorithm.

            // algorithm derived from here:
            // https://stackoverflow.com/a/24372504

            List<Rectangle> rects = new List<Rectangle>();
            for (int i = 0; i < points.Length; i++)
            {
                List<Point> closePoints = new List<Point>();

                // loop from back, downwards to minimize the amount of loops.
                // this also prevents double checking.
                for (int j = points.Length - 1; j >= i; j--)
                {
                    if (points[i].Distance(points[j]) < maxDistance)
                    {
                        closePoints.Add(points[j]);
                    }
                }

                var minX = float.MaxValue;
                var minY = float.MaxValue;
                var maxX = float.MinValue;
                var maxY = float.MinValue;

                // find the min and maz to create a new rectangle.
                for (int k = 0; k < closePoints.Count; k++)
                {
                    if (closePoints[k].X < minX) 
                        minX = closePoints[k].X;
                    if (closePoints[k].Y < minY)
                        minY = closePoints[k].Y;
                    if (closePoints[k].X > maxX)
                        maxX = closePoints[k].X;
                    if (closePoints[k].Y > maxY)
                        maxY = closePoints[k].Y;
                }

                rects.Add(new Rectangle((int)minX - 30, (int)minY - 10, (int)(maxX - minX) + 120, (int)(maxY - minY) + 120));
            }

            return rects;
        }

    }
}
