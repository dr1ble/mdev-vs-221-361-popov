using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wfaPaint
{
    public class DrawingManager
    {
        public Bitmap Bitmap { get; private set; }

        public event EventHandler? BitmapChanged;
        public Graphics Graphics { get; private set; }
        public Pen Pen { get; private set; }
        public Bitmap Backup { get; private set; }

        public DrawingManager(int width, int height, Color color, int penWidth)
        {
            Bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics = Graphics.FromImage(Bitmap);
            Graphics.Clear(Color.Transparent);
            Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen = new Pen(color, penWidth);
            Pen.StartCap = Pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        public void BackupImage()
        {
            Backup = (Bitmap)Bitmap.Clone();
        }

        public void RestoreBackup()
        {
            Graphics.Dispose();
            Bitmap.Dispose();
            Bitmap = (Bitmap)Backup.Clone();
            Graphics = Graphics.FromImage(Bitmap);
        }

        public void ChangeColor(Color color) => Pen.Color = color;

        public void ChangePenWidth(int width) => Pen.Width = width;

        public void LoadImage(string filePath)
        {
            using Bitmap loadedBitmap = (Bitmap)Bitmap.FromFile(filePath);
            Graphics.Dispose();
            Bitmap.Dispose();
            Bitmap = new Bitmap(loadedBitmap.Width, loadedBitmap.Height, PixelFormat.Format32bppArgb);
            Graphics.Clear(Color.Transparent);
            Graphics.DrawImage(loadedBitmap, 0, 0);
        }

        protected virtual void OnBitmapChanged()
        {
            BitmapChanged?.Invoke(this, EventArgs.Empty);
        }
    }

}
