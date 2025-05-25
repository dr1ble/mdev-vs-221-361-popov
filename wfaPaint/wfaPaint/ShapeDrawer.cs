using System.Drawing.Drawing2D;
using System;
namespace wfaPaint
{
    // Класс ShapeDrawer содержит статические методы для отрисовки различных геометрических фигур.
    // Он инкапсулирует логику вычисления координат и вызова соответствующих методов Graphics.
    internal static class ShapeDrawer
    {
        // DrawFigure является основным методом, который определяет, какую фигуру рисовать,
        // основываясь на переданном режиме (выбранном инструменте).
        // Он вызывает более специализированные методы для отрисовки каждой конкретной фигуры.
        public static void DrawFigure(Graphics g, Pen pen, ActiveToolManager.MyDrawMode mode, Point start, Point end)
        {
            switch (mode)
            {
                case ActiveToolManager.MyDrawMode.Pencil:
                case ActiveToolManager.MyDrawMode.Line:
                    DrawLine(g, pen, start, end);
                    break;
                case ActiveToolManager.MyDrawMode.Ellipse:
                    DrawEllipse(g, pen, start, end);
                    break;
                case ActiveToolManager.MyDrawMode.Rectangle:
                    DrawRectangle(g, pen, start, end);
                    break;
                case ActiveToolManager.MyDrawMode.Triangle:
                    DrawTriangle(g, pen, start, end);
                    break;
                case ActiveToolManager.MyDrawMode.Arrow:
                    DrawArrow(g, pen, start, end);
                    break;
                case ActiveToolManager.MyDrawMode.Star:
                    // Звезда рисуется с параметрами по умолчанию: 5 лучей и определенная "острота".
                    DrawStar(g, pen, start, end);
                    break;
                case ActiveToolManager.MyDrawMode.Hexagon:
                    DrawHexagon(g, pen, start, end);
                    break;
            }
        }

        // Рисует простую линию между указанными начальной и конечной точками.
        public static void DrawLine(Graphics g, Pen pen, Point start, Point end)
        {
            g.DrawLine(pen, start, end);
        }

        // Рисует эллипс. Эллипс определяется прямоугольником,
        // который задается диагональными точками start и end.
        public static void DrawEllipse(Graphics g, Pen pen, Point start, Point end)
        {
            g.DrawEllipse(pen, GetRect(start, end));
        }

        // Рисует прямоугольник. Точки start и end определяют его диагональ.
        public static void DrawRectangle(Graphics g, Pen pen, Point start, Point end)
        {
            g.DrawRectangle(pen, GetRect(start, end));
        }

        // Рисует равнобедренный треугольник. Его верхняя вершина находится
        // посередине отрезка, соединяющего X-координаты start и end, на Y-координате start.
        // Две другие вершины формируют основание на Y-координате end.
        public static void DrawTriangle(Graphics g, Pen pen, Point start, Point end)
        {
            Point p1 = new Point(start.X + (end.X - start.X) / 2, start.Y);
            Point p2 = new Point(start.X, end.Y);
            Point p3 = new Point(end.X, end.Y);
            g.DrawPolygon(pen, new[] { p1, p2, p3 });
        }

        // Рисует линию с наконечником в виде стрелки в конечной точке.
        // Для этого создается клон основного пера, чтобы не изменять его глобально.
        public static void DrawArrow(Graphics g, Pen pen, Point start, Point end)
        {
            using (Pen arrowPen = (Pen)pen.Clone())
            {
                // Размер и форма наконечника стрелки.
                using (AdjustableArrowCap arrowCap = new AdjustableArrowCap(5, 5))
                {
                    arrowPen.CustomEndCap = arrowCap; // Применение наконечника к перу.
                    g.DrawLine(arrowPen, start, end);
                }
            }
        }

        // Рисует звезду, вписанную в эллипс (определяемый точками start и end).
        // Можно настроить количество лучей (numPoints) и их "остроту" (innerRadiusRatio).
        public static void DrawStar(Graphics g, Pen pen, Point start, Point end, int numPoints = 5, double innerRadiusRatio = 0.382)
        {
            if (numPoints < 2) numPoints = 2;
            if (innerRadiusRatio <= 0 || innerRadiusRatio >= 1) innerRadiusRatio = 0.382;

            Rectangle rect = GetRect(start, end);
            if (rect.Width <= 0 || rect.Height <= 0) return; // Проверка на вырожденный прямоугольник.

            Point[] points = new Point[numPoints * 2]; // Массив для вершин звезды.

            // Расчет радиусов и центра для вписывания звезды.
            double outerRx = rect.Width / 2.0, outerRy = rect.Height / 2.0;
            double innerRx = outerRx * innerRadiusRatio, innerRy = outerRy * innerRadiusRatio;
            double cx = rect.Left + outerRx, cy = rect.Top + outerRy;
            // Угол между "половинками" лучей.
            double angleStep = Math.PI / numPoints;

            for (int i = 0; i < numPoints * 2; i++)
            {
                // Угол текущей вершины, со смещением для вертикальной ориентации первого луча.
                double angle = angleStep * i - (Math.PI / 2.0);
                bool isOuterVertex = (i % 2 == 0); // Чередование внешних и внутренних вершин.
                double rX = isOuterVertex ? outerRx : innerRx;
                double rY = isOuterVertex ? outerRy : innerRy;
                points[i] = new Point((int)(cx + rX * Math.Cos(angle)), (int)(cy + rY * Math.Sin(angle)));
            }
            g.DrawPolygon(pen, points);
        }

        // Рисует правильный шестиугольник, вписанный в эллипс,
        // который задается точками start и end.
        public static void DrawHexagon(Graphics g, Pen pen, Point start, Point end)
        {
            Rectangle rect = GetRect(start, end);
            if (rect.Width <= 0 || rect.Height <= 0) return;

            Point[] points = new Point[6];
            double rx = rect.Width / 2.0, ry = rect.Height / 2.0; // Полуоси эллипса.
            double cx = rect.Left + rx, cy = rect.Top + ry;       // Центр эллипса.

            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i; // Угол для каждой из 6 вершин (шаг 60 градусов).
                points[i] = new Point((int)(cx + rx * Math.Cos(angle)), (int)(cy + ry * Math.Sin(angle)));
            }
            g.DrawPolygon(pen, points);
        }

        // Вспомогательный приватный метод для получения объекта Rectangle
        // по двум диагональным точкам (p1 и p2).
        // Гарантирует, что X и Y будут координатами верхнего левого угла,
        // а Width и Height - положительными значениями.
        private static Rectangle GetRect(Point p1, Point p2)
        {
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.Y - p2.Y)
            );
        }
    }
}