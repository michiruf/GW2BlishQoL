using Blish_HUD.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = Microsoft.Xna.Framework.Point;

namespace Kenedia.Modules.QoL
{
    internal static class PointExtensions
    {
        public static int Distance2D(this Point p1, Point p2)
        {
            return (int)Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }
        public static int Distance3D(this Vector3 p1, Vector3 p2)
        {
            float deltaX = p2.X - p1.X;
            float deltaY = p2.Y - p1.Y;
            float deltaZ = p2.Z - p1.Z;

            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
            return (int) distance;
        }
        public static Point Add(this Point b, Point p)
        {
            return new Point(b.X + p.X, b.Y + p.Y);
        }
        public static Point Scale(this Point p, double factor)
        {
            return new Point((int)(p.X * factor), (int)(p.Y * factor));
        }
        public static string ConvertToString(this Point p)
        {
            return string.Format("X: {0}, Y: {1}", p.X, p.Y);
        }
    }

    internal static class RectangleExtensions
    {
        public static string ConvertToString(this Rectangle r)
        {
            return string.Format("X: {0}, Y: {1}, Width: {2}, Height: {3}", r.Left, r.Top, r.Width, r.Height);
        }
    }

    internal static class DisposableExtensions
    {
        public static void DisposeAll(this IEnumerable<IDisposable> disposables)
        {
            foreach (var d in disposables)
                d?.Dispose();
        }
    }
}
