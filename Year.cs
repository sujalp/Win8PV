using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Win8PV.Common;
using Windows.Storage.Streams;

namespace Win8PV
{
    public class Image : BindableBase
    {
        public uint DbgIndex { get; protected set; }

        private class DownloadItem
        {
            public DownloadItem(string subpath)
            {
                SubPath = subpath;
                ImageStream = null;
                State = STATES.NOTSTARTED;
                Size = SIZES.NONE;
                CTS = null;
            }
            public string SubPath;
            public IRandomAccessStream ImageStream;
            public STATES State;
            public SIZES Size;
            public CancellationTokenSource CTS;
        }

        private enum STATES { NOTSTARTED, ATTEMPTING, SUCCEEDED, FAILED };
        private enum SIZES { SMALL, MEDIUM, LARGE, TOTAL, NONE };
        private DownloadItem[] m_DownloadMap = new DownloadItem[(int)SIZES.TOTAL] {
                new DownloadItem("_m.jpg"),     // Small thumbnail
                new DownloadItem(".jpg"),       // Medium thumbnail
                new DownloadItem("_b.jpg")      // Actual image
        };
        public string ImagePath; // Never databind to this

        public IRandomAccessStream SmallSizeStream
        {
            get
            {
                GetStream(SIZES.SMALL);
                return m_DownloadMap[(int)SIZES.SMALL].ImageStream;
            }
            set
            {
                OnPropertyChanged();
            }
        }

        public IRandomAccessStream MediumSizeStream
        {
            get
            {
                GetStream(SIZES.MEDIUM);
                return m_DownloadMap[(int)SIZES.MEDIUM].ImageStream;
            }
            set
            {
                OnPropertyChanged();
            }
        }

        private int m_largeProgressPercent;
        public int LargeProgressPercent
        {
            get { return m_largeProgressPercent; }
            set { SetProperty(ref m_largeProgressPercent, value); }
        }

        public IRandomAccessStream LargeSizeStream
        {
            get
            {
                Progress<int> progress = new Progress<int>((p) => { LargeProgressPercent = p; });
                GetStream(SIZES.LARGE, progress);
                return m_DownloadMap[(int)SIZES.LARGE].ImageStream;
            }
            set
            {
                OnPropertyChanged();
            }
        }

        public void CancelAllActiveDownloads()
        {
            int i = 0;
            foreach (var di in m_DownloadMap)
            {
                if (di.CTS != null)
                {
                    di.CTS.Cancel();
                    Debug.WriteLine(@"/\ Image: " + DbgIndex + " Size: " + ((SIZES)i).ToString() + " - Aborted" + " " + ImagePath + di.SubPath);
                }
                i++;
            }
        }

        private void SetStream(DownloadItem di, IRandomAccessStream iras, SIZES size)
        {
            di.Size = (SIZES)size;
            di.ImageStream = iras;

            switch (size)
            {
                case SIZES.SMALL: SmallSizeStream = null; break;
                case SIZES.MEDIUM: MediumSizeStream = null; break;
                case SIZES.LARGE: LargeSizeStream = null; break;
            }
        }

        private async void GetStream(SIZES size, Progress<int> progress = null)
        {
            DownloadItem di = m_DownloadMap[(int)size];
            if (di.State == STATES.NOTSTARTED)
            {
                IRandomAccessStream st;
                di.State = STATES.ATTEMPTING;
                Debug.WriteLine(@"/\ Attempting Image: " + DbgIndex + " Size: " + size.ToString() + " " + ImagePath + di.SubPath);
                di.CTS = new CancellationTokenSource();
                Uri u = new Uri(ImagePath + di.SubPath, UriKind.Absolute);
                BasicFileDownloader bidl = new BasicFileDownloader(u);
                st = await bidl.DownloadAsync(progress, di.CTS.Token);
                if (st == null)
                {
                    di.State = di.CTS.IsCancellationRequested ? STATES.NOTSTARTED : STATES.FAILED;
                    if (di.CTS.IsCancellationRequested)
                        Debug.WriteLine(@"/\ Cancelled Image: " + DbgIndex + " Size: " + size.ToString() + " " + ImagePath + di.SubPath);
                    else
                        Debug.WriteLine(@"/\ Failed Image: " + DbgIndex + " Size: " + size.ToString() + " " + ImagePath + di.SubPath);
                    di.CTS = null;
                }
                else
                {
                    di.State = STATES.SUCCEEDED;
                    di.CTS = null;
                    Debug.WriteLine(@"/\ Finished Image: " + DbgIndex + " Size: " + size.ToString() + " " + ImagePath + di.SubPath);
                    SetStream(di, st, size);
                }

                if (st != null)
                {
                    for (int k = 0; k < (int)SIZES.TOTAL; k++)
                    {
                        if (k == (int)size) continue;

                        di = m_DownloadMap[k];

                        switch (di.State)
                        {
                            case STATES.SUCCEEDED:
                                break;
                            case STATES.NOTSTARTED:
                            case STATES.ATTEMPTING:
                                SetStream(di, st, size);
                                break;
                            case STATES.FAILED:
                                if ((di.ImageStream == null) || (di.Size < size))
                                {
                                    SetStream(di, st, size);
                                }
                                break;
                        }
                    }
                }
            }
        }
    }

    class Year : Image
    {
        public Year(string year, string filepath, uint count) 
        { 
            Y = year; 
            ImagePath = filepath; 
            Count = count;
        }

        public string Y     { get; set; }
        public uint   Count { get; set; }
    }

    class Years : ObservableCollection<Year>
    {
    }
}
