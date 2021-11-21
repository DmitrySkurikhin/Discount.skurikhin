using System;

namespace LinearInterpolation
{
    public struct Point
    {
        public double x;
        public double y;

        public Point(double xval, double yval)
        {
            x = xval;
            y = yval;
        }
    }

    public class Interpolation
    {
        /// <summary>
        /// Linear interpolation for an array of points
        /// </summary>
        /// <param name="xs">Array for argument values</param>
        /// <param name="ys">Array for functions values</param>
        /// <returns>Returns function which calculates interpolated value for given z</returns>
        public static Func<Double, Double> GetInterpolation(Point[] points)
        {
            return (z) =>
            {
                var p = points;
                int i = 0;
                if (z < p[0].x)
                {
                    return p[i].y + (p[i + 1].y - p[i].y) / (p[i + 1].x - p[i].x) * (z - p[i].x);
                }
                for (; i < p.Length - 1; i++)
                {
                    if (z <= p[i + 1].x)
                    {
                        return p[i].y + (p[i + 1].y - p[i].y) / (p[i + 1].x - p[i].x) * (z - p[i].x);
                    }
                }
                i = p.Length - 1;
                // if we got beyond x_max
                return p[i].y + (p[i + 1].y - p[i].y) / (p[i + 1].x - p[i].x) * (z - p[i].x);
            };
        }
    }
}