using System;
using System.Globalization;
using System.Windows.Data;

namespace ExamSheduleDesign.Converters
{
    public class TeacherNameNormalizingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string name && !string.IsNullOrWhiteSpace(name))
            {
                // Приводим к нижнему регистру, затем делаем первую букву каждого слова заглавной
                var textInfo = CultureInfo.CurrentCulture.TextInfo;
                var lower = name.ToLower();
                return textInfo.ToTitleCase(lower);
            }
            return value ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}