using System.Globalization;

namespace comApp.posts
{
    public class LikeButtonIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isLiked = (bool)value;
            return isLiked ? '\uf004' : '\uf08b'; // return char instead of string
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}