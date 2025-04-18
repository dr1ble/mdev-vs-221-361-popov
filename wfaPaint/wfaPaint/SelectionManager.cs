using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wfaPaint
{
    internal class SelectionManager
    {
        public Rectangle SelectedArea { get; private set; }
        public Bitmap? SelectedBitmap { get; private set; }
        public bool IsDragging { get; private set; }
        public bool HasSelection => SelectedBitmap != null;
        private Point dragOffset;

        public void StartSelection(Point startLocation)
        {
            SelectedArea = new Rectangle(startLocation, Size.Empty);
            IsDragging = false;
            SelectedBitmap?.Dispose();
            SelectedBitmap = null;
        }

        public void UpdateSelection(DrawingManager drawingManager, Point currentLocation)
        {
            var rect = GetRect(SelectedArea.Location, currentLocation);

            drawingManager.RestoreBackup();

            SelectedArea = rect;

            if (rect.Width > 0 && rect.Height > 0)
            {
                SelectedBitmap = new Bitmap(rect.Width, rect.Height);
                using (Graphics g = Graphics.FromImage(SelectedBitmap))
                {
                    g.DrawImage(drawingManager.Bitmap, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
                }

                using (Graphics g = Graphics.FromImage(drawingManager.Bitmap))
                {
                    using SolidBrush transparentBrush = new(Color.FromArgb(0, 0, 0, 0));
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    g.FillRectangle(transparentBrush, rect);
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                }
            }
        }

        public void StartDragging(Point mouseLocation)
        {
            dragOffset = new Point(mouseLocation.X - SelectedArea.X, mouseLocation.Y - SelectedArea.Y);
            IsDragging = true;
        }

        public void DragTo(Point mouseLocation)
        {
            if (IsDragging)
            {
                var newTopLeft = new Point(mouseLocation.X - dragOffset.X, mouseLocation.Y - dragOffset.Y);
                SelectedArea = new Rectangle(newTopLeft, SelectedArea.Size);
            }
        }

        public void FinishDragging(DrawingManager drawingManager)
        {
            if (SelectedBitmap != null)
            {
                using (Graphics g = Graphics.FromImage(drawingManager.Bitmap))
                {
                    g.DrawImage(SelectedBitmap, SelectedArea);
                }
            }
            ClearSelection();
        }

        public void ClearSelection()
        {
            SelectedArea = Rectangle.Empty;
            SelectedBitmap?.Dispose();
            SelectedBitmap = null;
            IsDragging = false;
        }

        public void DeleteSelection(DrawingManager drawingManager)
        {
            if (SelectedArea != Rectangle.Empty)
            {
                using SolidBrush transparent = new(Color.FromArgb(0, 255, 255, 255));
                drawingManager.Graphics.FillRectangle(transparent, SelectedArea);
                ClearSelection();
            }
        }

        public void SetSelection(Rectangle area, Bitmap bitmap)
        {
            SelectedArea = area;
            SelectedBitmap = bitmap;
        }

        private Rectangle GetRect(Point a, Point b)
        {
            return new Rectangle(
                Math.Min(a.X, b.X),
                Math.Min(a.Y, b.Y),
                Math.Abs(a.X - b.X),
                Math.Abs(a.Y - b.Y));
        }
    }
}
