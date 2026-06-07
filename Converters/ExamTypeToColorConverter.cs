using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ExamSheduleDesign.Converters
{
    public class ExamTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string type = value as string;
            if (type == "Экзамен")
                return (SolidColorBrush)Application.Current.Resources["AccentBrush"]; // синий
            if (type == "Консультация")
                return (SolidColorBrush)Application.Current.Resources["SuccessBrush"] ?? new SolidColorBrush(Colors.Green);
            return (SolidColorBrush)Application.Current.Resources["AccentBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}