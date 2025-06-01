using System;
using System.Globalization;
using System.Windows.Data;

namespace TimeTrainer.Converters
{
    /// <summary>
    /// Конвертер, который проверяет, начинается ли строка с заданного значения
    /// </summary>
    public class StringStartsWithConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string stringValue = value.ToString();
            string prefix = parameter.ToString();

            return stringValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
