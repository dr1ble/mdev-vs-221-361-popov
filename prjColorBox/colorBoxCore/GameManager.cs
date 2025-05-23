namespace ColorBox.Core
{
    public class GameManager
    {
        private readonly GameSettings _settings;
        private readonly Random _random = new Random();

        public IReadOnlyList<GameColor> CurrentMapItems => _currentMapItems.AsReadOnly();
        private List<GameColor> _currentMapItems = new List<GameColor>();

        private List<GameColor> _correctOrderOfColorsToGuess = new List<GameColor>();
        private int _currentGuessIndex = 0;

        public bool IsLevelActive { get; private set; } = false;

        public event Action? LevelStarted;
        public event Action<GuessResult, GameColor>? GuessProcessed;
        public event Action? LevelCompleted;

        public GameManager(GameSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void StartNewLevel()
        {
            var levelColors = _settings.AvailableColors
                                     .OrderBy(x => _random.Next())
                                     .Take(_settings.NumberOfDistinctColorsToUse)
                                     .ToList();

            var counts = GenerateDistinctCounts(_settings.TotalItemsOnMap, levelColors.Count);
            counts = counts.OrderBy(x => _random.Next()).ToList(); // Перемешиваем соответствие количеств цветам


            var colorWithCounts = new Dictionary<GameColor, int>();
            for (int i = 0; i < levelColors.Count; i++)
            {
                colorWithCounts.Add(levelColors[i], counts[i]);
            }

            _currentMapItems.Clear();
            foreach (var pair in colorWithCounts)
            {
                for (int i = 0; i < pair.Value; i++)
                {
                    _currentMapItems.Add(pair.Key);
                }
            }
            _currentMapItems = _currentMapItems.OrderBy(x => _random.Next()).ToList();

            _correctOrderOfColorsToGuess = colorWithCounts
                                          .OrderByDescending(pair => pair.Value)
                                          .Select(pair => pair.Key)
                                          .ToList();

            _currentGuessIndex = 0;
            IsLevelActive = true;
            LevelStarted?.Invoke();
        }

        private List<int> GenerateDistinctCounts(int totalSum, int k)
        {
            if (k <= 0) throw new ArgumentOutOfRangeException(nameof(k), "Количество различных элементов должно быть положительным.");
            int minPossibleSum = k * (k + 1) / 2;
            if (totalSum < minPossibleSum)
                throw new ArgumentException($"Сумма {totalSum} слишком мала для {k} различных слагаемых. Минимум: {minPossibleSum}");

            var counts = new List<int>(k);
            long currentSum = 0;

            for (int i = 0; i < k; i++)
            {
                counts.Add(i + 1);
                currentSum += (i + 1);
            }

            long remainder = totalSum - currentSum;
            if (remainder < 0) throw new InvalidOperationException("Отрицательный остаток при первичной генерации.");

            if (remainder > 0)
            {
                for (int i = 0; i < remainder; i++)
                {
                    int j = (k - 1) - (i % k);
                    counts[j]++;
                }
            }

            bool smoothed;
            int maxSmoothingAttempts = k * k + (int)remainder;
            int attempts = 0;

            do
            {
                smoothed = false;
                counts.Sort();

                if (k > 1)
                {
                    if (counts[k - 1] - counts[0] > 2)
                    {
                        bool canSmoothThisStep = true;
                        if (k > 2)
                        {
                            if ((counts[k - 1] - 1) <= counts[k - 2])
                            {
                                canSmoothThisStep = false;
                            }
                            if ((counts[0] + 1) >= counts[1])
                            {
                                canSmoothThisStep = false;
                            }
                        }
                        if (canSmoothThisStep)
                        {
                            counts[k - 1]--;
                            counts[0]++;
                            smoothed = true;
                        }
                    }
                }
                attempts++;
            } while (smoothed && attempts < maxSmoothingAttempts);

            if (counts.Distinct().Count() != k)
                throw new InvalidOperationException($"Сгенерированные количества не являются уникальными (после сглаживания). Уникальных: {counts.Distinct().Count()}, ожидалось: {k}. Counts: [{string.Join(",", counts)}]");
            if (counts.Sum() != totalSum)
                throw new InvalidOperationException($"Сумма ({counts.Sum()}) не равна целевой ({totalSum}) (после сглаживания). Counts: [{string.Join(",", counts)}]");
            if (counts.Any(c => c <= 0))
                throw new InvalidOperationException($"Одно из количеств не положительное (после сглаживания). Counts: [{string.Join(",", counts)}]");

            return counts;
        }


        public GuessResult MakeGuess(GameColor guessedColor)
        {
            if (!IsLevelActive)
            {
                GuessProcessed?.Invoke(GuessResult.Incorrect, guessedColor);
                return GuessResult.Incorrect;
            }
            if (guessedColor == null) throw new ArgumentNullException(nameof(guessedColor));

            GuessResult result;
            GameColor expectedColor = _correctOrderOfColorsToGuess[_currentGuessIndex];

            if (expectedColor.Equals(guessedColor))
            {
                _currentMapItems.RemoveAll(item => item.Equals(guessedColor));
                _currentGuessIndex++;

                if (_currentGuessIndex >= _correctOrderOfColorsToGuess.Count)
                {
                    result = GuessResult.CorrectAndLevelOver;
                    IsLevelActive = false;
                    LevelCompleted?.Invoke();
                }
                else
                {
                    result = GuessResult.CorrectAndContinue;
                }
            }
            else
            {
                result = GuessResult.Incorrect;
            }

            GuessProcessed?.Invoke(result, guessedColor);
            return result;
        }

        public List<GameColor> GetRemainingColorsOrderedByFrequency()
        {
            if (!_currentMapItems.Any()) return new List<GameColor>();

            return _currentMapItems
                .GroupBy(color => color)
                .Select(group => new { Color = group.Key, Count = group.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => x.Color)
                .ToList();
        }

        public GameColor? GetNextExpectedColor()
        {
            if (!IsLevelActive || _currentGuessIndex >= _correctOrderOfColorsToGuess.Count)
            {
                return null;
            }
            return _correctOrderOfColorsToGuess[_currentGuessIndex];
        }
    }
}