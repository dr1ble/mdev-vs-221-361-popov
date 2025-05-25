 namespace wfaPaint
{
    partial class MainPaintWindow
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
            pxImage = new PictureBox();
            panelTools = new Panel();
            groupBox3 = new GroupBox();
            buEraser = new Button();
            buModeSelect = new Button();
            groupBox2 = new GroupBox();
            paSelectColorRed = new Panel();
            paSelectColorGreen = new Panel();
            paSelectColorYellow = new Panel();
            paSelectColorCyan = new Panel();
            paSelectColorBlack = new Panel();
            paSelectColorPink = new Panel();
            paSelectColorOrange = new Panel();
            paSelectColorBlue = new Panel();
            groupBox1 = new GroupBox();
            buSelectPen = new Button();
            buSelectLine = new Button();
            buSelectEllipse = new Button();
            buSelectRectangle = new Button();
            buSelectTriangle = new Button();
            buSelectArrow = new Button();
            buSelectStar = new Button();
            buSelectHexagon = new Button();
            buNewImage = new Button();
            buLoadFromFile = new Button();
            buSaveAsToFile = new Button();
            trPenSize = new TrackBar();
            statusStrip1 = new StatusStrip();
            toolStripLabelImageInfo = new ToolStripStatusLabel();
            panel1 = new Panel();
            ((System.ComponentModel.ISupportInitialize)pxImage).BeginInit();
            panelTools.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trPenSize).BeginInit();
            statusStrip1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // pxImage
            // 
            pxImage.Dock = DockStyle.Fill;
            pxImage.Location = new Point(226, 0);
            pxImage.Margin = new Padding(3, 2, 3, 2);
            pxImage.Name = "pxImage";
            pxImage.Size = new Size(637, 485);
            pxImage.TabIndex = 1;
            pxImage.TabStop = false;
            // 
            // panelTools
            // 
            panelTools.BackColor = SystemColors.ControlLightLight;
            panelTools.BorderStyle = BorderStyle.FixedSingle;
            panelTools.Controls.Add(groupBox3);
            panelTools.Controls.Add(groupBox2);
            panelTools.Controls.Add(groupBox1);
            panelTools.Controls.Add(buNewImage);
            panelTools.Controls.Add(buLoadFromFile);
            panelTools.Controls.Add(buSaveAsToFile);
            panelTools.Controls.Add(trPenSize);
            panelTools.Dock = DockStyle.Left;
            panelTools.Location = new Point(0, 0);
            panelTools.Margin = new Padding(3, 2, 3, 2);
            panelTools.Name = "panelTools";
            panelTools.Size = new Size(226, 485);
            panelTools.TabIndex = 11;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(buEraser);
            groupBox3.Controls.Add(buModeSelect);
            groupBox3.Location = new Point(6, 246);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(215, 100);
            groupBox3.TabIndex = 21;
            groupBox3.TabStop = false;
            groupBox3.Text = "Режим";
            // 
            // buEraser
            // 
            buEraser.Font = new Font("Montserrat", 8.249999F);
            buEraser.Location = new Point(111, 21);
            buEraser.Margin = new Padding(3, 2, 3, 2);
            buEraser.Name = "buEraser";
            buEraser.Size = new Size(98, 22);
            buEraser.TabIndex = 17;
            buEraser.Text = "Ластик";
            buEraser.UseVisualStyleBackColor = true;
            // 
            // buModeSelect
            // 
            buModeSelect.Font = new Font("Montserrat", 8.249999F);
            buModeSelect.Location = new Point(6, 21);
            buModeSelect.Margin = new Padding(3, 2, 3, 2);
            buModeSelect.Name = "buModeSelect";
            buModeSelect.Size = new Size(98, 22);
            buModeSelect.TabIndex = 16;
            buModeSelect.Text = "Выделение";
            buModeSelect.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(paSelectColorRed);
            groupBox2.Controls.Add(paSelectColorGreen);
            groupBox2.Controls.Add(paSelectColorYellow);
            groupBox2.Controls.Add(paSelectColorCyan);
            groupBox2.Controls.Add(paSelectColorBlack);
            groupBox2.Controls.Add(paSelectColorPink);
            groupBox2.Controls.Add(paSelectColorOrange);
            groupBox2.Controls.Add(paSelectColorBlue);
            groupBox2.Location = new Point(6, 11);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(215, 60);
            groupBox2.TabIndex = 20;
            groupBox2.TabStop = false;
            groupBox2.Text = "Палитра";
            // 
            // paSelectColorRed
            // 
            paSelectColorRed.BackColor = Color.Red;
            paSelectColorRed.BorderStyle = BorderStyle.FixedSingle;
            paSelectColorRed.Location = new Point(6, 21);
            paSelectColorRed.Margin = new Padding(3, 2, 3, 2);
            paSelectColorRed.Name = "paSelectColorRed";
            paSelectColorRed.Size = new Size(37, 11);
            paSelectColorRed.TabIndex = 0;
            // 
            // paSelectColorGreen
            // 
            paSelectColorGreen.BackColor = Color.Lime;
            paSelectColorGreen.Location = new Point(60, 21);
            paSelectColorGreen.Margin = new Padding(3, 2, 3, 2);
            paSelectColorGreen.Name = "paSelectColorGreen";
            paSelectColorGreen.Size = new Size(37, 11);
            paSelectColorGreen.TabIndex = 1;
            // 
            // paSelectColorYellow
            // 
            paSelectColorYellow.BackColor = Color.Yellow;
            paSelectColorYellow.Location = new Point(111, 21);
            paSelectColorYellow.Margin = new Padding(3, 2, 3, 2);
            paSelectColorYellow.Name = "paSelectColorYellow";
            paSelectColorYellow.Size = new Size(37, 11);
            paSelectColorYellow.TabIndex = 2;
            // 
            // paSelectColorCyan
            // 
            paSelectColorCyan.BackColor = Color.Cyan;
            paSelectColorCyan.BorderStyle = BorderStyle.FixedSingle;
            paSelectColorCyan.Location = new Point(160, 37);
            paSelectColorCyan.Margin = new Padding(3, 2, 3, 2);
            paSelectColorCyan.Name = "paSelectColorCyan";
            paSelectColorCyan.Size = new Size(37, 11);
            paSelectColorCyan.TabIndex = 5;
            // 
            // paSelectColorBlack
            // 
            paSelectColorBlack.BackColor = Color.Black;
            paSelectColorBlack.Location = new Point(160, 21);
            paSelectColorBlack.Margin = new Padding(3, 2, 3, 2);
            paSelectColorBlack.Name = "paSelectColorBlack";
            paSelectColorBlack.Size = new Size(37, 11);
            paSelectColorBlack.TabIndex = 3;
            // 
            // paSelectColorPink
            // 
            paSelectColorPink.BackColor = Color.FromArgb(255, 128, 255);
            paSelectColorPink.BorderStyle = BorderStyle.FixedSingle;
            paSelectColorPink.Location = new Point(111, 37);
            paSelectColorPink.Margin = new Padding(3, 2, 3, 2);
            paSelectColorPink.Name = "paSelectColorPink";
            paSelectColorPink.Size = new Size(37, 11);
            paSelectColorPink.TabIndex = 4;
            // 
            // paSelectColorOrange
            // 
            paSelectColorOrange.BackColor = Color.FromArgb(255, 128, 0);
            paSelectColorOrange.BorderStyle = BorderStyle.FixedSingle;
            paSelectColorOrange.Location = new Point(7, 37);
            paSelectColorOrange.Margin = new Padding(3, 2, 3, 2);
            paSelectColorOrange.Name = "paSelectColorOrange";
            paSelectColorOrange.Size = new Size(37, 11);
            paSelectColorOrange.TabIndex = 2;
            // 
            // paSelectColorBlue
            // 
            paSelectColorBlue.BackColor = Color.Blue;
            paSelectColorBlue.BorderStyle = BorderStyle.FixedSingle;
            paSelectColorBlue.Location = new Point(60, 37);
            paSelectColorBlue.Margin = new Padding(3, 2, 3, 2);
            paSelectColorBlue.Name = "paSelectColorBlue";
            paSelectColorBlue.Size = new Size(37, 11);
            paSelectColorBlue.TabIndex = 3;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.ButtonHighlight;
            groupBox1.Controls.Add(buSelectPen);
            groupBox1.Controls.Add(buSelectLine);
            groupBox1.Controls.Add(buSelectEllipse);
            groupBox1.Controls.Add(buSelectRectangle);
            groupBox1.Controls.Add(buSelectTriangle);
            groupBox1.Controls.Add(buSelectArrow);
            groupBox1.Controls.Add(buSelectStar);
            groupBox1.Controls.Add(buSelectHexagon);
            groupBox1.Location = new Point(6, 115);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(215, 125);
            groupBox1.TabIndex = 19;
            groupBox1.TabStop = false;
            groupBox1.Text = "Кисть";
            // 
            // buSelectPen
            // 
            buSelectPen.Font = new Font("Montserrat", 8.249999F);
            buSelectPen.Location = new Point(6, 18);
            buSelectPen.Margin = new Padding(3, 2, 3, 2);
            buSelectPen.Name = "buSelectPen";
            buSelectPen.Size = new Size(99, 22);
            buSelectPen.TabIndex = 5;
            buSelectPen.Text = "Карандаш";
            buSelectPen.UseVisualStyleBackColor = true;
            // 
            // buSelectLine
            // 
            buSelectLine.Font = new Font("Montserrat", 8.249999F);
            buSelectLine.Location = new Point(109, 17);
            buSelectLine.Margin = new Padding(3, 2, 3, 2);
            buSelectLine.Name = "buSelectLine";
            buSelectLine.Size = new Size(104, 22);
            buSelectLine.TabIndex = 6;
            buSelectLine.Text = "Линия";
            buSelectLine.UseVisualStyleBackColor = true;
            // 
            // buSelectEllipse
            // 
            buSelectEllipse.Font = new Font("Montserrat", 8.249999F);
            buSelectEllipse.Location = new Point(7, 44);
            buSelectEllipse.Margin = new Padding(3, 2, 3, 2);
            buSelectEllipse.Name = "buSelectEllipse";
            buSelectEllipse.Size = new Size(99, 22);
            buSelectEllipse.TabIndex = 7;
            buSelectEllipse.Text = "Элипс";
            buSelectEllipse.UseVisualStyleBackColor = true;
            // 
            // buSelectRectangle
            // 
            buSelectRectangle.Font = new Font("Montserrat", 8.249999F);
            buSelectRectangle.Location = new Point(109, 44);
            buSelectRectangle.Margin = new Padding(3, 2, 3, 2);
            buSelectRectangle.Name = "buSelectRectangle";
            buSelectRectangle.Size = new Size(104, 22);
            buSelectRectangle.TabIndex = 8;
            buSelectRectangle.Text = "Прямоугольник";
            buSelectRectangle.UseVisualStyleBackColor = true;
            // 
            // buSelectTriangle
            // 
            buSelectTriangle.Font = new Font("Montserrat", 8.249999F);
            buSelectTriangle.Location = new Point(7, 70);
            buSelectTriangle.Margin = new Padding(3, 2, 3, 2);
            buSelectTriangle.Name = "buSelectTriangle";
            buSelectTriangle.Size = new Size(99, 22);
            buSelectTriangle.TabIndex = 9;
            buSelectTriangle.Text = "Треугольник";
            buSelectTriangle.UseVisualStyleBackColor = true;
            // 
            // buSelectArrow
            // 
            buSelectArrow.Font = new Font("Montserrat", 8.249999F);
            buSelectArrow.Location = new Point(109, 70);
            buSelectArrow.Margin = new Padding(3, 2, 3, 2);
            buSelectArrow.Name = "buSelectArrow";
            buSelectArrow.Size = new Size(104, 22);
            buSelectArrow.TabIndex = 12;
            buSelectArrow.Text = "Стрела";
            buSelectArrow.UseVisualStyleBackColor = true;
            // 
            // buSelectStar
            // 
            buSelectStar.Font = new Font("Montserrat", 8.249999F);
            buSelectStar.Location = new Point(110, 96);
            buSelectStar.Margin = new Padding(3, 2, 3, 2);
            buSelectStar.Name = "buSelectStar";
            buSelectStar.Size = new Size(103, 22);
            buSelectStar.TabIndex = 13;
            buSelectStar.Text = "Звезда";
            buSelectStar.UseVisualStyleBackColor = true;
            // 
            // buSelectHexagon
            // 
            buSelectHexagon.Font = new Font("Montserrat", 8.249999F);
            buSelectHexagon.Location = new Point(7, 96);
            buSelectHexagon.Margin = new Padding(3, 2, 3, 2);
            buSelectHexagon.Name = "buSelectHexagon";
            buSelectHexagon.Size = new Size(99, 22);
            buSelectHexagon.TabIndex = 14;
            buSelectHexagon.Text = "Шестиугольник";
            buSelectHexagon.UseVisualStyleBackColor = true;
            // 
            // buNewImage
            // 
            buNewImage.Font = new Font("Montserrat", 8.249999F);
            buNewImage.Location = new Point(11, 398);
            buNewImage.Margin = new Padding(3, 2, 3, 2);
            buNewImage.Name = "buNewImage";
            buNewImage.Size = new Size(203, 22);
            buNewImage.TabIndex = 11;
            buNewImage.Text = "Новое изображение";
            buNewImage.UseVisualStyleBackColor = true;
            // 
            // buLoadFromFile
            // 
            buLoadFromFile.Font = new Font("Montserrat", 8.249999F);
            buLoadFromFile.Location = new Point(116, 424);
            buLoadFromFile.Margin = new Padding(3, 2, 3, 2);
            buLoadFromFile.Name = "buLoadFromFile";
            buLoadFromFile.Size = new Size(98, 57);
            buLoadFromFile.TabIndex = 10;
            buLoadFromFile.Text = "Загрузить из файла";
            buLoadFromFile.UseVisualStyleBackColor = true;
            // 
            // buSaveAsToFile
            // 
            buSaveAsToFile.Font = new Font("Montserrat", 8.249999F);
            buSaveAsToFile.Location = new Point(11, 424);
            buSaveAsToFile.Margin = new Padding(3, 2, 3, 2);
            buSaveAsToFile.Name = "buSaveAsToFile";
            buSaveAsToFile.Size = new Size(99, 57);
            buSaveAsToFile.TabIndex = 2;
            buSaveAsToFile.Text = "Сохранить в файл";
            buSaveAsToFile.UseVisualStyleBackColor = true;
            // 
            // trPenSize
            // 
            trPenSize.Location = new Point(6, 76);
            trPenSize.Margin = new Padding(3, 2, 3, 2);
            trPenSize.Name = "trPenSize";
            trPenSize.Size = new Size(215, 45);
            trPenSize.TabIndex = 4;
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = Color.WhiteSmoke;
            statusStrip1.Dock = DockStyle.Top;
            statusStrip1.Font = new Font("Segoe UI", 7F);
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabelImageInfo });
            statusStrip1.Location = new Point(226, 0);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 12, 0);
            statusStrip1.Size = new Size(637, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripLabelImageInfo
            // 
            toolStripLabelImageInfo.Name = "toolStripLabelImageInfo";
            toolStripLabelImageInfo.Size = new Size(77, 17);
            toolStripLabelImageInfo.Text = "Изображение: -";
            // 
            // panel1
            // 
            panel1.Controls.Add(panelTools);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(3, 2, 3, 2);
            panel1.Name = "panel1";
            panel1.Size = new Size(226, 485);
            panel1.TabIndex = 0;
            // 
            // wfaPaint
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(863, 485);
            Controls.Add(statusStrip1);
            Controls.Add(pxImage);
            Controls.Add(panel1);
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(394, 524);
            Name = "wfaPaint";
            Text = "PochtiPaint";
            ((System.ComponentModel.ISupportInitialize)pxImage).EndInit();
            panelTools.ResumeLayout(false);
            panelTools.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)trPenSize).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox pxImage;
        private Panel panelTools;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripLabelImageInfo;
        private Button buModeSelect;
        private Button buSelectHexagon;
        private Button buSelectStar;
        private Button buSelectArrow;
        private Button buNewImage;
        private Button buLoadFromFile;
        private Button buSaveAsToFile;
        private Button buSelectTriangle;
        private Button buSelectRectangle;
        private Button buSelectEllipse;
        private Button buSelectLine;
        private Button buSelectPen;
        private TrackBar trPenSize;
        private Panel paSelectColorBlack;
        private Panel paSelectColorYellow;
        private Panel paSelectColorGreen;
        private Panel paSelectColorRed;
        private Panel panel1;
        private Panel paSelectColorCyan;
        private Panel paSelectColorPink;
        private Panel paSelectColorBlue;
        private Panel paSelectColorOrange;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Button buEraser;
    }
}
