using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Menafrinet.Core
{
    public class CutoffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int.Parse(parameter as string) > Cutoff);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public int Cutoff { get; set; }
    }
}
