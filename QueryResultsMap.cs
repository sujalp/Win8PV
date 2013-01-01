using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Win8PV
{
    class QueryResultsMapEntry
    {
        string Query;
        string ResultFile;
    }

    class QueryResultsMap
    {
        QueryResultsMap()
        {
            StorageFolder sf = ApplicationData.Current.LocalFolder;
            System.Guid.NewGuid().ToString();
        }
    }
}
