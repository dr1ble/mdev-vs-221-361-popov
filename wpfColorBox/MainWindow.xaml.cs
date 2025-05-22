using ColorBox.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;

namespace wpfColorBox
{
    public partial class MainWindow : Window
    {
        private GameManager? _gameManager;
        private List<GameColor> _availableCoreColors = new List<GameColor>();
        private DispatcherTimer? _nextLevelTimer;

        public MainWindow()
        {
            InitializeComponent();
            Debug.WriteLine("MainWindow Constructor: InitializeComponent called.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Window_Loaded event triggered.");
            try
            {
                InitializeGame();
                CreateGuessButtons();

                if (_gameManager != null)
                {
                    Debug.WriteLine("Window_Loaded: GameManager is not null, starting new level.");
                    _gameManager.StartNewLevel();
                }
                else
                {
                    StatusTextBlock.Text = "Ошибка: GameManager не был инициализирован!";
                    Debug.WriteLine("Window_Loaded: GameManager IS NULL after InitializeGame!");
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Критическая ошибка при загрузке: {ex.Message}";
                Debug.WriteLine($"КРИТИЧЕСКАЯ ОШИБКА в Window_Loaded: {ex.ToString()}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}\n\n{ex.StackTrace}", "Ошибка Загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeGame()
        {
            Debug.WriteLine("InitializeGame started.");
            _availableCoreColors = new List<GameColor>
            {
                new GameColor(1, "Красный", ShapeType.Square),
                new GameColor(2, "Зеленый", ShapeType.Circle),
                new GameColor(3, "Синий", ShapeType.Triangle),
                new GameColor(4, "Желтый", ShapeType.Star)
            };
            Debug.WriteLine($"_availableCoreColors populated with {_availableCoreColors.Count} colors.");

            var gameSettings = new GameSettings(
                availableColors: _availableCoreColors,
                numberOfDistinctColorsToUse: _availableCoreColors.Count,
                totalItemsOnMap: 15
            );
            Debug.WriteLine("GameSettings created.");

            _gameManager = new GameManager(gameSettings);
            Debug.WriteLine("GameManager created.");

            _gameManager.LevelStarted += GameManager_LevelStarted;
            _gameManager.GuessProcessed += GameManager_GuessProcessed;
            _gameManager.LevelCompleted += GameManager_LevelCompleted;
            Debug.WriteLine("GameManager events subscribed.");

            _nextLevelTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1.5)
            };
            _nextLevelTimer.Tick += NextLevelTimer_Tick;
            Debug.WriteLine("NextLevelTimer created.");
            Debug.WriteLine("InitializeGame finished.");
        }

        private void CreateGuessButtons()
        {
            Debug.WriteLine("CreateGuessButtons started.");
            GuessButtonsPanel.Children.Clear();
            if (!_availableCoreColors.Any())
            {
                Debug.WriteLine("CreateGuessButtons: _availableCoreColors is empty or null.");
                return;
            }

            foreach (var coreColor in _availableCoreColors)
            {
                Button guessButton = new Button
                {
                    Content = coreColor.Name,
                    Tag = coreColor,
                    MinWidth = 80,
                    Height = 30,
                    Margin = new Thickness(5),
                    Background = GetBrushFromGameColor(coreColor),
                    Foreground = Brushes.White,
                    IsEnabled = false
                };
                guessButton.Click += GuessButton_Click;
                GuessButtonsPanel.Children.Add(guessButton);
            }
            Debug.WriteLine($"CreateGuessButtons finished, added {GuessButtonsPanel.Children.Count} buttons.");
        }

        private Brush GetBrushFromGameColor(GameColor gameColor)
        {
            if (gameColor == null) return Brushes.Transparent;
            System.Windows.Media.Color mediaColor;
            switch (gameColor.Name.ToLowerInvariant())
            {
                case "красный": mediaColor = Colors.Red; break;
                case "зеленый": mediaColor = Colors.Green; break;
                case "синий": mediaColor = Colors.Blue; break;
                case "желтый": mediaColor = Colors.Yellow; break;
                default: mediaColor = Colors.Gray; break;
            }
            return new SolidColorBrush(mediaColor);
        }

        private void GameManager_LevelStarted()
        {
            Debug.WriteLine("GameManager_LevelStarted triggered.");
            Dispatcher.Invoke(() =>
            {
                _nextLevelTimer?.Stop();
                StatusTextBlock.Text = "Какой цвет самый частый?";
                UpdateGameBoardUI();
                SetGuessButtonsEnabled(true);
                NewLevelButton.IsEnabled = true;
            });
        }

        private void GameManager_GuessProcessed(GuessResult result, GameColor guessedColor)
        {
            Debug.WriteLine($"GameManager_GuessProcessed triggered. Result: {result}, GuessedColor: {guessedColor?.Name ?? "null"}.");
            Dispatcher.Invoke(() =>
            {
                if (guessedColor == null) { StatusTextBlock.Text = "Ошибка: null guessedColor."; return; }
                switch (result)
                {
                    case GuessResult.Incorrect:
                        StatusTextBlock.Text = $"Неверно. Попробуйте еще раз.";
                        break;
                    case GuessResult.CorrectAndContinue:
                        StatusTextBlock.Text = $"Правильно! {guessedColor.Name} убран. Какой цвет самый частый теперь?";
                        UpdateGameBoardUI();
                        break;
                    case GuessResult.CorrectAndLevelOver:
                        StatusTextBlock.Text = $"Отлично! {guessedColor.Name} был последним.";
                        UpdateGameBoardUI();
                        break;
                }
            });
        }

        private void GameManager_LevelCompleted()
        {
            Debug.WriteLine("GameManager_LevelCompleted triggered.");
            Dispatcher.Invoke(() =>
            {
                StatusTextBlock.Text += " Уровень пройден! Скоро следующий...";
                SetGuessButtonsEnabled(false);
                NewLevelButton.IsEnabled = false;
                _nextLevelTimer?.Start();
            });
        }

        private void NextLevelTimer_Tick(object? sender, EventArgs e)
        {
            Debug.WriteLine("NextLevelTimer_Tick triggered.");
            _nextLevelTimer?.Stop();
            if (_gameManager != null)
            {
                _gameManager.StartNewLevel();
            }
            else { Debug.WriteLine("NextLevelTimer_Tick: _gameManager is null!"); }
        }

        private void UpdateGameBoardUI()
        {
            Debug.WriteLine("UpdateGameBoardUI started.");
            if (_gameManager == null)
            {
                Debug.WriteLine("UpdateGameBoardUI: _gameManager is null!");
                GameItemsControl.ItemsSource = null;
                return;
            }

            if (_gameManager.CurrentMapItems.Any())
            {
                Debug.WriteLine($"UpdateGameBoardUI: Populating with {_gameManager.CurrentMapItems.Count} items.");
                GameItemsControl.ItemsSource = new List<GameColor>(_gameManager.CurrentMapItems);
            }
            else
            {
                Debug.WriteLine("UpdateGameBoardUI: No items to display.");
                GameItemsControl.ItemsSource = null;
            }
            Debug.WriteLine("UpdateGameBoardUI finished.");
        }


        private void SetGuessButtonsEnabled(bool isEnabled)
        {
            Debug.WriteLine($"SetGuessButtonsEnabled called with: {isEnabled}");
            foreach (Button btn in GuessButtonsPanel.Children.OfType<Button>())
            {
                btn.IsEnabled = isEnabled && (_gameManager?.IsLevelActive ?? false);
            }
        }

        private void GuessButton_Click(object sender, RoutedEventArgs e)
        {
            if (_gameManager == null) { Debug.WriteLine("GuessButton_Click: _gameManager is null!"); return; }

            if (sender is Button clickedButton && clickedButton.Tag is GameColor selectedColor)
            {
                Debug.WriteLine($"GuessButton_Click: Guessed {selectedColor.Name}.");
                if (_gameManager.IsLevelActive)
                {
                    _gameManager.MakeGuess(selectedColor);
                }
            }
        }

        private void NewLevelButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("NewLevelButton_Click triggered.");
            _nextLevelTimer?.Stop();
            if (_gameManager != null)
            {
                _gameManager.StartNewLevel();
            }
            else { Debug.WriteLine("NewLevelButton_Click: _gameManager is null!"); }
        }
    }
}