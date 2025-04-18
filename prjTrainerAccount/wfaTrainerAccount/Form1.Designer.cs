namespace wfaTrainerAccount
{
    partial class Form1
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
            tableLayoutPanel1 = new TableLayoutPanel();
            label2 = new Label();
            label1 = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            buNo = new Button();
            buYes = new Button();
            laQuestion = new Label();
            label4 = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(label2, 1, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Location = new Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(367, 49);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.FromArgb(255, 128, 128);
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 14F);
            label2.Location = new Point(186, 0);
            label2.Name = "label2";
            label2.Size = new Size(178, 49);
            label2.TabIndex = 1;
            label2.Text = "Неверно = 0";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.LimeGreen;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 14F);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(177, 49);
            label1.TabIndex = 0;
            label1.Text = "Верно = 0";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(buNo, 1, 0);
            tableLayoutPanel2.Controls.Add(buYes, 0, 0);
            tableLayoutPanel2.Location = new Point(15, 320);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(361, 125);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // buNo
            // 
            buNo.Dock = DockStyle.Fill;
            buNo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            buNo.ForeColor = Color.Red;
            buNo.Location = new Point(183, 3);
            buNo.Name = "buNo";
            buNo.Size = new Size(175, 119);
            buNo.TabIndex = 1;
            buNo.Text = "Нет";
            buNo.UseVisualStyleBackColor = true;
            // 
            // buYes
            // 
            buYes.Dock = DockStyle.Fill;
            buYes.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            buYes.ForeColor = Color.FromArgb(0, 192, 0);
            buYes.Location = new Point(3, 3);
            buYes.Name = "buYes";
            buYes.Size = new Size(174, 119);
            buYes.TabIndex = 0;
            buYes.Text = "Да";
            buYes.UseVisualStyleBackColor = true;
            // 
            // laQuestion
            // 
            laQuestion.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            laQuestion.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            laQuestion.Location = new Point(15, 73);
            laQuestion.Name = "laQuestion";
            laQuestion.Size = new Size(361, 166);
            laQuestion.TabIndex = 2;
            laQuestion.Text = "10 + 11 = 21";
            laQuestion.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label4.Font = new Font("Segoe UI", 12F);
            label4.ImageAlign = ContentAlignment.BottomLeft;
            label4.Location = new Point(15, 267);
            label4.Name = "label4";
            label4.Size = new Size(364, 29);
            label4.TabIndex = 3;
            label4.Text = "Верно?";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(391, 457);
            Controls.Add(label4);
            Controls.Add(laQuestion);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(tableLayoutPanel1);
            MinimumSize = new Size(409, 504);
            Name = "Form1";
            Text = "Игра \"Устный счёт\"";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel2;
        private Button buYes;
        private Button buNo;
        private Label laQuestion;
        private Label label4;
    }
}
