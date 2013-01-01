using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8PV
{
    class Album : Image
    {
        public Album(string albumid, string title, string filepath, uint count)
        {
            Id = albumid;
            A = title;
            ImagePath = filepath;
            Count = count;
        }

        public string Id { get; set; }
        public string A { get; set; }
        public uint Count { get; set; }
    }
    class Albums : ObservableCollection<Album>
    {
    }
}
