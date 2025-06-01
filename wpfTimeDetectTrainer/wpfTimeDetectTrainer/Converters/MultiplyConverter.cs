using System;
using System.Globalization;
using System.Windows.Data;

namespace TimeTrainer.Converters
{
    /// <summary>
    /// Конвертер для умножения значения на заданный коэффициент
    /// </summary>
    public class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверка входных данных
            if (!(value is double val))
                return value;

            if (!(parameter is string paramStr))
                return value;

            // Обработка параметра с запятой (множитель,смещение)
            if (paramStr.Contains(","))
            {
                string[] parts = paramStr.Split(',');
                if (parts.Length == 2 &&
                    double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double factor) &&
                    double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double offset))
                {
                    return (val * factor) + offset;
                }
            }

            // Обычное умножение
            if (double.TryParse(paramStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double simpleFactor))
            {
                return val * simpleFactor;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}