using System;
using System.Collections.Generic;
using System.Linq;

namespace cnsGameFindLock
{
    // Перечисление возможных действий в главном меню
    public enum MenuAction
    {
        Play = 1,
        ViewStats = 2,
        GameRules = 3,
        Exit = 4
    }

    public class ConsoleRenderer
    {

        public void ShowWelcomeScreen()
        {
            Console.Clear();
            Console.ResetColor();
            Console.WriteLine("\nДобро пожаловать в игру 'Найди Замок'!");
            Console.WriteLine("Испытайте свои навыки в распознавании паттернов.");
        }

        public MenuAction ShowMainMenu()
        {
            Console.WriteLine("\n--- ГЛАВНОЕ МЕНЮ ---");
            Console.WriteLine("1. Играть");
            Console.WriteLine("2. Просмотреть статистику");
            Console.WriteLine("3. Правила игры");
            Console.WriteLine("4. Выход");

            return (MenuAction)GetValidNumberInput("\nВыберите действие (1-4): ", 1, 4);
        }

        public GameConfig ConfigureGame()
        {
            Console.WriteLine("\n--- Настройка Игры ---");
            GameConfig config = new GameConfig();

            Console.WriteLine("Выберите уровень сложности:");
            Console.WriteLine("1. Легкий (3 зубца, 30% шаг, без зеркалирования, 3 варианта)");
            Console.WriteLine("2. Средний (4 зубца, 20% шаг, без зеркалирования, 4 варианта)");
            Console.WriteLine("3. Сложный (5 зубцов, 15% шаг, с зеркалированием, 4 варианта)");
            Console.WriteLine("4. Эксперт (6 зубцов, 10% шаг, с зеркалированием, 5 вариантов)");

            int difficultyChoice = GetValidNumberInput("\nВаш выбор (1-4): ", 1, 4);

            DifficultyLevel selectedDifficulty;
            switch (difficultyChoice)
            {
                case 1: selectedDifficulty = DifficultyLevel.Easy; break;
                case 2: selectedDifficulty = DifficultyLevel.Normal; break;
                case 3: selectedDifficulty = DifficultyLevel.Hard; break;
                case 4: selectedDifficulty = DifficultyLevel.Expert; break;
                default: selectedDifficulty = DifficultyLevel.Normal; break;
            }

            config.ApplyDifficultyPreset(selectedDifficulty);

            return config;
        }

        public bool AskForDisplayMode()
        {
            Console.Write("Использовать продвинутый режим отображения? (да/нет): ");
            return Console.ReadLine()?.Trim().ToLower() == "да";
        }

        public int GetUserChoice(int numberOfOptions)
        {
            if (numberOfOptions <=0) { // Защита от случая, когда нет опций
                Console.WriteLine("Ошибка: нет доступных вариантов выбора.");
                return 1; // Возвращаем фиктивный выбор, но это состояние ошибки
            }
            if (numberOfOptions == 1) return 1; // Если только одна опция, выбор очевиден

            return GetValidNumberInput($"Какой замок подходит? (1-{numberOfOptions}): ", 1, numberOfOptions);
        }

        public void DisplayKeyAndLocks(KeyLockPattern key, List<KeyLockPattern> lockOptions, bool useAdvancedDisplay, GameConfig config)
        {
            Console.WriteLine("\n--- КЛЮЧ ---");
            if (useAdvancedDisplay) key.DisplayAdvanced("Ключ", config);
            else Console.WriteLine($"Ключ: {key}");

            Console.WriteLine("\n--- ВАРИАНТЫ ЗАМКОВ ---");
            for (int i = 0; i < lockOptions.Count; i++)
            {
                string label = $"Замок {i + 1}";
                if (useAdvancedDisplay) lockOptions[i].DisplayAdvanced(label, config);
                else Console.WriteLine($"{label}: {lockOptions[i]}");
                Console.WriteLine(); 
            }
        }
        
        
        public void DisplayStats(KeyLockPattern.GameStats stats)
        {
            stats.Display(); // Делегируем отображение самому классу статистики
        }

        public void ShowGameRules()
        {
            Console.Clear();
            Console.WriteLine("--- ПРАВИЛА ИГРЫ 'НАЙДИ ЗАМОК' ---\n");
            Console.WriteLine("Цель игры: Найти замок, который соответствует представленному ключу.");
            Console.WriteLine("Ключ и замок подходят друг к другу, если сумма высот их соответствующих зубцов равна 100%.");
            Console.WriteLine("Пример: Ключ [60, 20] подходит к Замку [40, 80].\n");
            Console.WriteLine("Как играть:");
            Console.WriteLine("1. Вам будет показан ключ (набор зубцов).");
            Console.WriteLine("2. Затем вам будет представлено несколько вариантов замков.");
            Console.WriteLine("3. Вы должны выбрать замок, в который подойдет ваш ключ.\n");
            Console.WriteLine("Отзеркаливание (на сложных уровнях):");
            Console.WriteLine("- Если отзеркаливание включено, правильный замок может подходить");
            Console.WriteLine("  к ОБРАТНОМУ ПОРЯДКУ зубцов ключа.");
            Console.WriteLine("  Пример: Ключ [60, 20], Отзеркаленный ключ [20, 60].");
            Console.WriteLine("  Замок [80, 40] подойдет к отзеркаленному ключу [20, 60].\n");
            Console.WriteLine("Уровни сложности (предустановленные):");
            Console.WriteLine("- Легкий: 3 зубца, большой шаг изменения, без зеркалирования.");
            Console.WriteLine("- Средний: 4 зубца, средний шаг изменения, без зеркалирования.");
            Console.WriteLine("- Сложный: 5 зубцов, малый шаг изменения, с зеркалированием.");
            Console.WriteLine("- Эксперт: 6 зубцов, минимальный шаг изменения, с зеркалированием.");
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        public bool AskPlayAgain()
        {
            Console.Write("\nСыграть еще раз? (да/нет): ");
            return Console.ReadLine()?.Trim().ToLower() == "да";
        }

        public int GetValidNumberInput(string prompt, int min, int max, int step = 1)
        {
            int result;
            bool validInput = false;
            do
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                validInput = int.TryParse(input, out result) && result >= min && result <= max && (result - min) % step == 0;
                if (!validInput)
                {
                    Console.Write($"Неверный ввод. Пожалуйста, введите число от {min} до {max}");
                    if (step > 1) Console.Write($" кратное {step}");
                    Console.WriteLine(".");
                }
            } while (!validInput);
            return result;
        }
    }
}