using System;
using System.Globalization;
using System.Windows.Data;

namespace Dice.Ui
{
    class MultiplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return (value, parameter) switch
            {
                (double v, double multiplier) => v * multiplier,
                (double v, int multiplier) => v * multiplier,
                (double v, float multiplier) => v * multiplier,
                (double v, long multiplier) => v * multiplier,
                (double v, string multiplierStr) when double.TryParse(multiplierStr, out var multiplier) => v * multiplier,
                _ => throw new NotImplementedException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
