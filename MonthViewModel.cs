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
    class MonthViewModel : BasicViewModel
    {
        private Months m_months;
        public Months Months
        {
            get { return m_months; }
            set { SetProperty(ref m_months, value); }
        }

        private string m_Year;
        public string Year
        {
            get { return m_Year; }
            set { SetProperty(ref m_Year, value); }
        }

        public MonthViewModel(string year)
        {
            Year = year;
            ReadMyXML(year);
        }

        public async void ReadMyXML(string year)
        {
            Months = new Months();

            Progress<int> progress = new Progress<int>((p) => { ProgressPercent = p; });

            BasicFileDownloader bidl = new BasicFileDownloader(ToAbsoluteUri("xmlmonths.aspx?ay=" + year));
            IRandomAccessStream s = await bidl.DownloadAsync(progress);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.Async = true;
            XmlReader reader = XmlReader.Create(s.AsStream(), settings);
            reader.ReadStartElement("Model");
            reader.ReadStartElement("Months");
            Count = 0;
            while (reader.IsStartElement())
            {
                string month = reader[0];
                string str = reader[1];
                str = str.Replace("_s.jpg", "");
                if (!String.IsNullOrEmpty(str))
                {
                    uint count = 0;
                    if (uint.TryParse(reader[2], out count))
                    {
                        Month m = new Month(month, str, count);
                        Months.Add(m);
                        Count += m.Count;
                    }
                }
                await reader.ReadAsync();
            }
        }
    }
}
