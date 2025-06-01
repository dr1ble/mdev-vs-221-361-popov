using System;
using System.Globalization;
using System.Windows.Data;

namespace TimeTrainer.Converters 
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string enumValue = value.ToString();
            string targetValue = parameter.ToString();
            return enumValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null; // Or DependencyProperty.UnsetValue

            bool boolValue = (bool)value;
            if (!boolValue)
                return null; // Or DependencyProperty.UnsetValue (don't change if false, as it's for a group)


            // This parameter should be the Enum type if you want to convert back robustly.
            // For this RadioButton scenario, parameter is the Enum value as a string.
            // We can parse it.
            try
            {
                return Enum.Parse(targetType, parameter.ToString(), true);
            }
            catch 
            {
                return null; // Or DependencyProperty.UnsetValue
            }
        }
    }
}