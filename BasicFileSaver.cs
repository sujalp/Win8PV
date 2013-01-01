﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace Win8PV
{
    class BasicFileSaver
    {
        private Uri m_uri;
        Stream m_s;

        public BasicFileSaver(Uri u, Stream s)
        {
            m_uri = u;
            m_s = s;
        }

        private static async Task ReadWriteStreamAsync(Stream readStream, Stream writeStream, IProgress<ulong> progress)
        {
            int Length = 1024;
            Byte[] buffer1 = new Byte[Length];
            Byte[] buffer2 = new Byte[Length];
            bool writebufferis1 = true;
            int bytesRead1, bytesRead2 = 0;
            Task writertask;
            ulong total = 0;

            bytesRead1 = await readStream.ReadAsync(buffer1, 0, Length);
            bool keepgoing = bytesRead1 > 0;

            while (keepgoing)
            {
                if (writebufferis1)
                {
                    writertask = writeStream.WriteAsync(buffer1, 0, bytesRead1);
                    total += (ulong)bytesRead1;
                    bytesRead2 = await readStream.ReadAsync(buffer2, 0, Length);
                    keepgoing = bytesRead2 > 0;
                }
                else
                {
                    writertask = writeStream.WriteAsync(buffer2, 0, bytesRead2);
                    total += (ulong)bytesRead2;
                    bytesRead1 = await readStream.ReadAsync(buffer1, 0, Length);
                    keepgoing = bytesRead1 > 0;
                }
                await writertask;
                writebufferis1 = !writebufferis1;

                if (progress != null)
                    progress.Report(total);
            }
        }

        public async Task<bool> SaveAsync(bool fBackup, IProgress<ulong> progress)
        {
            string subdir = BasicFileDownloader.UriToSubDirectory(m_uri);
            string fname = BasicFileDownloader.UriToFilename(m_uri) + (fBackup ? ".bak" : "");
            IStorageItem si = null;
            bool fSuccess = false;
            StorageFile sfile = null;

            // Check if the folder has been created
            var y = Windows.Storage.ApplicationData.Current.LocalFolder;
            try
            {
                Task<IStorageItem> ti = y.GetItemAsync(subdir).AsTask();
                ti.Wait();
                si = ti.Result;
            }
            catch
            {
            }

            // If not, then create it
            StorageFolder sf = (si == null) ? null : (si as StorageFolder);
            if (sf == null)
            {
                try
                {
                    Task<StorageFolder> ti = y.CreateFolderAsync(subdir).AsTask();
                    ti.Wait();
                    sf = ti.Result;
                }
                catch
                {
                }
            }

            // Now we will have a folder, delta errors
            if (sf != null)
            {
                try
                {
                    sfile = await sf.CreateFileAsync(fname, fBackup ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.FailIfExists);
                    if (sfile != null)
                    {
                        using (Stream outstream = await sfile.OpenStreamForWriteAsync())
                        {
                            await ReadWriteStreamAsync(m_s, outstream, progress);
                            await outstream.FlushAsync();
                        }
                        fSuccess = true;
                    }
                }
                catch
                {
                }
            }

            // If we were able to create a file but did not successfully download a file - then we need to delete
            // the file so as not to leave turds around
            if (!fSuccess && sfile != null)
            {
                try
                {
                    await sfile.DeleteAsync();
                }
                catch
                {
                }
            }

            // Note - we catch all errors - if we cannot save - it is not the end of the world - keep motoring on
            // We do however notify our customer if we were able to save or not
            return fSuccess;
        }
    }
}
