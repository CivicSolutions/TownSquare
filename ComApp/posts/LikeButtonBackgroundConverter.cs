using System.Globalization;

namespace comApp.posts
{
    public class LikeButtonBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isLiked = (bool)value;
            return isLiked ? Color.FromArgb("#E0245E") : Color.FromArgb("#bbb"); // red or grey
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}