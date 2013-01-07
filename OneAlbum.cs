using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8PV
{
    class OneImage : Image
    {
        public OneImage(string title, string filepath, uint dbgIndex)
        {
            Title = title;
            ImagePath = filepath;
            DbgIndex = dbgIndex;
        }

        public string Title { get; set; }
    }
    class Images : ObservableCollection<OneImage>
    {
    }
}
