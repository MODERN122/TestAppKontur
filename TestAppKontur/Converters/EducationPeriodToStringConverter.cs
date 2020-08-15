using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TestAppKontur.Models;
using Xamarin.Forms;

namespace TestAppKontur.Converters
{
    class EducationPeriodToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Educationperiod educationPeriod)
            {
                DateTimeFormatInfo fmt = (new CultureInfo("ru-RU")).DateTimeFormat;
                return $"{educationPeriod.Start.ToString("d", fmt)} - {educationPeriod.End.ToString("d", fmt)}";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strSplit = ((string)value).Split(' ');
            return new Educationperiod()
            {
                End = DateTime.Parse(strSplit[2]),
                Start = DateTime.Parse(strSplit[0])
            };
        }
    }
}
