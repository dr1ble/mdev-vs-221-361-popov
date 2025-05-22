using ColorBox.Core; // ���������� ���� �� Core
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D; // ��� ��������� ������
using System.Linq;
using System.Windows.Forms;

namespace wfaColorBox
{
    public partial class MainGameForm : Form
    {
        private GameManager _gameManager;
        private List<GameColor> _availableColors; // ������ ������, ��������� ��� ����
        private List<Button> _colorGuessButtons;  // ������ ��� ���������� ������

        private System.Windows.Forms.Timer _nextLevelTimer; // ������ ��� �������� ����� ��������� �������

        // ��������� ��� ����������� ��������� �� ������� ����
        private const int ItemSize = 40;
        private const int ItemMargin = 5;
        private const int MaxItemsPerRow = 10;

        public MainGameForm()
        {
            InitializeComponent();
            InitializeGame();
            ConnectDesignerUIToLogic();

            _nextLevelTimer = new System.Windows.Forms.Timer
            {
                Interval = 1500 // �������� � �������������
            };
            _nextLevelTimer.Tick += NextLevelTimer_Tick;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (_gameManager != null)
            {
                _gameManager.StartNewLevel();
            }
            else
            {
                MessageBox.Show("����������� ������: GameManager �� ��� ��������������� � ������� OnLoad.", "������ �������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void InitializeGame()
        {
            // ���������� ��������� ����� � �� ������
            _availableColors = new List<GameColor>
            {
                new GameColor(1, "�������", ShapeType.Square),
                new GameColor(2, "�������", ShapeType.Circle),
                new GameColor(3, "�����", ShapeType.Triangle),
                new GameColor(4, "������", ShapeType.Star)
            };

            // ��������� ����
            var gameSettings = new GameSettings(
                availableColors: _availableColors,
                numberOfDistinctColorsToUse: _availableColors.Count, // ���������� ��� 4 ��������� �����
                totalItemsOnMap: 20 // ��������, 20 ���������. (1+2+3+4=10; 20-10=10. 1+2,2+2,3+3,4+3 -> 3,4,6,7)
                                    // ���������, ��� TotalItemsOnMap >= minItemsRequiredForDistinctCounts
            );

            _gameManager = new GameManager(gameSettings);

            // �������� �� ������� GameManager
            _gameManager.LevelStarted += GameManager_LevelStarted;
            _gameManager.GuessProcessed += GameManager_GuessProcessed;
            _gameManager.LevelCompleted += GameManager_LevelCompleted;
        }

        private void ConnectDesignerUIToLogic()
        {
            this.Text = "���� ���������";

            // ��������������, ��� ������ redButton, greenButton, blueButton, yellowButton
            // � newGameButton, � ����� statusLabel � gamePanel ������� � ��������� ����.
            _colorGuessButtons = new List<Button?> { this.redButton, this.greenButton, this.blueButton, this.yellowButton }
                                    .OfType<Button>()
                                    .ToList();

            // ��������� ����� � ����������� ������� ����������
            // ������� ����� � ������ ��������������� _availableColors, ���� ����� ������� �������������
            // ��� ������������ ����� �������� ������ ������ �� ����� �����.
            AssignTagAndClickHandlerForButton(this.redButton, "�������");
            AssignTagAndClickHandlerForButton(this.greenButton, "�������");
            AssignTagAndClickHandlerForButton(this.blueButton, "�����");
            AssignTagAndClickHandlerForButton(this.yellowButton, "������");


            if (this.newGameButton != null)
            {
                this.newGameButton.Click += NewGameButton_Click;
            }
            else
            {
                MessageBox.Show("������� ���������� 'newGameButton' �� ������.", "������ UI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (this.gamePanel == null)
            {
                MessageBox.Show("������� ���������� 'gamePanel' �� ������.", "������ UI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (this.statusLabel == null)
            {
                MessageBox.Show("������� ���������� 'statusLabel' �� ������.", "������ UI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AssignTagAndClickHandlerForButton(Button? button, string colorName)
        {
            if (button == null) return;

            // ������� GameColor �� �����
            GameColor? color = _availableColors.FirstOrDefault(c => c.Name == colorName);
            if (color != null)
            {
                button.Tag = color; // ��������� ������ GameColor � Tag ������
                button.Text = color.Name;
                button.Click -= ColorGuessButton_Click;
                button.Click += ColorGuessButton_Click;
                button.Enabled = false; // ������ ����� �������� ��� ������ ������
                button.BackColor = GetSystemDrawingColor(color);
                button.ForeColor = (button.BackColor.GetBrightness() < 0.55) ? Color.Black : Color.White;
            }
            else
            {
                MessageBox.Show($"���� '{colorName}' ��� ������ '{button.Name}' �� ������ � ��������� ������.", "������ ������������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button.Enabled = false;
            }
        }

        private void GameManager_LevelStarted()
        {
            _nextLevelTimer.Stop();
            if (this.statusLabel != null) this.statusLabel.Text = "����� ���� ����� ������?";
            DrawGameBoard();
            UpdateColorChoiceButtons();
            if (this.newGameButton != null) this.newGameButton.Enabled = true;
        }

        private void GameManager_GuessProcessed(GuessResult result, GameColor guessedColor)
        {
            if (this.statusLabel == null) return;

            switch (result)
            {
                case GuessResult.Incorrect:
                    this.statusLabel.Text = $"�������. ���������� ��� ���.";
                    break;
                case GuessResult.CorrectAndContinue:
                    this.statusLabel.Text = $"���������! {guessedColor.Name} �����. ����� ���� ����� ������ ������?";
                    DrawGameBoard();
                    break;
                case GuessResult.CorrectAndLevelOver:
                    this.statusLabel.Text = $"�������! {guessedColor.Name} ��� ���������.";
                    DrawGameBoard();
                    break;
            }
            UpdateColorChoiceButtons();
        }

        private void GameManager_LevelCompleted()
        {
            if (this.statusLabel != null) this.statusLabel.Text += " ������� �������! ����� ���������...";
            foreach (var btn in _colorGuessButtons)
            {
                if (btn != null) btn.Enabled = false;
            }
            _nextLevelTimer.Start();
        }

        private void NextLevelTimer_Tick(object? sender, EventArgs e)
        {
            _nextLevelTimer.Stop();
            if (_gameManager != null) _gameManager.StartNewLevel();
        }

        private void NewGameButton_Click(object? sender, EventArgs e)
        {
            _nextLevelTimer.Stop();
            if (_gameManager != null) _gameManager.StartNewLevel();
        }

        private void ColorGuessButton_Click(object? sender, EventArgs e)
        {
            if (_gameManager == null || !_gameManager.IsLevelActive) return;

            if (sender is Button clickedButton && clickedButton.Tag is GameColor selectedColor)
            {
                _gameManager.MakeGuess(selectedColor);
            }
        }

        private void DrawGameBoard()
        {
            if (this.gamePanel == null || _gameManager == null) return;

            this.gamePanel.SuspendLayout();
            this.gamePanel.Controls.Clear();
            var items = _gameManager.CurrentMapItems;
            if (items == null || !items.Any())
            {
                this.gamePanel.ResumeLayout();
                return;
            }

            int currentX = ItemMargin;
            int currentY = ItemMargin;
            int itemsInCurrentRow = 0;

            foreach (var gameColorItem in items)
            {
                PictureBox itemPictureBox = new PictureBox
                {
                    Size = new Size(ItemSize, ItemSize),
                    Location = new Point(currentX, currentY),
                    // BackColor = Color.LightGray, // ��� ������� ���� ��������
                };
                itemPictureBox.Paint += (sender, e) =>
                {
                    DrawShapeOnGraphics(e.Graphics, gameColorItem, itemPictureBox.ClientRectangle);
                };
                this.gamePanel.Controls.Add(itemPictureBox);

                currentX += ItemSize + ItemMargin;
                itemsInCurrentRow++;
                if (itemsInCurrentRow >= MaxItemsPerRow && currentX > ItemMargin)
                {
                    currentX = ItemMargin;
                    currentY += ItemSize + ItemMargin;
                    itemsInCurrentRow = 0;
                }
            }
            this.gamePanel.ResumeLayout();
        }

        private void UpdateColorChoiceButtons()
        {
            bool enableButtons = _gameManager?.IsLevelActive ?? false;
            foreach (var btn in _colorGuessButtons)
            {
                if (btn != null) btn.Enabled = enableButtons;
            }
        }

        private void DrawShapeOnGraphics(Graphics g, GameColor gameColor, Rectangle bounds)
        {
            Color drawingColor = GetSystemDrawingColor(gameColor);
            ShapeType shapeType = gameColor.Shape;

            g.SmoothingMode = SmoothingMode.AntiAlias; // ��� �������� ����������
            // ��������� ������� ��������� ��� ��������� �������� ������ ��������
            Rectangle drawingRect = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width - 4, bounds.Height - 4);

            using (SolidBrush brush = new SolidBrush(drawingColor))
            {
                switch (shapeType)
                {
                    case ShapeType.Square:
                        g.FillRectangle(brush, drawingRect);
                        break;
                    case ShapeType.Circle:
                        g.FillEllipse(brush, drawingRect);
                        break;
                    case ShapeType.Triangle:
                        Point point1 = new Point(drawingRect.Left + drawingRect.Width / 2, drawingRect.Top);
                        Point point2 = new Point(drawingRect.Left, drawingRect.Bottom);
                        Point point3 = new Point(drawingRect.Right, drawingRect.Bottom);
                        Point[] trianglePoints = { point1, point2, point3 };
                        g.FillPolygon(brush, trianglePoints);
                        break;
                    case ShapeType.Star:
                        DrawStar(g, brush, drawingRect);
                        break;
                    default: // �� ������ ������, ���� ����� �������� ����� ��� ������ ��� ����������
                        g.FillRectangle(brush, drawingRect);
                        break;
                }
            }
        }

        private void DrawStar(Graphics g, Brush brush, Rectangle bounds)
        {
            int numPoints = 5;
            PointF[] points = new PointF[2 * numPoints];
            float outerRadius = Math.Min(bounds.Width, bounds.Height) / 2.0f;
            float innerRadius = outerRadius / 2.5f;
            PointF center = new PointF(bounds.Left + bounds.Width / 2.0f, bounds.Top + bounds.Height / 2.0f);
            float angleIncrement = (float)(Math.PI / numPoints);

            for (int i = 0; i < 2 * numPoints; i++)
            {
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;
                float angle = i * angleIncrement - (float)(Math.PI / 2); // �������� � ������� �����
                points[i] = new PointF(
                    center.X + radius * (float)Math.Cos(angle),
                    center.Y + radius * (float)Math.Sin(angle)
                );
            }
            g.FillPolygon(brush, points);
        }

        private Color GetSystemDrawingColor(GameColor gameColor)
        {
            switch (gameColor.Name.ToLowerInvariant())
            {
                case "�������": return System.Drawing.Color.Red;
                case "�������": return System.Drawing.Color.Green;
                case "�����": return System.Drawing.Color.Blue;
                case "������": return System.Drawing.Color.Yellow;
                default: return System.Drawing.Color.Gray;
            }
        }
    }
}