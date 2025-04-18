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
            panelTools = new Panel();
            buModeSelect = new Button();
            label1 = new Label();
            buSelectHexagon = new Button();
            buSelectStar = new Button();
            buSelectArrow = new Button();
            buNewImage = new Button();
            buLoadFromFile = new Button();
            buSaveAsToFile = new Button();
            buSelectTriangle = new Button();
            buSelectRectangle = new Button();
            buSelectEllipse = new Button();
            buSelectLine = new Button();
            buSelectPen = new Button();
            trPenSize = new TrackBar();
            paSelectColorBlack = new Panel();
            paSelectColorYellow = new Panel();
            paSelectColorGreen = new Panel();
            paSelectColorRed = new Panel();
            trPenWidth = new TrackBar();
            panel5 = new Panel();
            panel4 = new Panel();
            panel3 = new Panel();
            panel2 = new Panel();
            pxImage = new PictureBox();
            statusStrip1 = new StatusStrip();
            toolStripLabelImageInfo = new ToolStripStatusLabel();
            panel1.SuspendLayout();
            panelTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trPenSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trPenWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pxImage).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(panelTools);
            panel1.Controls.Add(trPenWidth);
            panel1.Controls.Add(panel5);
            panel1.Controls.Add(panel4);
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 631);
            panel1.TabIndex = 0;
            // 
            // panelTools
            // 
            panelTools.BackColor = SystemColors.ControlLightLight;
            panelTools.BorderStyle = BorderStyle.FixedSingle;
            panelTools.Controls.Add(buModeSelect);
            panelTools.Controls.Add(label1);
            panelTools.Controls.Add(buSelectHexagon);
            panelTools.Controls.Add(buSelectStar);
            panelTools.Controls.Add(buSelectArrow);
            panelTools.Controls.Add(buNewImage);
            panelTools.Controls.Add(buLoadFromFile);
            panelTools.Controls.Add(buSaveAsToFile);
            panelTools.Controls.Add(buSelectTriangle);
            panelTools.Controls.Add(buSelectRectangle);
            panelTools.Controls.Add(buSelectEllipse);
            panelTools.Controls.Add(buSelectLine);
            panelTools.Controls.Add(buSelectPen);
            panelTools.Controls.Add(trPenSize);
            panelTools.Controls.Add(paSelectColorBlack);
            panelTools.Controls.Add(paSelectColorYellow);
            panelTools.Controls.Add(paSelectColorGreen);
            panelTools.Controls.Add(paSelectColorRed);
            panelTools.Dock = DockStyle.Left;
            panelTools.Location = new Point(0, 0);
            panelTools.Name = "panelTools";
            panelTools.Size = new Size(250, 631);
            panelTools.TabIndex = 11;
            // 
            // buModeSelect
            // 
            buModeSelect.Location = new Point(12, 262);
            buModeSelect.Name = "buModeSelect";
            buModeSelect.Size = new Size(113, 29);
            buModeSelect.TabIndex = 16;
            buModeSelect.Text = "Выделение";
            buModeSelect.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F);
            label1.Location = new Point(91, 239);
            label1.Name = "label1";
            label1.Size = new Size(76, 23);
            label1.TabIndex = 15;
            label1.Text = "Режимы";
            // 
            // buSelectHexagon
            // 
            buSelectHexagon.Font = new Font("Segoe UI", 8F);
            buSelectHexagon.Location = new Point(128, 207);
            buSelectHexagon.Name = "buSelectHexagon";
            buSelectHexagon.Size = new Size(119, 29);
            buSelectHexagon.TabIndex = 14;
            buSelectHexagon.Text = "Шестиугольник";
            buSelectHexagon.UseVisualStyleBackColor = true;
            // 
            // buSelectStar
            // 
            buSelectStar.Font = new Font("Segoe UI", 8F);
            buSelectStar.Location = new Point(12, 207);
            buSelectStar.Name = "buSelectStar";
            buSelectStar.Size = new Size(113, 29);
            buSelectStar.TabIndex = 13;
            buSelectStar.Text = "Звезда";
            buSelectStar.UseVisualStyleBackColor = true;
            // 
            // buSelectArrow
            // 
            buSelectArrow.Font = new Font("Segoe UI", 8F);
            buSelectArrow.Location = new Point(128, 172);
            buSelectArrow.Name = "buSelectArrow";
            buSelectArrow.Size = new Size(119, 29);
            buSelectArrow.TabIndex = 12;
            buSelectArrow.Text = "Стрела";
            buSelectArrow.UseVisualStyleBackColor = true;
            // 
            // buNewImage
            // 
            buNewImage.Location = new Point(11, 484);
            buNewImage.Name = "buNewImage";
            buNewImage.Size = new Size(232, 29);
            buNewImage.TabIndex = 11;
            buNewImage.Text = "Новая картинка";
            buNewImage.UseVisualStyleBackColor = true;
            // 
            // buLoadFromFile
            // 
            buLoadFromFile.Location = new Point(12, 570);
            buLoadFromFile.Name = "buLoadFromFile";
            buLoadFromFile.Size = new Size(232, 29);
            buLoadFromFile.TabIndex = 10;
            buLoadFromFile.Text = "Загрузить из файла";
            buLoadFromFile.UseVisualStyleBackColor = true;
            // 
            // buSaveAsToFile
            // 
            buSaveAsToFile.Location = new Point(12, 535);
            buSaveAsToFile.Name = "buSaveAsToFile";
            buSaveAsToFile.Size = new Size(232, 29);
            buSaveAsToFile.TabIndex = 2;
            buSaveAsToFile.Text = "Сохранить в файл";
            buSaveAsToFile.UseVisualStyleBackColor = true;
            // 
            // buSelectTriangle
            // 
            buSelectTriangle.Font = new Font("Segoe UI", 8F);
            buSelectTriangle.Location = new Point(12, 172);
            buSelectTriangle.Name = "buSelectTriangle";
            buSelectTriangle.Size = new Size(113, 29);
            buSelectTriangle.TabIndex = 9;
            buSelectTriangle.Text = "Треугольник";
            buSelectTriangle.UseVisualStyleBackColor = true;
            // 
            // buSelectRectangle
            // 
            buSelectRectangle.Font = new Font("Segoe UI", 8F);
            buSelectRectangle.Location = new Point(128, 137);
            buSelectRectangle.Name = "buSelectRectangle";
            buSelectRectangle.Size = new Size(119, 29);
            buSelectRectangle.TabIndex = 8;
            buSelectRectangle.Text = "Прямоугольник";
            buSelectRectangle.UseVisualStyleBackColor = true;
            // 
            // buSelectEllipse
            // 
            buSelectEllipse.Font = new Font("Segoe UI", 8F);
            buSelectEllipse.Location = new Point(12, 137);
            buSelectEllipse.Name = "buSelectEllipse";
            buSelectEllipse.Size = new Size(113, 29);
            buSelectEllipse.TabIndex = 7;
            buSelectEllipse.Text = "Элипс";
            buSelectEllipse.UseVisualStyleBackColor = true;
            // 
            // buSelectLine
            // 
            buSelectLine.Font = new Font("Segoe UI", 8F);
            buSelectLine.Location = new Point(128, 102);
            buSelectLine.Name = "buSelectLine";
            buSelectLine.Size = new Size(119, 29);
            buSelectLine.TabIndex = 6;
            buSelectLine.Text = "Линия";
            buSelectLine.UseVisualStyleBackColor = true;
            // 
            // buSelectPen
            // 
            buSelectPen.Font = new Font("Segoe UI", 8F);
            buSelectPen.Location = new Point(12, 102);
            buSelectPen.Name = "buSelectPen";
            buSelectPen.Size = new Size(113, 29);
            buSelectPen.TabIndex = 5;
            buSelectPen.Text = "Карандаш";
            buSelectPen.UseVisualStyleBackColor = true;
            // 
            // trPenSize
            // 
            trPenSize.Location = new Point(12, 59);
            trPenSize.Name = "trPenSize";
            trPenSize.Size = new Size(232, 56);
            trPenSize.TabIndex = 4;
            // 
            // paSelectColorBlack
            // 
            paSelectColorBlack.BackColor = Color.Black;
            paSelectColorBlack.Location = new Point(188, 12);
            paSelectColorBlack.Name = "paSelectColorBlack";
            paSelectColorBlack.Size = new Size(42, 41);
            paSelectColorBlack.TabIndex = 3;
            // 
            // paSelectColorYellow
            // 
            paSelectColorYellow.BackColor = Color.Yellow;
            paSelectColorYellow.Location = new Point(131, 12);
            paSelectColorYellow.Name = "paSelectColorYellow";
            paSelectColorYellow.Size = new Size(42, 41);
            paSelectColorYellow.TabIndex = 2;
            // 
            // paSelectColorGreen
            // 
            paSelectColorGreen.BackColor = Color.Lime;
            paSelectColorGreen.Location = new Point(73, 12);
            paSelectColorGreen.Name = "paSelectColorGreen";
            paSelectColorGreen.Size = new Size(42, 41);
            paSelectColorGreen.TabIndex = 1;
            // 
            // paSelectColorRed
            // 
            paSelectColorRed.BackColor = Color.Red;
            paSelectColorRed.BorderStyle = BorderStyle.FixedSingle;
            paSelectColorRed.Location = new Point(12, 12);
            paSelectColorRed.Name = "paSelectColorRed";
            paSelectColorRed.Size = new Size(42, 41);
            paSelectColorRed.TabIndex = 0;
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
            pxImage.Size = new Size(733, 631);
            pxImage.TabIndex = 1;
            pxImage.TabStop = false;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabelImageInfo });
            statusStrip1.Location = new Point(250, 605);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(733, 26);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripLabelImageInfo
            // 
            toolStripLabelImageInfo.Name = "toolStripLabelImageInfo";
            toolStripLabelImageInfo.Size = new Size(64, 20);
            toolStripLabelImageInfo.Text = "Image: -";
            // 
            // wfaPaint
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(983, 631);
            Controls.Add(statusStrip1);
            Controls.Add(pxImage);
            Controls.Add(panel1);
            Name = "wfaPaint";
            Text = "wfaPaint";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panelTools.ResumeLayout(false);
            panelTools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trPenSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)trPenWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)pxImage).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private PictureBox pxImage;
        private TrackBar trPenWidth;
        private Panel panel5;
        private Panel panel4;
        private Panel panel3;
        private Panel panel2;
        private Panel panelTools;
        private Button buNewImage;
        private Button buLoadFromFile;
        private Button buSaveAsToFile;
        private Button buSelectTriangle;
        private Button buSelectRectangle;
        private Button buSelectEllipse;
        private Button buSelectLine;
        private TrackBar trPenSize;
        private Panel paSelectColorBlack;
        private Panel paSelectColorYellow;
        private Panel paSelectColorGreen;
        private Panel paSelectColorRed;
        private Button buSelectPen;
        private Button buSelectHexagon;
        private Button buSelectStar;
        private Button buSelectArrow;
        private Button buModeSelect;
        private Label label1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripLabelImageInfo;
    }
}
