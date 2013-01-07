using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Win8PV.Common;
using Windows.Storage.Streams;
using System.IO;

namespace Win8PV
{
    class OneAlbumViewModel : BasicViewModel
    {
        private Images m_allImages;
        public Images AllImages
        {
            get { return m_allImages; }
            set { SetProperty(ref m_allImages, value); }
        }
        private Images m_Images;
        public Images Images
        {
            get { return m_Images; }
            set { SetProperty(ref m_Images, value); }
        }
        private Image m_mainImage;
        public Image MainImage
        {
            get { return m_mainImage; }
            set { SetProperty(ref m_mainImage, value); }
        }

        private string m_Year;
        public string Year
        {
            get { return m_Year; }
            set { SetProperty(ref m_Year, value); }
        }

        private string m_Month;
        public string Month
        {
            get { return m_Month; }
            set { SetProperty(ref m_Month, value); }
        }

        private string m_AlbumTitle;
        public string AlbumTitle
        {
            get { return m_AlbumTitle; }
            set { SetProperty(ref m_AlbumTitle, value); }
        }

        public OneAlbumViewModel(string year, string month, string albumid)
        {
            Year = year;
            Month = month;
            ReadMyXML(albumid);
        }

        public async void ReadMyXML(string albumid)
        {
            Images = new Images();
            AllImages = new Images();

            Progress<int> progress = new Progress<int>((p) => { ProgressPercent = p; });

            BasicFileDownloader bidl = new BasicFileDownloader(ToAbsoluteUri("xmlonealbum.aspx?ah=" + albumid));
            IRandomAccessStream s = await bidl.DownloadAsync(progress);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.Async = true;
            XmlReader reader = XmlReader.Create(s.AsStream(), settings);
            reader.ReadStartElement("Model");
            reader.ReadStartElement("PhotoList");
            Count = 0;
            string albumtitle = "";
            while (reader.IsStartElement())
            {
                string main = reader[3];
                albumtitle = reader[2];
                string phototitle = reader[1];
                string str = reader[0];
                str = str.Replace(".jpg", "");

                OneImage oi = new OneImage(phototitle, str, Count);
                AllImages.Add(oi);
                if (char.ToUpper(main[0]) == 'Y')
                {
                    MainImage = oi;
                }
                else
                {
                    Images.Add(oi);
                }
                //var donotuse = oi.MediumSizeStream; // Access it here - so that the download for it kicks off
                Count++;

                await reader.ReadAsync();
            }
            AlbumTitle = albumtitle;
        }
    }
}
