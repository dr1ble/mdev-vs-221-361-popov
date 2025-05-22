using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ColorBox.Core;
using System.Diagnostics;

namespace wpfColorBox
{
    public class ShapeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is ShapeType currentShape && parameter is ShapeType targetShape)
                {
                    return currentShape == targetShape ? Visibility.Visible : Visibility.Collapsed;
                }
                return Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в ShapeToVisibilityConverter: {ex.Message}");
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Метод ConvertBack не реализован для ShapeToVisibilityConverter, так как он используется для односторонней привязки.");
        }
    }
}