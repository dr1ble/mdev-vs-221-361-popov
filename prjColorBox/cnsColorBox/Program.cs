
using System;
using System.Collections.Generic;
using System.Linq;
using ColorBox.Core; 

namespace cnsColorBox
{
    class Program
    {
        private static GameManager _gameManager;
        private static List<GameColor> _availableColors; // Держим список доступных цветов

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Для корректного отображения русских букв и символов
            Console.Title = "Игра «Цветовод» - Консольная версия";

            InitializeGame();
            _gameManager.StartNewLevel(); // Начинаем первый уровень

            // Игровой цикл не нужен в явном виде здесь, так как события GameManager будут управлять потоком
            // Оставляем программу работать, пока пользователь не решит выйти 
            // Для простоты, программа завершится после одного или нескольких уровней, если не сделать цикл ожидания.
            // Чтобы программа не закрывалась сразу после первого уровня:
            Console.WriteLine("\nНажмите любую клавишу для выхода после завершения всех уровней...");
            // Console.ReadKey(); // Убрано, т.к. LevelCompleted вызовет новый уровень
        }

        static void InitializeGame()
        {
            // Те же цвета и фигуры, что и в WinForms версии
            _availableColors = new List<GameColor>
            {
                new GameColor(1, "Красный", ShapeType.Square),
                new GameColor(2, "Зеленый", ShapeType.Circle),
                new GameColor(3, "Синий", ShapeType.Triangle),
                new GameColor(4, "Желтый", ShapeType.Star)
            };

            var gameSettings = new GameSettings(
                availableColors: _availableColors,
                numberOfDistinctColorsToUse: _availableColors.Count, // Используем все доступные
                totalItemsOnMap: 15 // Можно настроить
            );

            _gameManager = new GameManager(gameSettings);

            // Подписываемся на события
            _gameManager.LevelStarted += GameManager_LevelStarted;
            _gameManager.GuessProcessed += GameManager_GuessProcessed;
            _gameManager.LevelCompleted += GameManager_LevelCompleted;
        }

        private static void GameManager_LevelStarted()
        {
            Console.Clear();
            Console.WriteLine("--- Новый Уровень Начался! ---");
            DrawGameBoard();
            PromptForGuess();
        }

        private static void GameManager_GuessProcessed(GuessResult result, GameColor guessedColor)
        {
            switch (result)
            {
                case GuessResult.Incorrect:
                    SetConsoleColorForMessage(ConsoleColor.Red);
                    Console.WriteLine($"\nНеверно! Попробуйте еще раз.");
                    ResetConsoleColor();
                    break;
                case GuessResult.CorrectAndContinue:
                    SetConsoleColorForMessage(ConsoleColor.Green);
                    Console.WriteLine($"\nПравильно! Цвет '{guessedColor.Name}' убран.");
                    ResetConsoleColor();
                    DrawGameBoard(); // Перерисовываем поле
                    break;
                case GuessResult.CorrectAndLevelOver:
                    SetConsoleColorForMessage(ConsoleColor.Cyan);
                    Console.WriteLine($"\nОтлично! Цвет '{guessedColor.Name}' был последним.");
                    ResetConsoleColor();
                    DrawGameBoard(); // Показываем пустое поле (или почти пустое)
                    // LevelCompleted обработает запуск следующего уровня
                    break;
            }

            if (_gameManager.IsLevelActive) // Если уровень еще активен (не CorrectAndLevelOver)
            {
                PromptForGuess();
            }
        }

        private static void GameManager_LevelCompleted()
        {
            SetConsoleColorForMessage(ConsoleColor.Magenta);
            Console.WriteLine("\n--- Уровень Пройден! ---");
            ResetConsoleColor();
            Console.WriteLine("Подготовка следующего уровня...");
            System.Threading.Thread.Sleep(2000); // Пауза перед следующим уровнем
            _gameManager.StartNewLevel(); // Автоматически начинаем новый уровень
        }

        private static void DrawGameBoard()
        {
            Console.WriteLine("\nТекущее игровое поле:");
            var items = _gameManager.CurrentMapItems;
            if (!items.Any())
            {
                Console.WriteLine("(Пусто)");
                return;
            }

            // Простая отрисовка в одну строку для консоли
            // Можно сделать более сложную сетку, если нужно
            int itemsPerRow = 10; // Сколько элементов в строке консоли
            int count = 0;
            foreach (var item in items)
            {
                Console.ForegroundColor = GetConsoleColor(item);
                Console.Write(GetShapeSymbol(item.Shape) + " ");
                Console.ResetColor();
                count++;
                if (count % itemsPerRow == 0)
                {
                    Console.WriteLine();
                }
            }
            if (count % itemsPerRow != 0) // Если последняя строка не полная, переводим строку
            {
                Console.WriteLine();
            }
            Console.WriteLine(); // Дополнительный отступ
        }

        private static void PromptForGuess()
        {
            Console.WriteLine("\nКакой цвет самый частый? Введите номер цвета:");
            List<GameColor> guessableColors = _availableColors
                                             .Where(ac => _gameManager.CurrentMapItems.Any(ci => ci.Id == ac.Id))
                                             .ToList(); // Показываем только те цвета, которые еще есть на поле

            if (!guessableColors.Any() && _gameManager.IsLevelActive) // На случай, если что-то пошло не так и нет цветов для угадывания
            {
                var nextExpected = _gameManager.GetNextExpectedColor();
                if (nextExpected != null) guessableColors.Add(nextExpected); // Добавляем хотя бы ожидаемый
            }


            for (int i = 0; i < guessableColors.Count; i++)
            {
                Console.ForegroundColor = GetConsoleColor(guessableColors[i]);
                Console.Write($"{i + 1}. {guessableColors[i].Name} {GetShapeSymbol(guessableColors[i].Shape)}");
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.Write("Ваш выбор (номер): ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= guessableColors.Count)
            {
                GameColor selectedColor = guessableColors[choice - 1];
                _gameManager.MakeGuess(selectedColor);
            }
            else
            {
                SetConsoleColorForMessage(ConsoleColor.Yellow);
                Console.WriteLine("Неверный ввод. Пожалуйста, введите номер из списка.");
                ResetConsoleColor();
                PromptForGuess(); // Повторный запрос
            }
        }

        private static char GetShapeSymbol(ShapeType shape)
        {
            switch (shape)
            {
                case ShapeType.Square: return '■'; // U+25A0 BLACK SQUARE
                case ShapeType.Circle: return '●'; // U+25CF BLACK CIRCLE
                case ShapeType.Triangle: return '▲'; // U+25B2 BLACK UP-POINTING TRIANGLE
                case ShapeType.Star: return '*'; 
                default: return '?';
            }
        }

        private static ConsoleColor GetConsoleColor(GameColor gameColor)
        {
            switch (gameColor.Name.ToLowerInvariant())
            {
                case "красный": return ConsoleColor.Red;
                case "зеленый": return ConsoleColor.Green;
                case "синий": return ConsoleColor.Blue;
                case "желтый": return ConsoleColor.Yellow;
                default: return ConsoleColor.Gray;
            }
        }
        private static void SetConsoleColorForMessage(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }
        private static void ResetConsoleColor()
        {
            Console.ResetColor();
        }
    }
}