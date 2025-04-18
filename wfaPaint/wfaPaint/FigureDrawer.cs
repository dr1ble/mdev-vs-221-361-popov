using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wfaPaint
{
    public static class FigureDrawer
    {
        public static void DrawLine(Graphics g, Pen pen, Point start, Point end)
        {
            g.DrawLine(pen, start, end);
        }

        public static void DrawRectangle(Graphics g, Pen pen, Point start, Point end)
        {
            var rect = GetRect(start, end);
            g.DrawRectangle(pen, rect);
        }

        public static void DrawEllipse(Graphics g, Pen pen, Point start, Point end)
        {
            var rect = GetRect(start, end);
            g.DrawEllipse(pen, rect);
        }

        public static void DrawTriangle(Graphics g, Pen pen, Point start, Point end)
        {
            int w = end.X - start.X;
            int h = end.Y - start.Y;
            Point p1 = new(start.X + w / 2, start.Y);
            Point p2 = new(start.X + w, start.Y + h);
            Point p3 = new(start.X, start.Y + h);
            g.DrawPolygon(pen, new[] { p1, p2, p3 });
        }

        private static Rectangle GetRect(Point start, Point end)
        {
            return new Rectangle(
                Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y),
                Math.Abs(end.X - start.X),
                Math.Abs(end.Y - start.Y));
        }

        public static void DrawArrow(Graphics g, Pen pen, Point start, Point end)
        {
            using AdjustableArrowCap arrowCap = new(6, 6);
            Pen arrowPen = (Pen)pen.Clone();
            arrowPen.CustomEndCap = arrowCap;
            g.DrawLine(arrowPen, start, end);
            arrowPen.Dispose();
        }

        public static void DrawStar(Graphics g, Pen pen, Point start, Point end)
        {
            var rect = GetRect(start, end);
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            int outerR = Math.Min(rect.Width, rect.Height) / 2;
            int innerR = outerR / 2;

            Point[] points = new Point[10];
            for (int i = 0; i < 10; i++)
            {
                double angle = Math.PI / 5 * i - Math.PI / 2;
                int r = i % 2 == 0 ? outerR : innerR;
                points[i] = new Point(
                    (int)(cx + r * Math.Cos(angle)),
                    (int)(cy + r * Math.Sin(angle)));
            }
            g.DrawPolygon(pen, points);
        }

        public static void DrawHexagon(Graphics g, Pen pen, Point start, Point end)
        {
            var rect = GetRect(start, end);
            int cx = rect.X + rect.Width / 2;   
            int cy = rect.Y + rect.Height / 2;
            int r = Math.Min(rect.Width, rect.Height) / 2;

            Point[] points = new Point[6];
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i - Math.PI / 2;
                points[i] = new Point(
                    (int)(cx + r * Math.Cos(angle)),
                    (int)(cy + r * Math.Sin(angle)));
            }
            g.DrawPolygon(pen, points);
        }
    }

}
