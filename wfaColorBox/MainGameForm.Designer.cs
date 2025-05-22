namespace wfaColorBox
{
    partial class MainGameForm
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
            gamePanel = new Panel();
            redButton = new Button();
            greenButton = new Button();
            blueButton = new Button();
            yellowButton = new Button();
            newGameButton = new Button();
            statusLabel = new Label();
            SuspendLayout();
            // 
            // gamePanel
            // 
            gamePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gamePanel.BorderStyle = BorderStyle.FixedSingle;
            gamePanel.Location = new Point(-1, 0);
            gamePanel.MinimumSize = new Size(100, 100);
            gamePanel.Name = "gamePanel";
            gamePanel.Size = new Size(458, 137);
            gamePanel.TabIndex = 0;
            // 
            // redButton
            // 
            redButton.Anchor = AnchorStyles.Bottom;
            redButton.BackColor = Color.Red;
            redButton.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            redButton.ForeColor = Color.Black;
            redButton.Location = new Point(16, 206);
            redButton.Name = "redButton";
            redButton.Size = new Size(101, 39);
            redButton.TabIndex = 1;
            redButton.Text = "Красный";
            redButton.UseVisualStyleBackColor = false;
            // 
            // greenButton
            // 
            greenButton.Anchor = AnchorStyles.Bottom;
            greenButton.BackColor = Color.Lime;
            greenButton.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            greenButton.ForeColor = Color.Black;
            greenButton.Location = new Point(118, 206);
            greenButton.Name = "greenButton";
            greenButton.Size = new Size(101, 39);
            greenButton.TabIndex = 2;
            greenButton.Text = "Зеленый";
            greenButton.UseVisualStyleBackColor = false;
            // 
            // blueButton
            // 
            blueButton.Anchor = AnchorStyles.Bottom;
            blueButton.BackColor = Color.DeepSkyBlue;
            blueButton.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            blueButton.ForeColor = Color.Black;
            blueButton.Location = new Point(220, 206);
            blueButton.Name = "blueButton";
            blueButton.Size = new Size(101, 39);
            blueButton.TabIndex = 3;
            blueButton.Text = "Синий";
            blueButton.UseVisualStyleBackColor = false;
            // 
            // yellowButton
            // 
            yellowButton.Anchor = AnchorStyles.Bottom;
            yellowButton.BackColor = Color.Yellow;
            yellowButton.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            yellowButton.ForeColor = Color.Black;
            yellowButton.Location = new Point(322, 206);
            yellowButton.Name = "yellowButton";
            yellowButton.Size = new Size(101, 39);
            yellowButton.TabIndex = 4;
            yellowButton.Text = "Желтый";
            yellowButton.UseVisualStyleBackColor = false;
            // 
            // newGameButton
            // 
            newGameButton.Anchor = AnchorStyles.Bottom;
            newGameButton.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            newGameButton.Location = new Point(138, 304);
            newGameButton.Name = "newGameButton";
            newGameButton.Size = new Size(183, 30);
            newGameButton.TabIndex = 5;
            newGameButton.Text = "Новая игра";
            newGameButton.UseVisualStyleBackColor = true;
            // 
            // statusLabel
            // 
            statusLabel.Anchor = AnchorStyles.Bottom;
            statusLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            statusLabel.Location = new Point(16, 161);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(407, 34);
            statusLabel.TabIndex = 6;
            statusLabel.Text = "Нажмите 'Новая игра' для начала.";
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainGameForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(454, 346);
            Controls.Add(statusLabel);
            Controls.Add(newGameButton);
            Controls.Add(yellowButton);
            Controls.Add(blueButton);
            Controls.Add(greenButton);
            Controls.Add(redButton);
            Controls.Add(gamePanel);
            MinimumSize = new Size(470, 385);
            Name = "MainGameForm";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Panel gamePanel;
        private Button redButton;
        private Button greenButton;
        private Button blueButton;
        private Button yellowButton;
        private Button newGameButton;
        private Label statusLabel;
    }
}
