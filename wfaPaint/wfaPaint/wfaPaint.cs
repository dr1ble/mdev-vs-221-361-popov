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
        private DrawingModeManager drawingModeManager;
        private ClipboardManager clipboardManager;
        private SelectionManager selectionManager;
        private ImageInfoManager imageInfoManager;

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

            drawingManager = new DrawingManager(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, paSelectColorRed.BackColor, 10);
            drawingModeManager = new DrawingModeManager();
            selectionManager = new SelectionManager();
            clipboardManager = new ClipboardManager(drawingManager, selectionManager);
            imageInfoManager = new ImageInfoManager(this, drawingManager, pxImage, toolStripLabelImageInfo);

            InitializeUI();
            HookEvents();
        }

        private void InitializeUI()
        {
            paSelectColorRed.Click += (s, e) => drawingManager.ChangeColor(paSelectColorRed.BackColor);
            paSelectColorGreen.Click += (s, e) => drawingManager.ChangeColor(paSelectColorGreen.BackColor);
            paSelectColorYellow.Click += (s, e) => drawingManager.ChangeColor(paSelectColorYellow.BackColor);
            paSelectColorBlack.Click += (s, e) => drawingManager.ChangeColor(paSelectColorBlack.BackColor);

            buSelectPen.Click += (s, e) => drawingModeManager.SetMode(DrawingModeManager.MyDrawMode.Pencil);
            buSelectLine.Click += (s, e) => drawingModeManager.SetMode(DrawingModeManager.MyDrawMode.Line);
            buSelectEllipse.Click += (s, e) => drawingModeManager.SetMode(DrawingModeManager.MyDrawMode.Ellipse);
            buSelectRectangle.Click += (s, e) => drawingModeManager.SetMode(DrawingModeManager.MyDrawMode.Rectangle);
            buSelectTriangle.Click += (s, e) => drawingModeManager.SetMode(DrawingModeManager.MyDrawMode.Triangle);
            buSelectArrow.Click += (s, e) => drawingModeManager.SetMode(DrawingModeManager.MyDrawMode.Arrow);
            buSelectStar.Click += (s, e) => drawingModeManager.SetMode(DrawingModeManager.MyDrawMode.Star);
            buSelectHexagon.Click += (s, e) => drawingModeManager.SetMode(DrawingModeManager.MyDrawMode.Hexagon);
            buModeSelect.Click += (s, e) => drawingModeManager.SetMode(DrawingModeManager.MyDrawMode.Select);

            buSaveAsToFile.Click += BuSaveAsToFile_Click;
            buLoadFromFile.Click += BuLoadFromFile_Click;
            buNewImage.Click += BuNewImage_Click;

            trPenSize.Minimum = 1;
            trPenSize.Maximum = 12;
            trPenSize.Value = (int)drawingManager.Pen.Width;
            trPenSize.ValueChanged += (s, e) => drawingManager.ChangePenWidth(trPenSize.Value);
        }

        private void HookEvents()
        {
            pxImage.MouseDown += PxImage_MouseDown;
            pxImage.MouseMove += PxImage_MouseMove;
            pxImage.MouseUp += PxImage_MouseUp;
            pxImage.Paint += PxImage_Paint;
        }

        private void PxImage_Paint(object? sender, PaintEventArgs e)
            {
                e.Graphics.DrawImage(drawingManager.Bitmap, 0, 0);

            if (drawingModeManager.CurrentMode == DrawingModeManager.MyDrawMode.Select && selectionManager.HasSelection)
                {
                if (selectionManager.SelectedBitmap != null)
                {
                    e.Graphics.DrawImage(selectionManager.SelectedBitmap, selectionManager.SelectedArea);
                }

                using Pen dashed = new(Color.Black) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
                e.Graphics.DrawRectangle(dashed, selectionManager.SelectedArea);
        }
        }

        private void BuNewImage_Click(object? sender, EventArgs e)
        {
            drawingManager = new DrawingManager(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, drawingManager.Pen.Color, (int)drawingManager.Pen.Width);
            trPenSize.Value = (int)drawingManager.Pen.Width;
            pxImage.Invalidate();
            imageInfoManager.AttachToDrawingManager(drawingManager);
        }

        private void BuSaveAsToFile_Click(object? sender, EventArgs e)
        {
            using SaveFileDialog dialog = new() { Filter = "PNG Files(*.PNG)|*.PNG" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                drawingManager.Bitmap.Save(dialog.FileName);
            }
        }

        private void BuLoadFromFile_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialog = new() { Filter = "PNG Files(*.PNG)|*.PNG" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                drawingManager.LoadImage(dialog.FileName);
                pxImage.Invalidate();
            }
        }

        private void PxImage_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Пипетка: сменить цвет пера на цвет пикселя
                if (e.X >= 0 && e.X < drawingManager.Bitmap.Width && e.Y >= 0 && e.Y < drawingManager.Bitmap.Height)
                {
                    Color pixelColor = drawingManager.Bitmap.GetPixel(e.X, e.Y);
                    drawingManager.ChangeColor(pixelColor);
                }
                return;
            }

            if (drawingModeManager.CurrentMode == DrawingModeManager.MyDrawMode.Select)
                {
                if (selectionManager.HasSelection && selectionManager.SelectedArea.Contains(e.Location))
                {
                    selectionManager.StartDragging(e.Location);
                }
                else
                {
                    startLocation = e.Location;
                    isDrawing = true;
                    selectionManager.StartSelection(e.Location);
                    drawingManager.BackupImage();
            }
            }
            else
            {
                startLocation = e.Location;
                isDrawing = true;
                drawingManager.BackupImage();
            }
        }


        private void PxImage_MouseMove(object? sender, MouseEventArgs e)
        {
            imageInfoManager.UpdateMousePosition(e.Location);

            if (e.Button != MouseButtons.Left) return;

            if (drawingModeManager.CurrentMode == DrawingModeManager.MyDrawMode.Select)
            {
                if (selectionManager.IsDragging)
                {
                    selectionManager.DragTo(e.Location);
                pxImage.Invalidate();
                return;
            }
                else if (isDrawing)
                {
                    selectionManager.UpdateSelection(drawingManager, e.Location);
                    pxImage.Invalidate();
                }

                pxImage.Invalidate();
            }

            else
            {
                if (!isDrawing) return;

                if (drawingModeManager.CurrentMode != DrawingModeManager.MyDrawMode.Pencil)
                drawingManager.RestoreBackup();

                FigureDrawer.DrawFigure(drawingManager.Graphics, drawingManager.Pen, drawingModeManager.CurrentMode, startLocation, e.Location);

                if (drawingModeManager.CurrentMode == DrawingModeManager.MyDrawMode.Pencil)
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
            if (drawingModeManager.CurrentMode == DrawingModeManager.MyDrawMode.Select)
            {
                if (selectionManager.IsDragging)
            {
                    selectionManager.FinishDragging(drawingManager);
                pxImage.Invalidate();
            }
                else
                {
                    isDrawing = false;
                }
        }
            else
        {
                isDrawing = false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.V))
            {
                clipboardManager.PasteFromClipboard(pxImage);
                return true;
            }

            if (selectionManager.HasSelection)
            {
                if (keyData == (Keys.Control | Keys.C))
                {
                    clipboardManager.CopySelectionToClipboard();
                    return true;
                }

                if (keyData == (Keys.Control | Keys.X))
                {
                    clipboardManager.CopySelectionToClipboard();
                    selectionManager.DeleteSelection(drawingManager);
                    pxImage.Invalidate();
                    return true;
                }

                if (keyData == Keys.Delete)
                {
                    selectionManager.DeleteSelection(drawingManager);
                    pxImage.Invalidate();
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
