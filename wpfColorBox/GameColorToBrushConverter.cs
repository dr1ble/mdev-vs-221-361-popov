using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ColorBox.Core;
using System.Diagnostics;

namespace wpfColorBox
{
    public class GameColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is GameColor gameColor)
                {
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
                return Brushes.Transparent;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в GameColorToBrushConverter: {ex.Message}");
                return Brushes.Magenta;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Метод ConvertBack не реализован для GameColorToBrushConverter, так как он используется для односторонней привязки.");
        }
    }
}