using System.Drawing.Drawing2D;
using wfaPaint;

internal static class FigureDrawer
{
    public static void DrawFigure(Graphics g, Pen pen, DrawingModeManager.MyDrawMode mode, Point start, Point end)
    {
        switch (mode)
        {
            case DrawingModeManager.MyDrawMode.Pencil:
                DrawLine(g, pen, start, end);
                break;
            case DrawingModeManager.MyDrawMode.Line:
                DrawLine(g, pen, start, end);
                break;
            case DrawingModeManager.MyDrawMode.Ellipse:
                DrawEllipse(g, pen, start, end);
                break;
            case DrawingModeManager.MyDrawMode.Rectangle:
                DrawRectangle(g, pen, start, end);
                break;
            case DrawingModeManager.MyDrawMode.Triangle:
                DrawTriangle(g, pen, start, end);
                break;
            case DrawingModeManager.MyDrawMode.Arrow:
                DrawArrow(g, pen, start, end);
                break;
            case DrawingModeManager.MyDrawMode.Star:
                DrawStar(g, pen, start, end);
                break;
            case DrawingModeManager.MyDrawMode.Hexagon:
                DrawHexagon(g, pen, start, end);
                break;
        }
    }

    public static void DrawLine(Graphics g, Pen pen, Point start, Point end)
    {
        g.DrawLine(pen, start, end);
    }

    public static void DrawEllipse(Graphics g, Pen pen, Point start, Point end)
    {
        Rectangle rect = GetRect(start, end);
        g.DrawEllipse(pen, rect);
    }

    public static void DrawRectangle(Graphics g, Pen pen, Point start, Point end)
    {
        Rectangle rect = GetRect(start, end);
        g.DrawRectangle(pen, rect);
    }

    public static void DrawTriangle(Graphics g, Pen pen, Point start, Point end)
    {
        Point p1 = new(start.X + (end.X - start.X) / 2, start.Y);
        Point p2 = new(start.X, end.Y);
        Point p3 = new(end.X, end.Y);
        g.DrawPolygon(pen, new[] { p1, p2, p3 });
    }

    public static void DrawArrow(Graphics g, Pen pen, Point start, Point end)
    {
        AdjustableArrowCap bigArrow = new(5, 5);
        Pen arrowPen = (Pen)pen.Clone();
        arrowPen.CustomEndCap = bigArrow;
        g.DrawLine(arrowPen, start, end);
        arrowPen.Dispose();
    }

    public static void DrawStar(Graphics g, Pen pen, Point start, Point end)
    {
        Rectangle rect = GetRect(start, end);
        Point[] points = new Point[10];
        double rx = rect.Width / 2;
        double ry = rect.Height / 2;
        double cx = rect.Left + rx;
        double cy = rect.Top + ry;
        for (int i = 0; i < 10; i++)
        {
            double r = (i % 2 == 0) ? Math.Min(rx, ry) : Math.Min(rx, ry) / 2.5;
            double angle = Math.PI / 5 * i - Math.PI / 2;
            points[i] = new Point((int)(cx + r * Math.Cos(angle)), (int)(cy + r * Math.Sin(angle)));
        }
        g.DrawPolygon(pen, points);
    }

    public static void DrawHexagon(Graphics g, Pen pen, Point start, Point end)
    {
        Rectangle rect = GetRect(start, end);
        Point[] points = new Point[6];
        double rx = rect.Width / 2;
        double ry = rect.Height / 2;
        double cx = rect.Left + rx;
        double cy = rect.Top + ry;
        for (int i = 0; i < 6; i++)
        {
            double angle = Math.PI / 3 * i;
            points[i] = new Point((int)(cx + rx * Math.Cos(angle)), (int)(cy + ry * Math.Sin(angle)));
        }
        g.DrawPolygon(pen, points);
    }

    private static Rectangle GetRect(Point a, Point b)
    {
        return new Rectangle(
            Math.Min(a.X, b.X),
            Math.Min(a.Y, b.Y),
            Math.Abs(a.X - b.X),
            Math.Abs(a.Y - b.Y));
    }
}