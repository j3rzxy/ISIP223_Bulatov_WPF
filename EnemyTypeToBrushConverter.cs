using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ISIP223_Bulatov_WPF
{
    /// <summary>
    /// Конвертер: тип врага → цвет рамки (для минималистичной графики)
    /// </summary>
    public class EnemyTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typeName = value as string;

            switch (typeName)
            {
                case "Undead":
                    return new SolidColorBrush(Color.FromRgb(156, 39, 176)); // Purple
                case "Beast":
                    return new SolidColorBrush(Color.FromRgb(251, 140, 0));  // Orange
                case "Boss":
                    return new SolidColorBrush(Color.FromRgb(211, 47, 47));  // Red
                case "Chest":
                    return new SolidColorBrush(Color.FromRgb(255, 193, 7));  // Gold
                case "Slug":
                    return new SolidColorBrush(Color.FromRgb(129, 199, 132)); // Green
                case "Skeleton":
                    return new SolidColorBrush(Color.FromRgb(158, 158, 158)); // Gray
                case "Goblin":
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80));   // Light Green
                case "Wizard":
                    return new SolidColorBrush(Color.FromRgb(103, 58, 183));  // Deep Purple
                default:
                    return new SolidColorBrush(Color.FromRgb(66, 165, 245));  // Default Blue
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}