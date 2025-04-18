using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace wfaPaint
{
    public partial class wfaPaint : Form
    {
        private enum MyDrawMode
        {
            Pencil,
            Line,
            Ellipse,
            Rectangle,
            Triangle,
            Arrow,
            Star,
            Hexagon,
            Select
        }

        private DrawingManager drawingManager;
        private Point startLocation;
        private MyDrawMode drawMode = MyDrawMode.Pencil;
        private bool isDrawing = false;
        private Point dragOffset; // смещение курсора относительно угла выделенной области


        private Rectangle? selectedArea = null;
        private Bitmap? selectedBitmap = null;
        private bool isDragging = false;

        public wfaPaint()
        {
            InitializeComponent();

            drawingManager = new DrawingManager(Screen.PrimaryScreen.Bounds.Width,
                                                Screen.PrimaryScreen.Bounds.Height,
                                                panel2.BackColor, 10);

            panel2.Click += (s, e) => drawingManager.ChangeColor(panel2.BackColor);
            panel3.Click += (s, e) => drawingManager.ChangeColor(panel3.BackColor);
            panel4.Click += (s, e) => drawingManager.ChangeColor(panel4.BackColor);
            panel5.Click += (s, e) => drawingManager.ChangeColor(panel5.BackColor);

            buSelectPen.Click += (s, e) => drawMode = MyDrawMode.Pencil;
            buSelectLine.Click += (s, e) => drawMode = MyDrawMode.Line;
            buSelectEllipse.Click += (s, e) => drawMode = MyDrawMode.Ellipse;
            buSelectRectangle.Click += (s, e) => drawMode = MyDrawMode.Rectangle;
            buSelectTriangle.Click += (s, e) => drawMode = MyDrawMode.Triangle;
            buSelectArrow.Click += (s, e) => drawMode = MyDrawMode.Arrow;
            buSelectStar.Click += (s, e) => drawMode = MyDrawMode.Star;
            buSelectHexagon.Click += (s, e) => drawMode = MyDrawMode.Hexagon;
            buModeSelect.Click += (s, e) => drawMode = MyDrawMode.Select;

            buSaveAsToFile.Click += BuSaveAsToFile_Click;
            buLoadFromFile.Click += BuLoadFromFile_Click;
            buNewImage.Click += BuNewImage_Click;

            trPenWidth.Minimum = 1;
            trPenWidth.Maximum = 12;
            trPenWidth.Value = (int)drawingManager.Pen.Width;
            trPenWidth.ValueChanged += (s, e) => drawingManager.ChangePenWidth(trPenWidth.Value);

            pxImage.MouseDown += PxImage_MouseDown;
            pxImage.MouseMove += PxImage_MouseMove;
            pxImage.MouseUp += PxImage_MouseUp;
            pxImage.Paint += (s, e) =>
            {
                e.Graphics.DrawImage(drawingManager.Bitmap, 0, 0);
                if (drawMode == MyDrawMode.Select && selectedArea.HasValue && !isDragging)
                {
                    using Pen dashed = new Pen(Color.Black) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
                    e.Graphics.DrawRectangle(dashed, selectedArea.Value);
                }
            };
        }

        private void BuNewImage_Click(object? sender, EventArgs e)
        {
            drawingManager = new DrawingManager(Screen.PrimaryScreen.Bounds.Width,
                                                Screen.PrimaryScreen.Bounds.Height,
                                                drawingManager.Pen.Color,
                                                (int)drawingManager.Pen.Width);
            trPenWidth.Value = (int)drawingManager.Pen.Width;
            pxImage.Invalidate();
        }

        private void BuSaveAsToFile_Click(object? sender, EventArgs e)
        {
            using SaveFileDialog dialog = new();
            dialog.Filter = "PNG Files(*.PNG)|*.PNG";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                drawingManager.Bitmap.Save(dialog.FileName);
            }
        }

        private void BuLoadFromFile_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialog = new();
            dialog.Filter = "PNG Files(*.PNG)|*.PNG";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                drawingManager.LoadImage(dialog.FileName);
                pxImage.Invalidate();
            }
        }

        private void PxImage_MouseDown(object? sender, MouseEventArgs e)
        {
            startLocation = e.Location;
            isDrawing = true;

            if (drawMode == MyDrawMode.Select && selectedArea.HasValue && selectedArea.Value.Contains(e.Location))
            {
                isDragging = true;

                var oldRect = selectedArea.Value;

                dragOffset = new Point(e.X - oldRect.X, e.Y - oldRect.Y); // ⬅️ фиксируем смещение

                selectedBitmap = new Bitmap(oldRect.Width, oldRect.Height);
                using (Graphics g = Graphics.FromImage(selectedBitmap))
                {
                    g.DrawImage(drawingManager.Bitmap, new Rectangle(0, 0, oldRect.Width, oldRect.Height), oldRect, GraphicsUnit.Pixel);
                }

                drawingManager.BackupImage();

                using (Graphics g = Graphics.FromImage(drawingManager.Bitmap))
                {
                    Bitmap transparent = new Bitmap(oldRect.Width, oldRect.Height, PixelFormat.Format32bppArgb);
                    g.DrawImage(transparent, oldRect);
                }
            }
            else
            {
                drawingManager.BackupImage();
            }
        }


        private void PxImage_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!isDrawing || e.Button != MouseButtons.Left) return;

            if (drawMode == MyDrawMode.Select && isDragging && selectedArea.HasValue && selectedBitmap != null)
            {
                var newTopLeft = new Point(e.X - dragOffset.X, e.Y - dragOffset.Y);
                var oldRect = selectedArea.Value;
                var newRect = new Rectangle(newTopLeft, oldRect.Size);

                drawingManager.RestoreBackup();
                drawingManager.Graphics.DrawImage(selectedBitmap, newRect);

                selectedArea = newRect;
                pxImage.Invalidate();
                return;
            }
            else if (drawMode == MyDrawMode.Select)
            {
                var rect = GetRect(startLocation, e.Location);
                selectedArea = rect;

                selectedBitmap?.Dispose();
                selectedBitmap = null;

                if (rect.Width > 0 && rect.Height > 0)
                {
                    selectedBitmap = drawingManager.Bitmap.Clone(rect, drawingManager.Bitmap.PixelFormat);
                }

                pxImage.Invalidate();
            }

            else
            {
                drawingManager.RestoreBackup();

                switch (drawMode)
                {
                    case MyDrawMode.Pencil:
                        FigureDrawer.DrawLine(drawingManager.Graphics, drawingManager.Pen, startLocation, e.Location);
                        startLocation = e.Location;
                        break;
                    case MyDrawMode.Line:
                        FigureDrawer.DrawLine(drawingManager.Graphics, drawingManager.Pen, startLocation, e.Location);
                        break;
                    case MyDrawMode.Ellipse:
                        FigureDrawer.DrawEllipse(drawingManager.Graphics, drawingManager.Pen, startLocation, e.Location);
                        break;
                    case MyDrawMode.Rectangle:
                        FigureDrawer.DrawRectangle(drawingManager.Graphics, drawingManager.Pen, startLocation, e.Location);
                        break;
                    case MyDrawMode.Triangle:
                        FigureDrawer.DrawTriangle(drawingManager.Graphics, drawingManager.Pen, startLocation, e.Location);
                        break;
                    case MyDrawMode.Arrow:
                        FigureDrawer.DrawArrow(drawingManager.Graphics, drawingManager.Pen, startLocation, e.Location);
                        break;
                    case MyDrawMode.Star:
                        FigureDrawer.DrawStar(drawingManager.Graphics, drawingManager.Pen, startLocation, e.Location);
                        break;
                    case MyDrawMode.Hexagon:
                        FigureDrawer.DrawHexagon(drawingManager.Graphics, drawingManager.Pen, startLocation, e.Location);
                        break;
                }

                pxImage.Invalidate();
            }
        }


        private void PxImage_MouseUp(object? sender, MouseEventArgs e)
        {
            isDrawing = false;

            if (isDragging)
            {
                isDragging = false;

                selectedArea = null;
                selectedBitmap?.Dispose();
                selectedBitmap = null;

                pxImage.Invalidate();
            }
        }

        private Rectangle GetRect(Point a, Point b)
        {
            return new Rectangle(
                Math.Min(a.X, b.X),
                Math.Min(a.Y, b.Y),
                Math.Abs(a.X - b.X),
                Math.Abs(a.Y - b.Y));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.V))
            {
                PasteFromClipboard();
                return true;
            }

            if (selectedArea.HasValue)
            {
                if (keyData == (Keys.Control | Keys.C))
                {
                    CopySelectionToClipboard();
                    return true;
                }

                if (keyData == (Keys.Control | Keys.X))
                {
                    CopySelectionToClipboard();
                    DeleteSelection();
                    return true;
                }

                if (keyData == Keys.Delete)
                {
                    DeleteSelection();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void CopySelectionToClipboard()
        {
            if (selectedBitmap != null)
            {
                Clipboard.SetImage(selectedBitmap);
            }
        }

        private void PasteFromClipboard()
        {
            try
            {
                IDataObject data = Clipboard.GetDataObject();
                if (data != null && data.GetDataPresent(DataFormats.Bitmap))
                {
                    var bitmap = (Bitmap)data.GetData(DataFormats.Bitmap);
                    if (bitmap == null) return;

                    drawingManager.BackupImage();

                    // Вставить в центр холста
                    int x = (drawingManager.Bitmap.Width - bitmap.Width) / 2;
                    int y = (drawingManager.Bitmap.Height - bitmap.Height) / 2;

                    drawingManager.Graphics.DrawImage(bitmap, x, y);
                    pxImage.Invalidate();
                }
                else
                {
                    MessageBox.Show("Буфер обмена не содержит изображение", "Вставка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка вставки изображения: " + ex.Message);
            }
        }



        private void DeleteSelection()
        {
            if (selectedArea.HasValue)
            {
                Rectangle rect = selectedArea.Value;
                using SolidBrush transparent = new(Color.FromArgb(0, 255, 255, 255));
                drawingManager.Graphics.FillRectangle(transparent, rect);

                selectedArea = null;
                selectedBitmap?.Dispose();
                selectedBitmap = null;
                pxImage.Invalidate();
            }
        }

    }
}
