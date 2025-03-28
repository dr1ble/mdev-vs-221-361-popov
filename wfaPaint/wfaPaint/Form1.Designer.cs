 namespace wfaPaint
{
    partial class wfaPaint
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            panel6 = new Panel();
            buNewImage = new Button();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            button7 = new Button();
            trackBar1 = new TrackBar();
            panel7 = new Panel();
            panel8 = new Panel();
            panel9 = new Panel();
            panel10 = new Panel();
            buLoadFromFile = new Button();
            buSaveAsToFile = new Button();
            buSelectTriangle = new Button();
            buSelectRectangle = new Button();
            buSelectElipse = new Button();
            buSelectLine = new Button();
            buSelectPencil = new Button();
            trPenWidth = new TrackBar();
            panel5 = new Panel();
            panel4 = new Panel();
            panel3 = new Panel();
            panel2 = new Panel();
            pxImage = new PictureBox();
            panel1.SuspendLayout();
            panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trPenWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pxImage).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(panel6);
            panel1.Controls.Add(buLoadFromFile);
            panel1.Controls.Add(buSaveAsToFile);
            panel1.Controls.Add(buSelectTriangle);
            panel1.Controls.Add(buSelectRectangle);
            panel1.Controls.Add(buSelectElipse);
            panel1.Controls.Add(buSelectLine);
            panel1.Controls.Add(buSelectPencil);
            panel1.Controls.Add(trPenWidth);
            panel1.Controls.Add(panel5);
            panel1.Controls.Add(panel4);
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 450);
            panel1.TabIndex = 0;
            // 
            // panel6
            // 
            panel6.Controls.Add(buNewImage);
            panel6.Controls.Add(button1);
            panel6.Controls.Add(button2);
            panel6.Controls.Add(button3);
            panel6.Controls.Add(button4);
            panel6.Controls.Add(button5);
            panel6.Controls.Add(button6);
            panel6.Controls.Add(button7);
            panel6.Controls.Add(trackBar1);
            panel6.Controls.Add(panel7);
            panel6.Controls.Add(panel8);
            panel6.Controls.Add(panel9);
            panel6.Controls.Add(panel10);
            panel6.Dock = DockStyle.Left;
            panel6.Location = new Point(0, 0);
            panel6.Name = "panel6";
            panel6.Size = new Size(250, 450);
            panel6.TabIndex = 11;
            // 
            // buNewImage
            // 
            buNewImage.Location = new Point(21, 306);
            buNewImage.Name = "buNewImage";
            buNewImage.Size = new Size(223, 29);
            buNewImage.TabIndex = 11;
            buNewImage.Text = "Новая картинка";
            buNewImage.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(21, 409);
            button1.Name = "button1";
            button1.Size = new Size(223, 29);
            button1.TabIndex = 10;
            button1.Text = "Загрузить из файла";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(21, 374);
            button2.Name = "button2";
            button2.Size = new Size(223, 29);
            button2.TabIndex = 2;
            button2.Text = "Сохранить в файл";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(12, 242);
            button3.Name = "button3";
            button3.Size = new Size(232, 29);
            button3.TabIndex = 9;
            button3.Text = "Треугольник";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(12, 207);
            button4.Name = "button4";
            button4.Size = new Size(232, 29);
            button4.TabIndex = 8;
            button4.Text = "Прямоугольник";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(12, 172);
            button5.Name = "button5";
            button5.Size = new Size(232, 29);
            button5.TabIndex = 7;
            button5.Text = "Элипс";
            button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Location = new Point(12, 137);
            button6.Name = "button6";
            button6.Size = new Size(232, 29);
            button6.TabIndex = 6;
            button6.Text = "Линия";
            button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.Location = new Point(12, 102);
            button7.Name = "button7";
            button7.Size = new Size(232, 29);
            button7.TabIndex = 5;
            button7.Text = "Карандаш";
            button7.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(12, 59);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(232, 56);
            trackBar1.TabIndex = 4;
            // 
            // panel7
            // 
            panel7.BackColor = Color.Black;
            panel7.Location = new Point(188, 12);
            panel7.Name = "panel7";
            panel7.Size = new Size(42, 41);
            panel7.TabIndex = 3;
            // 
            // panel8
            // 
            panel8.BackColor = Color.Yellow;
            panel8.Location = new Point(131, 12);
            panel8.Name = "panel8";
            panel8.Size = new Size(42, 41);
            panel8.TabIndex = 2;
            // 
            // panel9
            // 
            panel9.BackColor = Color.Lime;
            panel9.Location = new Point(73, 12);
            panel9.Name = "panel9";
            panel9.Size = new Size(42, 41);
            panel9.TabIndex = 1;
            // 
            // panel10
            // 
            panel10.BackColor = Color.Red;
            panel10.Location = new Point(12, 12);
            panel10.Name = "panel10";
            panel10.Size = new Size(42, 41);
            panel10.TabIndex = 0;
            // 
            // buLoadFromFile
            // 
            buLoadFromFile.Location = new Point(21, 409);
            buLoadFromFile.Name = "buLoadFromFile";
            buLoadFromFile.Size = new Size(223, 29);
            buLoadFromFile.TabIndex = 10;
            buLoadFromFile.Text = "Загрузить из файла";
            buLoadFromFile.UseVisualStyleBackColor = true;
            buLoadFromFile.Click += button7_Click;
            // 
            // buSaveAsToFile
            // 
            buSaveAsToFile.Location = new Point(21, 374);
            buSaveAsToFile.Name = "buSaveAsToFile";
            buSaveAsToFile.Size = new Size(223, 29);
            buSaveAsToFile.TabIndex = 2;
            buSaveAsToFile.Text = "Сохранить в файл";
            buSaveAsToFile.UseVisualStyleBackColor = true;
            buSaveAsToFile.Click += button6_Click;
            // 
            // buSelectTriangle
            // 
            buSelectTriangle.Location = new Point(12, 242);
            buSelectTriangle.Name = "buSelectTriangle";
            buSelectTriangle.Size = new Size(232, 29);
            buSelectTriangle.TabIndex = 9;
            buSelectTriangle.Text = "Треугольник";
            buSelectTriangle.UseVisualStyleBackColor = true;
            // 
            // buSelectRectangle
            // 
            buSelectRectangle.Location = new Point(12, 207);
            buSelectRectangle.Name = "buSelectRectangle";
            buSelectRectangle.Size = new Size(232, 29);
            buSelectRectangle.TabIndex = 8;
            buSelectRectangle.Text = "Прямоугольник";
            buSelectRectangle.UseVisualStyleBackColor = true;
            // 
            // buSelectElipse
            // 
            buSelectElipse.Location = new Point(12, 172);
            buSelectElipse.Name = "buSelectElipse";
            buSelectElipse.Size = new Size(232, 29);
            buSelectElipse.TabIndex = 7;
            buSelectElipse.Text = "Элипс";
            buSelectElipse.UseVisualStyleBackColor = true;
            // 
            // buSelectLine
            // 
            buSelectLine.Location = new Point(12, 137);
            buSelectLine.Name = "buSelectLine";
            buSelectLine.Size = new Size(232, 29);
            buSelectLine.TabIndex = 6;
            buSelectLine.Text = "Линия";
            buSelectLine.UseVisualStyleBackColor = true;
            // 
            // buSelectPencil
            // 
            buSelectPencil.Location = new Point(12, 102);
            buSelectPencil.Name = "buSelectPencil";
            buSelectPencil.Size = new Size(232, 29);
            buSelectPencil.TabIndex = 5;
            buSelectPencil.Text = "Карандаш";
            buSelectPencil.UseVisualStyleBackColor = true;
            // 
            // trPenWidth
            // 
            trPenWidth.Location = new Point(12, 59);
            trPenWidth.Name = "trPenWidth";
            trPenWidth.Size = new Size(232, 56);
            trPenWidth.TabIndex = 4;
            // 
            // panel5
            // 
            panel5.BackColor = Color.Black;
            panel5.Location = new Point(188, 12);
            panel5.Name = "panel5";
            panel5.Size = new Size(42, 41);
            panel5.TabIndex = 3;
            // 
            // panel4
            // 
            panel4.BackColor = Color.Yellow;
            panel4.Location = new Point(131, 12);
            panel4.Name = "panel4";
            panel4.Size = new Size(42, 41);
            panel4.TabIndex = 2;
            // 
            // panel3
            // 
            panel3.BackColor = Color.Lime;
            panel3.Location = new Point(73, 12);
            panel3.Name = "panel3";
            panel3.Size = new Size(42, 41);
            panel3.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Red;
            panel2.Location = new Point(12, 12);
            panel2.Name = "panel2";
            panel2.Size = new Size(42, 41);
            panel2.TabIndex = 0;
            // 
            // pxImage
            // 
            pxImage.Dock = DockStyle.Fill;
            pxImage.Location = new Point(250, 0);
            pxImage.Name = "pxImage";
            pxImage.Size = new Size(550, 450);
            pxImage.TabIndex = 1;
            pxImage.TabStop = false;
            // 
            // wfaPaint
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pxImage);
            Controls.Add(panel1);
            Name = "wfaPaint";
            Text = "wfaPaint";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trPenWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)pxImage).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private PictureBox pxImage;
        private TrackBar trPenWidth;
        private Panel panel5;
        private Panel panel4;
        private Panel panel3;
        private Panel panel2;
        private Button buSelectPencil;
        private Button buSelectLine;
        private Button buSelectTriangle;
        private Button buSelectRectangle;
        private Button buSelectElipse;
        private Button buLoadFromFile;
        private Button buSaveAsToFile;
        private Panel panel6;
        private Button buNewImage;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private TrackBar trackBar1;
        private Panel panel7;
        private Panel panel8;
        private Panel panel9;
        private Panel panel10;
    }
}
