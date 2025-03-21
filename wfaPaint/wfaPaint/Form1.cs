

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

            button1.Click += (s, e) => myDrawMode = MyDrawMode.Pencil;
            button2.Click += (s, e) => myDrawMode = MyDrawMode.Line;
            button3.Click += (s, e) => myDrawMode = MyDrawMode.Ellipse;
            button4.Click += (s, e) => myDrawMode = MyDrawMode.Rectangle;
            button5.Click += (s, e) => myDrawMode = MyDrawMode.Triangle;

            trPenWidth.Minimum = 1;
            trPenWidth.Maximum = 12;
            trPenWidth.Value = Convert.ToInt32(myPen.Width);
            trPenWidth.ValueChanged += (s, e) => myPen.Width = trPenWidth.Value;

            pxImage.MouseDown += PxImage_MouseDown;
            pxImage.MouseMove += PxImage_MouseMove;
            pxImage.MouseUp += PxImage_MouseUp;
            pxImage.Paint += (s, e) => e.Graphics.DrawImage(b, 0, 0);
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
                        //..
                        break;
                    case MyDrawMode.Rectangle:
                        break;
                    case MyDrawMode.Triangle:
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

      
    }
}
