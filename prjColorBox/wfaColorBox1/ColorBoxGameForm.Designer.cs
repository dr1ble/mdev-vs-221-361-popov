namespace wfaColorBox
{
    partial class ColorBoxGameForm
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
            panelColorGrid = new Panel();
            flowLayoutPanelButtons = new FlowLayoutPanel();
            btnStartLevel = new Button();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // panelColorGrid
            // 
            panelColorGrid.Dock = DockStyle.Top;
            panelColorGrid.Location = new Point(0, 0);
            panelColorGrid.Name = "panelColorGrid";
            panelColorGrid.Size = new Size(565, 288);
            panelColorGrid.TabIndex = 0;
            // 
            // flowLayoutPanelButtons
            // 
            flowLayoutPanelButtons.Dock = DockStyle.Top;
            flowLayoutPanelButtons.Location = new Point(0, 288);
            flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            flowLayoutPanelButtons.Size = new Size(565, 70);
            flowLayoutPanelButtons.TabIndex = 1;
            // 
            // btnStartLevel
            // 
            btnStartLevel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnStartLevel.Location = new Point(43, 420);
            btnStartLevel.Name = "btnStartLevel";
            btnStartLevel.Size = new Size(481, 23);
            btnStartLevel.TabIndex = 2;
            btnStartLevel.Text = "Начать новый уровень";
            btnStartLevel.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.Dock = DockStyle.Top;
            lblStatus.Location = new Point(0, 358);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(565, 59);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "label1";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ColorBoxGameForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(565, 443);
            Controls.Add(lblStatus);
            Controls.Add(btnStartLevel);
            Controls.Add(flowLayoutPanelButtons);
            Controls.Add(panelColorGrid);
            MinimumSize = new Size(581, 482);
            Name = "ColorBoxGameForm";
            Text = "wfaColorBox";
            ResumeLayout(false);
        }

        #endregion

        private Panel panelColorGrid;
        private FlowLayoutPanel flowLayoutPanelButtons;
        private Button btnStartLevel;
        private Label lblStatus;
    }
}
