using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Win8PV
{
    public class BasicFileDownloader
    {
        private Uri m_u;

        public static string UriToSubDirectory(Uri u)
        {
            byte bsubdir = 0;

            char[] ba = u.AbsoluteUri.ToCharArray();
            foreach (var b in ba)
            {
                bsubdir ^= (byte)b;
            }

            return bsubdir.ToString();
        }

        public static string UriToFilename(Uri u)
        {
            string s = u.AbsoluteUri;
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (Char.IsLetterOrDigit(c) || c == '.')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public BasicFileDownloader(Uri u)
        {
            m_u = u;
        }

        public async Task<IRandomAccessStream> DownloadAsyncFromFile(IProgress<int> progress)
        {
            var y = Windows.Storage.ApplicationData.Current.LocalFolder;
            string subdir = UriToSubDirectory(m_u);
            string fname = UriToFilename(m_u);

            // First check if it exists in the cache
            try
            {
                // If it does, then simply return that stream to the client
                StorageFile sfile = await y.GetFileAsync(subdir + "\\" + fname);
                var st = await sfile.OpenReadAsync();
                if (progress != null) progress.Report(100);
                return st;
            }
            catch
            {
                // File does not exist
                return null;
            }
        }

        public async Task<bool> DownloadAsyncForce(IProgress<int> progress)
        {
            bool fSaved = false;

            try
            {
                // Download from the inter-tubes
                HttpWebRequest hwreq = WebRequest.Create(m_u) as HttpWebRequest;
                HttpWebResponse hwresp = (await hwreq.GetResponseAsync()) as HttpWebResponse;

                if (hwresp.ResponseUri.AbsoluteUri == m_u.AbsoluteUri)
                {
                    Stream stream = hwresp.GetResponseStream();
                    BasicFileSaver bis = new BasicFileSaver(m_u, stream);
                    fSaved = await bis.SaveAsync(true, null);
                }
            }
            catch
            {
            }
            return fSaved;
        }

        public async Task<IRandomAccessStream> DownloadAsync(IProgress<int> progress)
        {
            IRandomAccessStream iras = await DownloadAsyncFromFile(progress);
            if (iras != null)
            {
                return iras;
            }
            else
            {
                bool fSaved;

                try
                {
                    // Download from the inter-tubes
                    HttpWebRequest hwreq = WebRequest.Create(m_u) as HttpWebRequest;
                    HttpWebResponse hwresp = (await hwreq.GetResponseAsync()) as HttpWebResponse;

                    if (hwresp.ResponseUri.AbsoluteUri == m_u.AbsoluteUri)
                    {
                        Stream stream = hwresp.GetResponseStream();

                        // Save the file out locally - note this moves the stream forward to the end
                        BasicFileSaver bis = new BasicFileSaver(m_u, stream);
                        Progress<ulong> newProgress = null;
                        if (progress != null)
                        {
                            newProgress = new Progress<ulong>((p) =>
                                                            {
                                                                var percent = (p * 100) / (ulong)hwresp.ContentLength;
                                                                progress.Report((int)percent);
                                                            });
                        }

                        fSaved = await bis.SaveAsync(false, newProgress);
                        if (progress != null)
                        {
                            progress.Report(100);
                        }
                    }
                    else
                    {
                        fSaved = false;
                    }
                }
                catch
                {
                    fSaved = false;
                }

                // If we managed to save the file, then read off of the disk - from the place we just saved
                return fSaved ? await DownloadAsyncFromFile(progress) : null;
            }
        }
    }
}
