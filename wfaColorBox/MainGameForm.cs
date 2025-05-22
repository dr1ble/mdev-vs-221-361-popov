using ColorBox.Core; // Используем типы из Core
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D; // Для рисования звезды
using System.Linq;
using System.Windows.Forms;

namespace wfaColorBox
{
    public partial class MainGameForm : Form
    {
        private GameManager _gameManager;
        private List<GameColor> _availableColors; // Список цветов, доступных для игры
        private List<Button> _colorGuessButtons;  // Кнопки для угадывания цветов

        private System.Windows.Forms.Timer _nextLevelTimer; // Таймер для задержки перед следующим уровнем

        // Константы для отображения элементов на игровом поле
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
                Interval = 1500 // Задержка в миллисекундах
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
                MessageBox.Show("Критическая ошибка: GameManager не был инициализирован к моменту OnLoad.", "Ошибка запуска", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void InitializeGame()
        {
            // Определяем доступные цвета и их фигуры
            _availableColors = new List<GameColor>
            {
                new GameColor(1, "Красный", ShapeType.Square),
                new GameColor(2, "Зеленый", ShapeType.Circle),
                new GameColor(3, "Синий", ShapeType.Triangle),
                new GameColor(4, "Желтый", ShapeType.Star)
            };

            // Настройки игры
            var gameSettings = new GameSettings(
                availableColors: _availableColors,
                numberOfDistinctColorsToUse: _availableColors.Count, // Используем все 4 доступных цвета
                totalItemsOnMap: 20 // Например, 20 элементов. (1+2+3+4=10; 20-10=10. 1+2,2+2,3+3,4+3 -> 3,4,6,7)
                                    // Убедитесь, что TotalItemsOnMap >= minItemsRequiredForDistinctCounts
            );

            _gameManager = new GameManager(gameSettings);

            // Подписка на события GameManager
            _gameManager.LevelStarted += GameManager_LevelStarted;
            _gameManager.GuessProcessed += GameManager_GuessProcessed;
            _gameManager.LevelCompleted += GameManager_LevelCompleted;
        }

        private void ConnectDesignerUIToLogic()
        {
            this.Text = "Игра «Цветовод»";

            // Предполагается, что кнопки redButton, greenButton, blueButton, yellowButton
            // и newGameButton, а также statusLabel и gamePanel созданы в дизайнере форм.
            _colorGuessButtons = new List<Button?> { this.redButton, this.greenButton, this.blueButton, this.yellowButton }
                                    .OfType<Button>()
                                    .ToList();

            // Назначаем цвета и обработчики кнопкам угадывания
            // Порядок важен и должен соответствовать _availableColors, если хотим прямого сопоставления
            // или использовать более надежный способ поиска по имени цвета.
            AssignTagAndClickHandlerForButton(this.redButton, "Красный");
            AssignTagAndClickHandlerForButton(this.greenButton, "Зеленый");
            AssignTagAndClickHandlerForButton(this.blueButton, "Синий");
            AssignTagAndClickHandlerForButton(this.yellowButton, "Желтый");


            if (this.newGameButton != null)
            {
                this.newGameButton.Click += NewGameButton_Click;
            }
            else
            {
                MessageBox.Show("Элемент управления 'newGameButton' не найден.", "Ошибка UI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (this.gamePanel == null)
            {
                MessageBox.Show("Элемент управления 'gamePanel' не найден.", "Ошибка UI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (this.statusLabel == null)
            {
                MessageBox.Show("Элемент управления 'statusLabel' не найден.", "Ошибка UI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AssignTagAndClickHandlerForButton(Button? button, string colorName)
        {
            if (button == null) return;

            // Находим GameColor по имени
            GameColor? color = _availableColors.FirstOrDefault(c => c.Name == colorName);
            if (color != null)
            {
                button.Tag = color; // Сохраняем объект GameColor в Tag кнопки
                button.Text = color.Name;
                button.Click -= ColorGuessButton_Click;
                button.Click += ColorGuessButton_Click;
                button.Enabled = false; // Кнопки будут включены при старте уровня
                button.BackColor = GetSystemDrawingColor(color);
                button.ForeColor = (button.BackColor.GetBrightness() < 0.55) ? Color.Black : Color.White;
            }
            else
            {
                MessageBox.Show($"Цвет '{colorName}' для кнопки '{button.Name}' не найден в доступных цветах.", "Ошибка конфигурации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button.Enabled = false;
            }
        }

        private void GameManager_LevelStarted()
        {
            _nextLevelTimer.Stop();
            if (this.statusLabel != null) this.statusLabel.Text = "Какой цвет самый частый?";
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
                    this.statusLabel.Text = $"Неверно. Попробуйте еще раз.";
                    break;
                case GuessResult.CorrectAndContinue:
                    this.statusLabel.Text = $"Правильно! {guessedColor.Name} убран. Какой цвет самый частый теперь?";
                    DrawGameBoard();
                    break;
                case GuessResult.CorrectAndLevelOver:
                    this.statusLabel.Text = $"Отлично! {guessedColor.Name} был последним.";
                    DrawGameBoard();
                    break;
            }
            UpdateColorChoiceButtons();
        }

        private void GameManager_LevelCompleted()
        {
            if (this.statusLabel != null) this.statusLabel.Text += " Уровень пройден! Скоро следующий...";
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
                    // BackColor = Color.LightGray, // Для отладки фона элемента
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

            g.SmoothingMode = SmoothingMode.AntiAlias; // Для гладкого рендеринга
            // Уменьшаем область рисования для небольших отступов внутри элемента
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
                    default: // На всякий случай, если будет добавлен новый тип фигуры без реализации
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
                float angle = i * angleIncrement - (float)(Math.PI / 2); // Начинаем с верхней точки
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
                case "красный": return System.Drawing.Color.Red;
                case "зеленый": return System.Drawing.Color.Green;
                case "синий": return System.Drawing.Color.Blue;
                case "желтый": return System.Drawing.Color.Yellow;
                default: return System.Drawing.Color.Gray;
            }
        }
    }
}