

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
            Triangle
        }
        private Bitmap b;
        private Graphics g;
        private Pen myPen;
        private Point startLocation;
        private Bitmap bb;
        private MyDrawMode myDrawMode = MyDrawMode.Pencil;

        public wfaPaint()
        {
            InitializeComponent();

            b = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            g = Graphics.FromImage(b);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            myPen = new Pen(panel2.BackColor, 10);
            myPen.StartCap = myPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            panel2.Click += (s, e) => myPen.Color = panel2.BackColor;
            panel3.Click += (s, e) => myPen.Color = panel3.BackColor;
            panel4.Click += (s, e) => myPen.Color = panel4.BackColor;
            panel5.Click += (s, e) => myPen.Color = panel5.BackColor;

            buSelectPencil.Click += (s, e) => myDrawMode = MyDrawMode.Pencil;
            buSelectLine.Click += (s, e) => myDrawMode = MyDrawMode.Line;
            buSelectElipse.Click += (s, e) => myDrawMode = MyDrawMode.Ellipse;
            buSelectRectangle.Click += (s, e) => myDrawMode = MyDrawMode.Rectangle;
            buSelectTriangle.Click += (s, e) => myDrawMode = MyDrawMode.Triangle;
            buSaveAsToFile.Click += BuSaveAsToFile_Click;
            buLoadFromFile.Click += BuLoadFromFile_Click;

            buNewImage.Click += BuNewImage_Click;

            trPenWidth.Minimum = 1;
            trPenWidth.Maximum = 12;
            trPenWidth.Value = Convert.ToInt32(myPen.Width);
            trPenWidth.ValueChanged += (s, e) => myPen.Width = trPenWidth.Value;

            pxImage.MouseDown += PxImage_MouseDown;
            pxImage.MouseMove += PxImage_MouseMove;
            pxImage.MouseUp += PxImage_MouseUp;
            pxImage.Paint += (s, e) => e.Graphics.DrawImage(b, 0, 0);
        }

        private void BuNewImage_Click(object? sender, EventArgs e)
        {
            g.Dispose();
            b.Dispose();
            b = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            g = Graphics.FromImage(b);
            pxImage.Invalidate();
        }

        private void BuSaveAsToFile_Click(object? sender, EventArgs e)
        {
            SaveFileDialog dialog = new();
            dialog.Filter = "PNG Files(*.PNG)|*.PNG";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                b.Save(dialog.FileName);
            }
        }

        private void BuLoadFromFile_Click(object? sender, EventArgs e)
        {
            OpenFileDialog dialog = new();
            dialog.Filter = "PNG Files(*.PNG)|*.PNG";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                g.Dispose();
                b.Dispose();
                b = (Bitmap)Bitmap.FromFile(dialog.FileName);
                g = Graphics.FromImage(b);
                pxImage.Invalidate();

            }
        }

        private void Button1_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PxImage_MouseUp(object? sender, MouseEventArgs e)
        {
            //
        }

        private void PxImage_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //g.DrawLine(myPen, startLocation, e.Location);
                //startLocation = e.Location;

                switch (myDrawMode)
                {
                    case MyDrawMode.Pencil:
                        g.DrawLine(myPen, startLocation, e.Location);
                        startLocation = e.Location;
                        break;
                    case MyDrawMode.Line:
                        RestoreBitmap();
                        g.DrawLine(myPen, startLocation, e.Location);
                        break;
                    case MyDrawMode.Ellipse:
                        RestoreBitmap();
                        g.DrawEllipse(myPen, startLocation.X, startLocation.Y,
                            e.Location.X - startLocation.X, e.Location.Y - startLocation.Y);
                        break;
                    case MyDrawMode.Rectangle:
                        RestoreBitmap();
                        g.DrawRectangle(myPen, startLocation.X, startLocation.Y,
                            e.Location.X - startLocation.X, e.Location.Y - startLocation.Y);
                        // TODO HW: реализовать рисование прямоугольника
                        break;
                    case MyDrawMode.Triangle:
                        RestoreBitmap();
                        var w = e.Location.X - startLocation.X;
                        var h = e.Location.Y - startLocation.Y;
                        var xy1 = new Point(startLocation.X + w / 2, startLocation.Y);
                        var xy2 = new Point(startLocation.X + w, startLocation.Y + h);
                        var xy3 = new Point(startLocation.X, startLocation.Y + h);
                        g.DrawPolygon(myPen, [xy1, xy2, xy3]);
                        // TODO HW: сделать как в пейнте
                        break;
                    default:
                        break;
                }
                pxImage.Invalidate();
            }
        }

        private void RestoreBitmap()
        {
            // TODO восстановить картинку
            // (1) плохо, лагает
            //g.Clear(DefaultBackColor);
            //g.DrawImage(bb, 0, 0);
            // (2)
            g.Dispose();
            b.Dispose();
            b = (Bitmap)bb.Clone();
            g = Graphics.FromImage(b);
        }

        private void PxImage_MouseDown(object? sender, MouseEventArgs e)
        {
            startLocation = e.Location;
            bb = (Bitmap)b.Clone();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }
    }
}
