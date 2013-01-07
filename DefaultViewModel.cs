using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Win8PV.Common;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace Win8PV
{
    class BasicViewModel : BindableBase
    {
        private int m_progressPercent;
        public int ProgressPercent
        {
            get { return m_progressPercent; }
            set { SetProperty(ref m_progressPercent, value); }
        }
        private uint m_count;
        public uint Count
        {
            get { return m_count; }
            set { SetProperty(ref m_count, value); }
        }
        protected static Uri ToAbsoluteUri(string u)
        {
            return new Uri(@"http://parikhs.homeserver.com/" + u, UriKind.Absolute);
        }
    }

    class DefaultViewModel : BasicViewModel
    {
        private Years m_years;
        public Years Years
        {
            get { return m_years; }
            set { SetProperty(ref m_years, value); }
        }
        private Year m_firstYear;
        public Year FirstYear
        {
            get { return m_firstYear; }
            set { SetProperty(ref m_firstYear, value); }
        }

        public DefaultViewModel()
        {
            ReadMyXML();
        }

        public async void ReadMyXML()
        {
            Uri uriYears = ToAbsoluteUri("xmlyears.aspx");

            App app = (App)Application.Current;
            if (app.BackgroundDownloader != null)
            {
                if (app.BackgroundDownloader.Status == TaskStatus.RanToCompletion)
                {
                    bool b = await app.BackgroundDownloader;
                    if (b)
                    {
                        try
                        {
                            var y = Windows.Storage.ApplicationData.Current.LocalFolder;
                            string subdir = BasicFileDownloader.UriToSubDirectory(uriYears);
                            string fname = BasicFileDownloader.UriToFilename(uriYears);
                            string backupfname = BasicFileDownloader.UriToFilename(uriYears) + ".bak";

                            var sf = await y.GetFileAsync(subdir + "\\" + fname);
                            await sf.DeleteAsync();
                            sf = await y.GetFileAsync(subdir + "\\" + backupfname);
                            await sf.RenameAsync(fname);
                        }
                        catch { }
                    }
                }
            } 
                    
            Years = new Years();

            Progress<int> progress = new Progress<int>((p) => { ProgressPercent = p; });

            BasicFileDownloader bidl = new BasicFileDownloader(uriYears);
            IRandomAccessStream s = await bidl.DownloadAsync(progress);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.Async = true;
            XmlReader reader = XmlReader.Create(s.AsStream(), settings);
            reader.ReadStartElement("Model");
            reader.ReadStartElement("Years");
            Count = 0;
            while (reader.IsStartElement())
            {
                string year = reader[0];
                string str = reader[1];
                str = str.Replace("_s.jpg", "");
                if (!String.IsNullOrEmpty(str))
                {
                    uint count = 0;
                    if (uint.TryParse(reader[2], out count))
                    {
                        Year y = new Year(year, str, count);
                        if (FirstYear == null)
                            FirstYear = y;
                        else
                            Years.Add(y);
                        Count += y.Count;
                    }
                }
                await reader.ReadAsync();
            }

            s.Dispose();

            BasicFileDownloader bfdlForce = new BasicFileDownloader(uriYears);
            app.BackgroundDownloader = bfdlForce.DownloadAsyncForce(null);
        }
    }
}
