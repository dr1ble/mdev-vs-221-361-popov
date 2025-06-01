using System;
using System.Collections.Generic;
using System.Linq;

namespace cnsGameFindLock
{
    // Перечисление, определяющее уровни сложности игры
    // Каждый уровень соответствует набору предустановленных настроек
    public enum DifficultyLevel
    {
        Easy, // Минимальная сложность для начинающих игроков
        Normal, // Стандартный уровень сложности
        Hard, // Повышенная сложность для опытных игроков
        Expert // Максимальная сложность с большим количеством зубцов
    }

    // Класс настроек игры
    public class GameConfig
    {
        // Базовые настройки игрового процесса
        public int NumberOfTeeth { get; set; } = 4;
        public int MinToothValue { get; set; } = 10; // Минимальное значение зубца в %
        public int MaxToothValue { get; set; } = 100; // Максимальное значение зубца в %
        public int VariationStepPercentage { get; set; } = 20; // Шаг изменения для генерации неверных вариантов
        public bool AllowMirroring { get; set; } = false; // Разрешено ли отзеркаливание ключа
        public int NumberOfLockOptions { get; set; } = 4; // Количество вариантов замков для выбора
        public int DisplayWidth { get; set; } = 20; // Ширина отступа для отображения зубцов замка
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Normal;

        // Метод применяет предустановленные настройки сложности в зависимости от выбранного уровня
        // Каждый уровень имеет свои параметры, которые влияют на игровой процесс
        public void ApplyDifficultyPreset(DifficultyLevel level)
        {
            Difficulty = level; // Сохраняем выбранный уровень сложности

            switch (level)
            {
                case DifficultyLevel.Easy:
                    // Легкий уровень: меньше зубцов, больше шаг вариации, меньше вариантов замков
                    NumberOfTeeth = 3; // Всего 3 зубца - минимальная сложность
                    MinToothValue = 10;
                    MaxToothValue = 100;
                    VariationStepPercentage = 30; // Большой шаг вариации - легче различать неправильные замки
                    AllowMirroring = false; // Отзеркаливание отключено
                    NumberOfLockOptions = 3; // Всего 3 варианта замков на выбор
                    break;

                case DifficultyLevel.Normal:
                    // Средний уровень: стандартные параметры сложности
                    NumberOfTeeth = 4; // 4 зубца - стандартное количество
                    MinToothValue = 10;
                    MaxToothValue = 100;
                    VariationStepPercentage = 20; // Средний шаг вариации
                    AllowMirroring = false; // Отзеркаливание отключено
                    NumberOfLockOptions = 4; // 4 варианта замков на выбор
                    break;

                case DifficultyLevel.Hard:
                    // Сложный уровень: больше зубцов, меньше шаг вариации, включено отзеркаливание
                    NumberOfTeeth = 5; // 5 зубцов - повышенная сложность
                    MinToothValue = 10;
                    MaxToothValue = 100;
                    VariationStepPercentage = 15; // Меньший шаг вариации - сложнее различать неправильные замки
                    AllowMirroring = true; // Включено отзеркаливание - требуется учитывать направление ключа
                    NumberOfLockOptions = 4; // 4 варианта замков на выбор
                    break;

                case DifficultyLevel.Expert:
                    // Экспертный уровень: максимальное количество зубцов, минимальный шаг вариации, больше вариантов замков
                    NumberOfTeeth = 6; // 6 зубцов - максимальная сложность
                    MinToothValue = 10;
                    MaxToothValue = 100;
                    VariationStepPercentage =
                        10; // Минимальный шаг вариации - очень сложно различать неправильные замки
                    AllowMirroring = true; // Включено отзеркаливание - требуется учитывать направление ключа
                    NumberOfLockOptions = 5; // 5 вариантов замков на выбор - наибольшее количество
                    break;


            }
        }
    }

    // Класс, представляющий паттерн ключа или замка
    // Содержит форму и размеры зубцов, а также методы для работы с ними
    // Может представлять как ключ, так и замок, в зависимости от контекста использования
    public class KeyLockPattern
    {
        // Список значений зубцов в процентах (от 0 до 100)
        // Каждое значение представляет высоту зубца, где 100% - максимальная высота
        public List<int> Teeth { get; private set; }

        // Константы для визуализации зубцов в консоли
        private const int MaxAsciiBarLength = 10; // Максимальная длина символов "█" для 100% в ASCII арте
        private static readonly int s_MaxAsciiBarLength = 10; // Статическая версия константы для статических методов

        // Конструктор, создающий паттерн на основе заданного списка значений зубцов
        // Позволяет создать точную копию существующего паттерна или задать конкретную форму
        public KeyLockPattern(List<int> teeth)
        {
            // Создаем новый список, чтобы избежать изменений исходного списка извне
            Teeth = new List<int>(teeth);
        }

        // Конструктор для случайной генерации паттерна ключа/замка на основе игровых настроек
        // Используется при создании новых игровых раундов
        public KeyLockPattern(GameConfig config, Random random)
        {
            Teeth = new List<int>();
            for (int i = 0; i < config.NumberOfTeeth; i++)
            {
                // Генерируем значения кратные 10 для простоты и наглядности (10, 20, 30, ..., 100)
                // Расчет диапазона: делим на 10, генерируем число, умножаем обратно на 10
                Teeth.Add(random.Next(config.MinToothValue / 10, (config.MaxToothValue / 10) + 1) * 10);
            }
        }

        // Создает и возвращает отзеркаленную копию текущего паттерна
        // Используется при генерации альтернативных решений с отзеркаливанием ключа
        public KeyLockPattern GetMirrored()
        {
            // Создаем копию списка зубцов
            var mirroredTeeth = new List<int>(Teeth);
            // Переворачиваем порядок зубцов (первый становится последним и т.д.)
            mirroredTeeth.Reverse();
            // Возвращаем новый паттерн с отзеркаленными зубцами
            return new KeyLockPattern(mirroredTeeth);
        }

        // Проверяет, подходит ли текущий паттерн (ключ) к другому паттерну (замок)
        // Ключ подходит к замку, если сумма соответствующих зубцов равна 100%
        public bool IsMatch(KeyLockPattern otherLock)
        {
            // Проверяем базовые условия: замок существует и имеет то же количество зубцов
            if (otherLock == null || Teeth.Count != otherLock.Teeth.Count)
                return false;

            // Проверяем каждую пару соответствующих зубцов
            for (int i = 0; i < Teeth.Count; i++)
            {
                // Ключ подходит к замку, если сумма каждой пары зубцов ровно 100%
                // Например: зубец ключа 30% + зубец замка 70% = 100%
                if (Teeth[i] + otherLock.Teeth[i] != 100)
                    return false;
            }

            // Все зубцы подходят, ключ соответствует замку
            return true;
        }

        // Создает комплементарный (дополнительный) замок для текущего ключа
        // Для каждого зубца ключа создается комплементарный зубец замка так, чтобы их сумма была 100%
        // Например, если зубец ключа 30%, то зубец замка будет 70%
        public KeyLockPattern CreateComplementaryLock()
        {
            // Создаем новый список для зубцов комплементарного замка
            var complementaryTeeth = new List<int>();
            for (int i = 0; i < Teeth.Count; i++)
            {
                // Каждый зубец замка дополняет соответствующий зубец ключа до 100%
                complementaryTeeth.Add(100 - Teeth[i]);
            }

            // Возвращаем новый паттерн замка, который идеально подходит к текущему ключу
            return new KeyLockPattern(complementaryTeeth);
        }

        // Переопределение метода ToString для удобного текстового представления паттерна
        // Возвращает строку вида [30, 50, 70, 20] для вывода зубцов в процентах
        public override string ToString()
        {
            // Преобразуем список значений зубцов в строку с разделителями
            return $"[{string.Join(", ", Teeth.Select(t => t.ToString()))}]";
        }

        // Метод для продвинутого графического отображения паттерна в консоли
        // Визуализирует зубцы с помощью символов ASCII-графики
        // Параметр label определяет, это ключ или замок
        public void DisplayAdvanced(string label, GameConfig config) // Config содержит DisplayWidth
        {
            // Выводим название паттерна (ключ или замок)
            Console.WriteLine($"{label}:");
            // Проверяем наличие зубцов
            if (Teeth == null || !Teeth.Any())
            {
                Console.WriteLine("(пусто)");
                return;
            }

            // Определяем тип паттерна по метке (ключ или замок) для правильного отображения
            bool isLock = label.ToLower().Contains("замок") || label.ToLower().Contains("эталон");
            // Получаем ширину отображения (стандартно 20 символов)
            int displayWidth = config?.DisplayWidth ?? 20;


            // Визуализируем каждый зубец
            foreach (int toothValue in Teeth)
            {
                // Вычисляем длину графической полосы пропорционально значению зубца
                // Например, для 100% длина будет MaxAsciiBarLength, для 50% - половина этого значения
                int barLength = (int)Math.Round(toothValue / 100.0 * MaxAsciiBarLength);
                barLength = Math.Max(0, barLength); // Гарантируем, что длина не будет отрицательной
                // Для значений > 0% но очень маленьких, обеспечиваем минимальную видимость (1 символ)
                if (toothValue > 0 && barLength == 0) barLength = 1; // Минимум 1 для >0%

                // Отображаем зубцы по-разному для замка и ключа
                if (isLock)
                {
                    // Для замка: пробелы слева, затем блоки справа (выравнивание по правому краю)
                    // Пример: "                  ████" (меньше блоков = более глубокий вырез)
                    Console.WriteLine(new string(' ', displayWidth - barLength) + new string('█', barLength));
                }
                else
                {
                    // Для ключа: блоки слева (выравнивание по левому краю)
                    // Пример: "████" (больше блоков = более высокий выступ)
                    Console.WriteLine(new string('█', barLength));
                }
            }
        }

        // Класс для хранения игровой статистики
        // Отслеживает количество игр, побед и поражений
        public class GameStats
        {
            // Статистические данные игры
            public int GamesPlayed { get; private set; } // Общее количество сыгранных игр
            public int Wins { get; private set; } // Количество побед (правильных выборов замка)
            public int Losses => GamesPlayed - Wins; // Вычисляемое свойство: количество поражений
            public double WinPercentage => GamesPlayed > 0 ? (double)Wins / GamesPlayed * 100 : 0; // Процент побед

            // Методы для регистрации результатов игры
            public void RecordWin()
            {
                GamesPlayed++;
                Wins++;
            } // Записать победу

            public void RecordLoss()
            {
                GamesPlayed++;
            } // Записать поражение (только увеличиваем счетчик игр)

            // Метод для отображения статистики в консоли
            // Выводит текущую статистику игры в консоль в удобном формате
            public void Display()
            {
                // Выводим заголовок блока статистики
                Console.WriteLine("\n--- Статистика Игры ---");
                // Выводим основные статистические показатели
                Console.WriteLine($"Игр сыграно: {GamesPlayed}"); // Общее количество игр
                Console.WriteLine($"Побед: {Wins}"); // Количество правильных выборов
                Console.WriteLine($"Поражений: {Losses}"); // Количество неправильных выборов
                Console.WriteLine($"Процент побед: {WinPercentage:F2}%"); // Процент побед с точностью до 2 знаков
                Console.WriteLine("----------------------"); // Нижняя граница блока статистики
            }
        }

        // Класс для хранения результатов игрового раунда
        // Содержит всю информацию о прошедшем раунде: выбор игрока, правильный ответ и т.д.
        public class GameRoundResult
        {
            // Результаты и данные раунда
            public bool IsCorrect { get; set; } // Флаг правильности выбора игрока
            public KeyLockPattern ActualKeyDisplayed { get; set; } // Ключ, который был показан игроку
            public List<KeyLockPattern> LockOptions { get; set; } // Список всех вариантов замков
            public int CorrectLockIndex { get; set; } // Индекс правильного замка в списке

            public KeyLockPattern CorrectLockPattern =>
                LockOptions[CorrectLockIndex]; // Правильный замок (вычисляемое свойство)

            public int UserChoiceIndex { get; set; } // Индекс замка, выбранного пользователем

            public KeyLockPattern UserSelectedLockPattern =>
                LockOptions[UserChoiceIndex]; // Замок, выбранный пользователем (вычисляемое свойство)

            public bool SolutionWasForMirroredKey { get; set; } // Флаг, требовалось ли отзеркаливание ключа для решения
        }

        // Основной класс игровой логики
        // Содержит методы для генерации, обработки и визуализации игровых раундов
        // Управляет игровым процессом и взаимодействием с пользователем
        public class GameFindLock
        {
            // Служебные объекты
            private Random _random = new Random(); // Генератор случайных чисел для создания ключей и замков
            private GameStats _gameStats = new GameStats(); // Объект для хранения игровой статистики

            private ConsoleRenderer
                _renderer = new ConsoleRenderer(); // Объект для отображения игровых элементов в консоли

            // Возвращает текущую статистику игры
            public GameStats GetStats() => _gameStats;

            // Генерирует данные для нового игрового раунда на основе текущей конфигурации
            // Возвращает кортеж, содержащий ключ, варианты замков, индекс правильного замка
            // и флаг необходимости отзеркаливания
            public (KeyLockPattern KeyDisplayed, List<KeyLockPattern> LockOptions, int CorrectLockIndex, bool
                SolutionRequiresMirroring) GenerateGameRound(GameConfig config)
            {
                // Генерируем случайный ключ, который будет показан игроку
                KeyLockPattern keyDisplayedToPlayer = new KeyLockPattern(config, _random);
                bool solutionRequiresMirroring = false; // По умолчанию отзеркаливание не требуется

                // Если отзеркаливание разрешено настройками и ключ имеет более одного зубца
                if (config.AllowMirroring && keyDisplayedToPlayer.Teeth.Count > 1)
                {
                    // Проверяем, является ли ключ палиндромом (симметричным)
                    // Ключ является палиндромом, если он совпадает со своей зеркальной копией
                    bool isPalindrome =
                        keyDisplayedToPlayer.Teeth.SequenceEqual(keyDisplayedToPlayer.GetMirrored().Teeth);

                    // Для несимметричных ключей с вероятностью 50% решаем использовать отзеркаливание
                    if (!isPalindrome && _random.Next(0, 2) == 0) // 50% шанс, если не палиндром
                    {
                        solutionRequiresMirroring = true; // Для этого раунда потребуется отзеркаливание
                    }
                }

                // Определяем фактический ключ для поиска решения
                // Если требуется отзеркаливание, используем зеркальную копию ключа
                KeyLockPattern effectiveKeyForSolution =
                    solutionRequiresMirroring ? keyDisplayedToPlayer.GetMirrored() : keyDisplayedToPlayer;

                // Генерируем варианты замков, включая правильный замок и дистракторы (неправильные варианты)
                List<KeyLockPattern> lockOptions =
                    GenerateLockOptions(keyDisplayedToPlayer, effectiveKeyForSolution, config, _random);

                // Находим индекс правильного замка среди сгенерированных вариантов
                int correctLockIndex = -1;
                for (int i = 0; i < lockOptions.Count; i++)
                {
                    // Проверяем, подходит ли ключ к текущему замку
                    if (effectiveKeyForSolution.IsMatch(lockOptions[i]))
                    {
                        correctLockIndex = i;
                        break; // Найден правильный замок, прекращаем поиск
                    }
                }

                // Проверка на ошибки генерации: правильный замок должен быть в списке вариантов
                if (correctLockIndex == -1 && lockOptions.Any())
                {
                    Console.WriteLine(
                        "КРИТИЧЕСКАЯ ОШИБКА В GENERATEGAMEROUND: Правильный замок не был найден среди опций после генерации!");
                }

                // Возвращаем все сгенерированные данные для игрового раунда
                return (keyDisplayedToPlayer, lockOptions, correctLockIndex, solutionRequiresMirroring);
            }

            // Обрабатывает выбор пользователя, проверяет правильность ответа и отображает результат
            // Возвращает объект GameRoundResult, содержащий все данные о результате раунда
            public GameRoundResult ProcessUserChoice(int userChoiceOneBased, KeyLockPattern keyDisplayed,
                List<KeyLockPattern> lockOptions, int correctLockIndex, bool solutionWasForMirroredKey,
                GameConfig config)
            {
                // Преобразуем выбор пользователя из 1-based в 0-based индекс для внутреннего использования
                int userChoiceIndex = userChoiceOneBased - 1;
                // Получаем правильный замок
                KeyLockPattern actualCorrectLock = lockOptions[correctLockIndex];
                // Определяем, правильный ли выбор сделал пользователь
                bool isCorrect = (userChoiceIndex == correctLockIndex);

                // Пустая строка для визуального разделения
                Console.WriteLine();
                // Обработка случая правильного ответа
                if (isCorrect)
                {
                    // Регистрируем победу в статистике
                    _gameStats.RecordWin();
                    // Выводим сообщение о правильном ответе зеленым цветом
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✓ Правильно! Вы нашли подходящий замок.");
                    Console.ResetColor();

                    // Определяем форму ключа для отображения результата
                    // Если решение требовало отзеркаливания, показываем отзеркаленный ключ для наглядности
                    KeyLockPattern keyFormForDisplay =
                        solutionWasForMirroredKey ? keyDisplayed.GetMirrored() : keyDisplayed;

                    // Информируем пользователя, если ключ был отзеркален для решения
                    if (solutionWasForMirroredKey) Console.WriteLine("(Ключ был отзеркален для этого решения)");
                }
                // Обработка случая неправильного ответа
                else
                {
                    // Регистрируем поражение в статистике
                    _gameStats.RecordLoss();
                    // Выводим сообщение о неправильном ответе красным цветом
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ Неправильно! Правильный замок был под номером {correctLockIndex + 1}.");
                    Console.ResetColor();

                }

                // Создаем и возвращаем объект с результатами раунда
                // Этот объект содержит все данные о прошедшем раунде для дальнейшего анализа
                return new GameRoundResult
                {
                    IsCorrect = isCorrect, // Флаг правильности ответа
                    ActualKeyDisplayed = keyDisplayed, // Ключ, показанный игроку
                    LockOptions = lockOptions, // Все варианты замков
                    CorrectLockIndex = correctLockIndex, // Индекс правильного замка
                    UserChoiceIndex = userChoiceIndex, // Индекс выбранного пользователем замка
                    SolutionWasForMirroredKey = solutionWasForMirroredKey // Требовалось ли отзеркаливание
                };
            }

            // Запускает полный игровой раунд: настройка, генерация данных, отображение и обработка выбора игрока
            // Этот метод управляет всем процессом одного раунда от начала до конца
            public void RunGameRound()
            {
                // Получаем конфигурацию игры от пользователя через консольный рендерер
                GameConfig config = _renderer.ConfigureGame();

                // Генерируем данные для раунда: ключ, варианты замков и т.д.
                var (keyDisplayed, lockOptions, correctLockIndex, solutionRequiresMirroring) =
                    GenerateGameRound(config);

                // Проверка на критические ошибки в генерации данных
                // Проверяем, был ли найден правильный замок
                if (correctLockIndex == -1)
                {
                    Console.WriteLine(
                        "Критическая ошибка: правильный замок не был сгенерирован или найден. Пропуск раунда.");
                    return; // Прерываем раунд при критической ошибке
                }

                // Проверяем, были ли сгенерированы варианты замков
                if (lockOptions.Count == 0)
                {
                    Console.WriteLine("Критическая ошибка: не сгенерировано ни одного варианта замка. Пропуск раунда.");
                    return; // Прерываем раунд при критической ошибке
                }

                // Предупреждение, если сгенерировано меньше вариантов, чем требовалось
                if (lockOptions.Count < config.NumberOfLockOptions && lockOptions.Count > 0)
                {
                    Console.WriteLine(
                        $"Предупреждение: Сгенерировано только {lockOptions.Count} из {config.NumberOfLockOptions} вариантов замков.");
                }


                // Запрашиваем у пользователя предпочтительный режим отображения (базовый или продвинутый)
                bool useAdvancedDisplay = _renderer.AskForDisplayMode();

                // Отображаем ключ и варианты замков в выбранном режиме
                _renderer.DisplayKeyAndLocks(keyDisplayed, lockOptions, useAdvancedDisplay, config);

                // Определяем фактическое количество вариантов для выбора
                int actualNumberOfOptions = lockOptions.Count;
                // Запрашиваем выбор пользователя
                int userChoice = _renderer.GetUserChoice(actualNumberOfOptions);

                // Обрабатываем выбор пользователя и отображаем результат
                ProcessUserChoice(userChoice, keyDisplayed, lockOptions, correctLockIndex, solutionRequiresMirroring,
                    config);
            }

            // Отображает текущую игровую статистику через консольный рендерер
            public void DisplayStats()
            {
                // Делегируем отображение статистики консольному рендереру
                _renderer.DisplayStats(GetStats());
            }

            // Приватный метод для генерации вариантов замков, включая правильный и дистракторы
            // Метод гарантирует, что среди вариантов будет хотя бы один правильный замок
            // и несколько неправильных с различной степенью отличия от правильного
            private List<KeyLockPattern> GenerateLockOptions(
                KeyLockPattern keyDisplayedToPlayer, // Ключ, показываемый игроку
                KeyLockPattern effectiveKeyForSolution, // Ключ, используемый для определения правильного замка (может быть отзеркаленным)
                GameConfig config, // Настройки игры
                Random random) // Генератор случайных чисел
            {
                // Список для хранения всех вариантов замков
                var options = new List<KeyLockPattern>();

                // Создаем правильный замок, который подходит к эффективному ключу решения
                KeyLockPattern correctLock = effectiveKeyForSolution.CreateComplementaryLock();
                // Добавляем правильный замок в список вариантов
                options.Add(correctLock);

                // Переменная для хранения альтернативной формы правильного замка
                // (используется при отзеркаливании, чтобы избежать дублирования)
                KeyLockPattern alternateFormOfCorrectLock = null;

                // Если отзеркаливание разрешено и ключ имеет более одного зубца
                if (config.AllowMirroring && keyDisplayedToPlayer.Teeth.Count > 1)
                {
                    // Определяем другой потенциальный эффективный ключ
                    // Если текущий эффективный ключ - это исходный ключ, то альтернативой будет отзеркаленный ключ
                    // Иначе альтернативой будет исходный ключ
                    KeyLockPattern otherPotentialEffectiveKey =
                        effectiveKeyForSolution.Teeth.SequenceEqual(keyDisplayedToPlayer.Teeth)
                            ? keyDisplayedToPlayer.GetMirrored()
                            : keyDisplayedToPlayer;

                    // Если другой потенциальный ключ отличается от текущего эффективного ключа
                    if (!otherPotentialEffectiveKey.Teeth.SequenceEqual(effectiveKeyForSolution.Teeth))
                    {
                        // Создаем альтернативную форму правильного замка
                        // Это замок, который подходит к альтернативному ключу
                        alternateFormOfCorrectLock = otherPotentialEffectiveKey.CreateComplementaryLock();
                    }
                }

                // Максимальное количество попыток создания каждого дистрактора
                // Предотвращает бесконечные циклы при невозможности создать уникальный дистрактор
                int maxAttemptsPerDistractor = 25;

                // Продолжаем добавлять дистракторы, пока не достигнем нужного количества вариантов
                while (options.Count < config.NumberOfLockOptions)
                {
                    int attempts = 0; // Счетчик попыток для текущего дистрактора
                    KeyLockPattern distractor = null; // Объект дистрактора
                    bool distractorIsValid = false; // Флаг валидности дистрактора

                    // Пытаемся создать валидный дистрактор
                    while (attempts < maxAttemptsPerDistractor && !distractorIsValid)
                    {
                        // Создаем копию зубцов правильного замка для последующей модификации
                        var distractorTeeth = new List<int>(correctLock.Teeth);

                        // Определяем количество зубцов, которые будем модифицировать
                        // Модифицируем от 1 до половины зубцов (но не менее 1)
                        int teethToModifyCount = random.Next(1, Math.Max(2, distractorTeeth.Count / 2 + 1));
                        // Если всего один зубец, то модифицируем только его
                        if (distractorTeeth.Count == 1) teethToModifyCount = 1;

                        // Множество для отслеживания индексов уже модифицированных зубцов
                        // Используем HashSet для быстрой проверки наличия элемента
                        HashSet<int> modifiedIndices = new HashSet<int>();

                        // Цикл модификации выбранного количества зубцов
                        for (int k = 0; k < teethToModifyCount; k++)
                        {
                            int toothIndexToChange; // Индекс зубца для изменения
                            int findIndexAttempts = 0; // Счетчик попыток найти неизмененный зубец

                            // Пытаемся найти зубец, который еще не был модифицирован
                            do
                            {
                                // Выбираем случайный индекс зубца
                                toothIndexToChange = random.Next(0, distractorTeeth.Count);
                                findIndexAttempts++;
                            } while (modifiedIndices.Contains(toothIndexToChange) && // Пока индекс уже использован
                                     findIndexAttempts <
                                     distractorTeeth.Count * 2 && // И не превышено максимальное число попыток
                                     modifiedIndices.Count <
                                     distractorTeeth.Count); // И еще остались неизмененные зубцы

                            // Если выбрали уже измененный зубец, но остались неизмененные - пропускаем текущую итерацию
                            if (modifiedIndices.Contains(toothIndexToChange) &&
                                modifiedIndices.Count < distractorTeeth.Count) continue;
                            // Если все зубцы уже выбраны, но нужно еще изменения - прерываем цикл
                            if (modifiedIndices.Count == distractorTeeth.Count && k < teethToModifyCount)
                                break; // All teeth already picked

                            // Добавляем индекс зубца в список модифицированных
                            modifiedIndices.Add(toothIndexToChange);

                            // Получаем исходное значение зубца
                            int originalValue = distractorTeeth[toothIndexToChange];

                            // Определяем направление изменения: увеличение или уменьшение
                            // -1 = уменьшение, 1 = увеличение значения
                            int changeDirection = random.Next(0, 2) == 0 ? -1 : 1;

                            // Определяем количество шагов вариации (1 или 2)
                            int stepsOfVariation = random.Next(1, 3);

                            // Вычисляем величину изменения на основе шага вариации из конфигурации
                            // Пример: если VariationStepPercentage = 20, changeDirection = 1, stepsOfVariation = 2,
                            // то changeAmount = 20 * 1 * 2 = 40%
                            int changeAmount = config.VariationStepPercentage * changeDirection * stepsOfVariation;

                            // Вычисляем предложенное новое значение
                            int newValueProposed = originalValue + changeAmount;

                            // Округляем до ближайшего значения, кратного 10, для единообразия
                            newValueProposed = (int)(Math.Round(newValueProposed / 10.0) * 10);

                            // Ограничиваем новое значение минимальным и максимальным значениями из конфигурации
                            int newValue = Math.Max(config.MinToothValue,
                                Math.Min(config.MaxToothValue, newValueProposed));

                            // Обработка случая, когда новое значение совпадает с исходным
                            // Это может произойти из-за ограничений или округления
                            if (newValue == originalValue && (config.MinToothValue != config.MaxToothValue))
                            {
                                // Пробуем изменить в противоположном направлении
                                newValueProposed = originalValue - changeAmount; // Try other direction
                                newValueProposed = (int)(Math.Round(newValueProposed / 10.0) * 10);
                                newValue = Math.Max(config.MinToothValue,
                                    Math.Min(config.MaxToothValue, newValueProposed));
                            }

                            // Если значение все равно осталось прежним, и это последний или единственный изменяемый зубец,
                            // принудительно выбираем случайное отличное значение из допустимого диапазона
                            if (newValue == originalValue &&
                                (teethToModifyCount == 1 || modifiedIndices.Count == distractorTeeth.Count) &&
                                (config.MinToothValue != config.MaxToothValue))
                            {

                                // Создаем список всех возможных значений кратных 10 в диапазоне
                                List<int> possibleValues = Enumerable.Range(config.MinToothValue / 10,
                                        (config.MaxToothValue - config.MinToothValue) / 10 + 1)
                                    .Select(v => v * 10) // Преобразуем в значения, кратные 10
                                    .Where(v => v != originalValue) // Исключаем исходное значение
                                    .ToList();

                                // Если есть доступные альтернативные значения, выбираем случайное из них
                                if (possibleValues.Any())
                                {
                                    newValue = possibleValues[random.Next(possibleValues.Count)];
                                }
                            }

                            // Устанавливаем новое значение зубца в списке
                            distractorTeeth[toothIndexToChange] = newValue;
                        }

                        // Создаем новый паттерн замка на основе модифицированных зубцов
                        distractor = new KeyLockPattern(distractorTeeth);

                        // Проверяем, что созданный дистрактор не дублирует уже имеющиеся варианты
                        bool isDuplicateInOptions = options.Any(opt => opt.Teeth.SequenceEqual(distractor.Teeth));

                        // Проверяем, что дистрактор не совпадает с альтернативной формой правильного замка
                        bool isTheAlternateCorrectLock = (alternateFormOfCorrectLock != null &&
                                                          distractor.Teeth.SequenceEqual(alternateFormOfCorrectLock
                                                              .Teeth));

                        // Дистрактор валиден, если он уникален и не является альтернативной формой правильного замка
                        if (!isDuplicateInOptions && !isTheAlternateCorrectLock)
                        {
                            distractorIsValid = true;
                        }

                        attempts++; // Увеличиваем счетчик попыток
                    }

                    // Если создан валидный дистрактор, добавляем его в список вариантов
                    if (distractorIsValid)
                    {
                        options.Add(distractor);
                    }
                    else
                    {
                        // Если не удалось создать валидный дистрактор после всех попыток,
                        // прекращаем генерацию вариантов
                        break;
                    }
                }

                // Перемешиваем варианты замков и возвращаем их
                // OrderBy с random.Next() используется для случайного перемешивания
                return options.OrderBy(x => random.Next()).ToList();
            }
        }
    }
}