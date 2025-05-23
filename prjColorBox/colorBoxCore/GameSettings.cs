namespace ColorBox.Core
{
    public class GameSettings
    {
        public List<GameColor> AvailableColors { get; }
        public int NumberOfDistinctColorsToUse { get; }
        public int TotalItemsOnMap { get; }

        public GameSettings(List<GameColor> availableColors, int numberOfDistinctColorsToUse, int totalItemsOnMap)
        {
            if (availableColors == null || !availableColors.Any())
                throw new ArgumentException("Список доступных цветов не может быть пустым.", nameof(availableColors));
            if (numberOfDistinctColorsToUse <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfDistinctColorsToUse), "Количество используемых цветов должно быть положительным.");
            if (numberOfDistinctColorsToUse > availableColors.Count)
                throw new ArgumentException("Количество используемых цветов не может превышать количество доступных цветов.", nameof(numberOfDistinctColorsToUse));

            // Минимальное количество элементов, если все количества должны быть уникальными и начинаться с 1
            int minItemsRequiredForDistinctCounts = numberOfDistinctColorsToUse * (numberOfDistinctColorsToUse + 1) / 2;
            if (totalItemsOnMap < minItemsRequiredForDistinctCounts)
                throw new ArgumentException(
                    $"Общее количество элементов на карте ({totalItemsOnMap}) слишком мало для {numberOfDistinctColorsToUse} " +
                    $"различных количеств цветов (минимально возможно {minItemsRequiredForDistinctCounts}, если начинать с 1,2,3...).", nameof(totalItemsOnMap));
            // Также нужно, чтобы общее количество было не меньше, чем количество используемых цветов (каждый цвет хотя бы один раз)
            if (totalItemsOnMap < numberOfDistinctColorsToUse)
                throw new ArgumentException(
                   $"Общее количество элементов на карте ({totalItemsOnMap}) не может быть меньше количества используемых цветов ({numberOfDistinctColorsToUse}).", nameof(totalItemsOnMap));


            AvailableColors = new List<GameColor>(availableColors);
            NumberOfDistinctColorsToUse = numberOfDistinctColorsToUse;
            TotalItemsOnMap = totalItemsOnMap;
        }
    }
}