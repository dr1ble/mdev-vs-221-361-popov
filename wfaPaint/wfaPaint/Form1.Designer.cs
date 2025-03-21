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
            button1 = new Button();
            trPenWidth = new TrackBar();
            panel5 = new Panel();
            panel4 = new Panel();
            panel3 = new Panel();
            panel2 = new Panel();
            pxImage = new PictureBox();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trPenWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pxImage).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(button5);
            panel1.Controls.Add(button4);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(button1);
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
            // button1
            // 
            button1.Location = new Point(12, 102);
            button1.Name = "button1";
            button1.Size = new Size(232, 29);
            button1.TabIndex = 5;
            button1.Text = "Карандаш";
            button1.UseVisualStyleBackColor = true;
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
            // button2
            // 
            button2.Location = new Point(12, 137);
            button2.Name = "button2";
            button2.Size = new Size(232, 29);
            button2.TabIndex = 6;
            button2.Text = "Линия";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(12, 172);
            button3.Name = "button3";
            button3.Size = new Size(232, 29);
            button3.TabIndex = 7;
            button3.Text = "Элипс";
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
            button5.Location = new Point(12, 242);
            button5.Name = "button5";
            button5.Size = new Size(232, 29);
            button5.TabIndex = 9;
            button5.Text = "Треугольник";
            button5.UseVisualStyleBackColor = true;
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
        private Button button1;
        private Button button2;
        private Button button5;
        private Button button4;
        private Button button3;
    }
}
