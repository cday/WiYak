using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WiYak
{
    public class MessageConverter : IValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(Thickness))
            {
                return ((bool)value) ? new Thickness(70, 30, 0, 0) : new Thickness(10, 30, 0, 0);
            }
            else if (targetType == typeof(HorizontalAlignment))
            {
                return ((bool)value) ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            }
            else if (targetType == typeof(Geometry))
            {
                PathFigure figure = new PathFigure();
                LineSegment lineX = new LineSegment();
                LineSegment lineY = new LineSegment();
                PathSegmentCollection collection = new PathSegmentCollection();
                PathFigureCollection figureCollection = new PathFigureCollection();
                PathGeometry pathGeo = new PathGeometry();

                if ((bool)value)
                {
                    figure.StartPoint = new Point(16, 12);
                    lineX.Point = new Point(16, 0);
                    lineY.Point = new Point(0, 12);
                }
                else
                {
                    figure.StartPoint = new Point(0, 0);
                    lineX.Point = new Point(0, 12);
                    lineY.Point = new Point(16, 12);
                }

                collection.Add(lineX);
                collection.Add(lineY);
                figure.Segments = collection;
                figureCollection.Add(figure);
                pathGeo.Figures = figureCollection;

                return pathGeo;
            }
            else if (targetType == typeof(string))
            {
                string temp = "";

                int count = (int)value;

                temp += count.ToString();

                temp += " unread messages";

                return temp;
            }
            else if (targetType == typeof(Visibility))
            {
                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
