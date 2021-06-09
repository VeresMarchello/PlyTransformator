using System;
using System.Globalization;
using System.Windows.Data;

namespace PlyTransformator.Converters
{
    class SizeScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 1;
            }
            var result = double.Parse(value.ToString()) / 5;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
