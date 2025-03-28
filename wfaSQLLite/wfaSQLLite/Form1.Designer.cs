namespace wfaSQLLite
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
            lvLogs = new ListView();
            edCityName = new TextBox();
            buCityAdd = new Button();
            buCityShow = new Button();
            dataGridView1 = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // lvLogs
            // 
            lvLogs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lvLogs.Location = new Point(12, 12);
            lvLogs.Name = "lvLogs";
            lvLogs.Size = new Size(282, 426);
            lvLogs.TabIndex = 0;
            lvLogs.UseCompatibleStateImageBehavior = false;
            // 
            // textBox1
            // 
            edCityName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            edCityName.Location = new Point(300, 12);
            edCityName.Name = "textBox1";
            edCityName.Size = new Size(291, 27);
            edCityName.TabIndex = 1;
            // 
            // button1
            // 
            buCityAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buCityAdd.Location = new Point(643, 12);
            buCityAdd.Name = "button1";
            buCityAdd.Size = new Size(145, 29);
            buCityAdd.TabIndex = 2;
            buCityAdd.Text = "Добавить город";
            buCityAdd.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            buCityShow.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            buCityShow.Location = new Point(300, 58);
            buCityShow.Name = "button2";
            buCityShow.Size = new Size(488, 29);
            buCityShow.TabIndex = 3;
            buCityShow.Text = "Показать все города";
            buCityShow.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(300, 93);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(488, 345);
            dataGridView1.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dataGridView1);
            Controls.Add(buCityShow);
            Controls.Add(buCityAdd);
            Controls.Add(edCityName);
            Controls.Add(lvLogs);
            Name = "Form1";
            Text = "wfaSQLLite";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView lvLogs;
        private TextBox edCityName;
        private Button buCityAdd;
        private Button buCityShow;
        private DataGridView dataGridView1;
    }
}
