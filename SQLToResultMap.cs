using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Win8PV
{
    public class SQLToResultMap
    {
        [XmlAttribute]
        public string SQL { get; set; }

        [XmlAttribute]
        public string ResultFile { get; set; }
    }

    public class Metadata
    {
        [XmlArray]
        public List<SQLToResultMap> QueryResults;

        private Metadata() {}

        public static async Task<Metadata> LoadMetaDataFile()
        {
            Metadata metadata;
            StorageFile sf;
            var y = Windows.Storage.ApplicationData.Current.LocalFolder;
            try
            {
                sf = await y.GetFileAsync("MetaData.xml");
                var s = await sf.OpenAsync(FileAccessMode.Read);
                XmlSerializer ds = new XmlSerializer(typeof(Metadata));
                metadata = (Metadata) ds.Deserialize(s.AsStreamForRead());
            }
            catch (Exception ex)
            {
                sf = null;
                metadata = new Metadata();
                metadata.QueryResults = new List<SQLToResultMap>();
            }
            return metadata;
        }
    }
}
