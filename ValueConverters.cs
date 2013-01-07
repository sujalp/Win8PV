using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Win8PV
{
    public abstract class AbstractConverter : IValueConverter
    {
        public abstract object Convert(object value);
        public virtual object Convert(object value, object parameter)
        {
            return Convert(value);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return parameter == null ? Convert(value) : Convert(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class StreamToBitmap : AbstractConverter
    {
        private static BitmapImage s_stockBitmapImage;

        static StreamToBitmap()
        {
            s_stockBitmapImage = new BitmapImage(new Uri("ms-appx:///Images/PlaceHolder.png"));
        }

        public override object Convert(object value)
        {
            return Convert(value, "No");
        }

        public override object Convert(object value, object parameter)
        {
            string sync = (string)parameter;
            bool fSync = sync == "Sync";
            if (value == null) return s_stockBitmapImage;
            IRandomAccessStream s = value as IRandomAccessStream;
            IRandomAccessStream snew = s.CloneStream();
            BitmapImage bitmapImage = new BitmapImage();
            if (fSync)
            {
                bitmapImage.SetSource(snew);
            }
            else
            {
                bitmapImage.SetSourceAsync(snew);
            }
            return bitmapImage;
        }
    }

    public sealed class Braceify : AbstractConverter
    {
        public override object Convert(object value)
        {
            if (value == null) return null;
            return "(" + value.ToString() + ")";
        }
    }

    public sealed class Colorify : AbstractConverter
    {
        static Random r = new Random();

        static ulong[] colors = {
        0x289898, 0x327C7C, 0x137171, 0x30A1A1, 0x34A1A1,
        0x3669A6, 0x3D5F87, 0x1A467B, 0x3E71AE, 0x4273AE,
        0x30B666, 0x3B9560, 0x178844, 0x38BC6E, 0x3DBC71 };

        public override object Convert(object value)
        {
            ulong sc = colors[r.Next(colors.Count())];
            Color c = new Color();
            c.R = (byte)((0xff0000 & sc) >> 16);
            c.G = (byte)((0xff00 & sc) >> 8);
            c.B = (byte)((0xff & sc));
            c.A = 0xff;
            return c;
        }
    }

    public sealed class ZeroToVisible : AbstractConverter
    {
        public override object Convert(object value)
        {
            return ((int)value) == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public sealed class ZeroToHidden : AbstractConverter
    {
        public override object Convert(object value)
        {
            return ((int)value) == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public sealed class Not0Not100 : AbstractConverter
    {
        public override object Convert(object value)
        {
            int val = (int)value;
            return (val == 0 || val == 100) ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public sealed class GetHeader : AbstractConverter
    {
        public override object Convert(object value)
        {
            return "Parikh Photos for the year " + value.ToString();
        }
    }

    public sealed class GetHeaderSm : AbstractConverter
    {
        public override object Convert(object value)
        {
            return "Photos for " + value.ToString();
        }
    }

    public sealed class GetHeaderMY : AbstractConverter
    {
        public override object Convert(object value)
        {
            AlbumsViewModel avm = (AlbumsViewModel)value;
            return "Photos for " + ToMonthString.GetMonth(avm.Month, false) + " " + avm.Year;
        }
    }

    public sealed class GetHeaderMYSmall : AbstractConverter
    {
        public override object Convert(object value)
        {
            AlbumsViewModel avm = (AlbumsViewModel)value;
            return ToMonthString.GetMonth(avm.Month, true) + " " + avm.Year;
        }
    }

    public sealed class GetHeaderMYFromOneAlbum : AbstractConverter
    {
        public override object Convert(object value)
        {
            OneAlbumViewModel oavm = (OneAlbumViewModel)value;
            return ToMonthString.GetMonth(oavm.Month, false) + " " + oavm.Year;
        }
    }

    public sealed class GetHeaderMYFromOneAlbumSm : AbstractConverter
    {
        public override object Convert(object value)
        {
            OneAlbumViewModel oavm = (OneAlbumViewModel)value;
            return ToMonthString.GetMonth(oavm.Month, true) + " " + oavm.Year;
        }
    }

    public sealed class ToMonthString : AbstractConverter
    {
        public static string GetMonth(string month, bool small)
        {
            int iMonth;
            if (int.TryParse(month, out iMonth))
            {
                if (iMonth >= 1 && iMonth <= 12)
                {
                    DateTime dt = new DateTime(2012, iMonth, 1);
                    return dt.ToString(small ? "MMM" : "MMMMMMMMMMMMMMM");
                }
            }
            return "";
        }

        public override object Convert(Object value)
        {
            return GetMonth((string)value, false);
        }
    }

    public sealed class ToMonthStringSm : AbstractConverter
    {
        public override object Convert(Object value)
        {
            return ToMonthString.GetMonth((string)value, true);
        }
    }

    public sealed class PlusOne : AbstractConverter
    {
        public override object Convert(object value)
        {
            return ((int)value) + 1;
        }
    }
}