using System;
using comApp.posts;

namespace comApp.posts
{
    public class TimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                TimeSpan timeDifference = DateTime.Now - dateTime;

                if (timeDifference.TotalMinutes < 1)
                {
                    return "just now";
                }
                else if (timeDifference.TotalMinutes < 60)
                {
                    return $"{(int)timeDifference.TotalMinutes} minutes ago";
                }
                else if (timeDifference.TotalHours < 24)
                {
                    return $"{(int)timeDifference.TotalHours} hours ago";
                }
                else
                {
                    return $"{(int)timeDifference.TotalDays} days ago";
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
