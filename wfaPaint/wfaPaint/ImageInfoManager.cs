using System;
using System.Drawing;
using System.Windows.Forms;

namespace wfaPaint
{
    public class ImageInfoManager
    {
        private readonly Form parentForm;
        private PictureBox pictureBox;
        private DrawingManager drawingManager;
        private ToolStripLabel infoLabel;
        private Point currentMousePosition;

        public ImageInfoManager(Form form, DrawingManager manager, PictureBox pictureBox, ToolStripLabel label)
        {
            parentForm = form;
            this.pictureBox = pictureBox;
            this.infoLabel = label;
            AttachToDrawingManager(manager);

            this.pictureBox.Resize += (s, e) => Update(); // обновлять при изменении размера
        }

        public void AttachToDrawingManager(DrawingManager newManager)
        {
            if (drawingManager != null)
            {
                drawingManager.BitmapChanged -= OnBitmapChanged;
            }

            drawingManager = newManager;
            if (drawingManager != null)
            {
                drawingManager.BitmapChanged += OnBitmapChanged;
            }

            Update();
        }

        private void OnBitmapChanged(object? sender, EventArgs e)
        {
            Update();
        }

        public void UpdateMousePosition(Point location)
        {
            currentMousePosition = location;
            Update();
        }

        private void Update()
        {
            if (infoLabel == null || pictureBox == null)
                return;

            // Берём размер PictureBox
            var sizeText = $"Image: {pictureBox.Width} x {pictureBox.Height}";
            var mouseText = $"Mouse: ({currentMousePosition.X}, {currentMousePosition.Y})";

            string colorText = "Color: N/A";
            if (drawingManager != null && drawingManager.Bitmap != null &&
                currentMousePosition.X >= 0 && currentMousePosition.X < drawingManager.Bitmap.Width &&
                currentMousePosition.Y >= 0 && currentMousePosition.Y < drawingManager.Bitmap.Height)
            {
                Color pixelColor = drawingManager.Bitmap.GetPixel(currentMousePosition.X, currentMousePosition.Y);
                colorText = $"Color: #{pixelColor.R:X2}{pixelColor.G:X2}{pixelColor.B:X2}";
            }

            infoLabel.Text = $"{sizeText} | {mouseText} | {colorText}";
        }
    }
}
