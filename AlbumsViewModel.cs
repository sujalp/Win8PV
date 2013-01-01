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
    class AlbumsViewModel : BasicViewModel
    {
        private Albums m_albums;
        public Albums Albums
        {
            get { return m_albums; }
            set { SetProperty(ref m_albums, value); }
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

        public AlbumsViewModel(string year, string month)
        {
            Year = year;
            Month = month;
            ReadMyXML(year, month);
        }

        public async void ReadMyXML(string year, string month)
        {
            Albums = new Albums();

            Progress<int> progress = new Progress<int>((p) => { ProgressPercent = p; });

            BasicFileDownloader bidl = new BasicFileDownloader(ToAbsoluteUri("xmlalbums.aspx?ay=" + year + "&am=" + month));
            IRandomAccessStream s = await bidl.DownloadAsync(progress);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.Async = true;
            XmlReader reader = XmlReader.Create(s.AsStream(), settings);
            reader.ReadStartElement("Model");
            reader.ReadStartElement("Albums");
            Count = 0;
            while (reader.IsStartElement())
            {
                string albumid = reader[0];
                string album = reader[2];
                string str = reader[1];
                str = str.Replace("_s.jpg", "");
                uint count = 0;
                if (uint.TryParse(reader[3], out count))
                {
                    Album m = new Album(albumid, album, str, count);
                    Albums.Add(m);
                    Count += m.Count;
                }
                await reader.ReadAsync();
            }
        }
    }
}
