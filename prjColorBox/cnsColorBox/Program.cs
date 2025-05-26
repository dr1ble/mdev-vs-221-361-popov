// Program.cs в проекте cnsColorBoxGame
using System;
using System.Collections.Generic;
using System.Linq;
using prjColorBox; // Ссылка на вашу библиотеку логики

namespace cnsColorBoxGame
{
    class Program
    {
        // Настройки игры для консоли
        private const int GRID_ROWS = 7;
        private const int GRID_COLS = 12;
        private const int NUM_COLORS = 3; // Количество цветов на уровне

        // Можно использовать тот же словарь для сопоставления ID цвета с его консольным представлением
        // Для простоты, будем использовать цифры. Можно добавить и текстовые имена.
        private static Dictionary<int, (ConsoleColor ConsoleFg, string Symbol, string RussianName)> _consoleColorMapping =
            new Dictionary<int, (ConsoleColor, string, string)>
        {
            { 1, (ConsoleColor.Red,     "1", "Красный") },
            { 2, (ConsoleColor.Green,   "2", "Зеленый") },
            { 3, (ConsoleColor.Blue,    "3", "Синий") },
            { 4, (ConsoleColor.Yellow,  "4", "Желтый") }, // Убедитесь, что фон консоли не желтый
            { 5, (ConsoleColor.Magenta, "5", "Пурпурный") }, // Orange нет в ConsoleColor, Magenta как замена
            { 6, (ConsoleColor.Cyan,    "6", "Голубой") }
        };

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Для корректного отображения русских букв
            Console.Title = "Цветовод - Консольная версия";
            ColorMapGenerator mapGenerator = new ColorMapGenerator();
            bool playing = true;
            int level = 1;

            Console.WriteLine("Добро пожаловать в игру 'Цветовод' (консольная версия)!");

            while (playing)
            {
                Console.Clear();
                Console.WriteLine($"--- Уровень {level} ---");
                Console.WriteLine($"Настройки: {GRID_ROWS}x{GRID_COLS} сетка, {NUM_COLORS} цветов.\n");

                MapData currentMapData;
                try
                {
                    currentMapData = mapGenerator.GenerateMap(GRID_ROWS, GRID_COLS, NUM_COLORS);
                }
                catch (ArgumentException ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Ошибка генерации карты: {ex.Message}");
                    Console.WriteLine("Пожалуйста, проверьте константы GRID_ROWS, GRID_COLS, NUM_COLORS.");
                    Console.ResetColor();
                    Console.WriteLine("Нажмите любую клавишу для выхода...");
                    Console.ReadKey();
                    return;
                }

                List<int> expectedColorOrder = new List<int>(currentMapData.SortedColorIDsByFrequency);
                // Для отображения будем хранить копию карты, из которой будем "удалять" цвета
                int[][] displayGrid = new int[GRID_ROWS][];
                for (int i = 0; i < GRID_ROWS; i++)
                {
                    displayGrid[i] = (int[])currentMapData.Grid[i].Clone();
                }


                while (expectedColorOrder.Any())
                {
                    DisplayConsoleMap(displayGrid);
                    DisplayColorChoices(currentMapData.ColorCounts.Keys.Where(id => displayGrid.SelectMany(row => row).Contains(id)).ToList()); // Показываем только оставшиеся цвета

                    Console.Write("\nКакого цвета больше всего из оставшихся? Введите номер (ID) цвета: ");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int chosenColorId))
                    {
                        if (currentMapData.ColorCounts.ContainsKey(chosenColorId) &&
                            displayGrid.SelectMany(row => row).Contains(chosenColorId)) // Проверяем, что такой цвет еще есть на поле
                        {
                            if (chosenColorId == expectedColorOrder.First())
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Верно! Цвет '{_consoleColorMapping[chosenColorId].RussianName}' ({chosenColorId}) был самым частым.");
                                Console.ResetColor();

                                expectedColorOrder.RemoveAt(0);
                                // "Удаляем" цвет с отображаемой карты
                                RemoveColorFromDisplayGrid(displayGrid, chosenColorId);

                                if (!expectedColorOrder.Any())
                                {
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("\nУровень пройден! Отлично!");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.WriteLine("Продолжаем...");
                                }
                                System.Threading.Thread.Sleep(1500); // Небольшая пауза
                                Console.Clear(); // Очищаем для следующего шага или уровня
                                if (expectedColorOrder.Any()) Console.WriteLine($"--- Уровень {level} (продолжение) ---");

                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Неверно. Попробуйте еще раз.");
                                Console.ResetColor();
                                System.Threading.Thread.Sleep(1000);
                                Console.Clear();
                                Console.WriteLine($"--- Уровень {level} (попытка снова) ---");
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Такого ID цвета нет среди доступных или он уже угадан. Попробуйте снова.");
                            Console.ResetColor();
                            System.Threading.Thread.Sleep(1500);
                            Console.Clear();
                            Console.WriteLine($"--- Уровень {level} (попытка снова) ---");
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Некорректный ввод. Пожалуйста, введите число (ID цвета).");
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(1500);
                        Console.Clear();
                        Console.WriteLine($"--- Уровень {level} (попытка снова) ---");
                    }
                }

                level++;
                Console.WriteLine("\nНачать следующий уровень? (y/n): ");
                string playAgainInput = Console.ReadLine()?.ToLower();
                if (playAgainInput != "y" && playAgainInput != "д")
                {
                    playing = false;
                }
            }

            Console.WriteLine("\nСпасибо за игру! Нажмите любую клавишу для выхода.");
            Console.ReadKey();
        }

        static void DisplayConsoleMap(int[][] grid)
        {
            Console.WriteLine("\nТекущее поле:");
            for (int r = 0; r < grid.Length; r++)
            {
                for (int c = 0; c < grid[r].Length; c++)
                {
                    int colorId = grid[r][c];
                    if (colorId != 0 && _consoleColorMapping.ContainsKey(colorId))
                    {
                        Console.ForegroundColor = _consoleColorMapping[colorId].ConsoleFg;
                        Console.Write(_consoleColorMapping[colorId].Symbol + " ");
                    }
                    else
                    {
                        Console.Write("  "); // Пустое место или фон
                    }
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        static void DisplayColorChoices(List<int> availableColorIds)
        {
            Console.WriteLine("\nДоступные цвета (ID - Название):");
            foreach (int id in availableColorIds.OrderBy(id => id))
            {
                if (_consoleColorMapping.ContainsKey(id))
                {
                    var colorInfo = _consoleColorMapping[id];
                    Console.ForegroundColor = colorInfo.ConsoleFg;
                    Console.Write($" {id} ");
                    Console.ResetColor();
                    Console.Write($"- {colorInfo.RussianName}  ");
                }
            }
            Console.WriteLine();
        }

        static void RemoveColorFromDisplayGrid(int[][] grid, int colorIdToRemove)
        {
            for (int r = 0; r < grid.Length; r++)
            {
                for (int c = 0; c < grid[r].Length; c++)
                {
                    if (grid[r][c] == colorIdToRemove)
                    {
                        grid[r][c] = 0; // Заменяем на "пустое" значение
                    }
                }
            }
        }
    }
}