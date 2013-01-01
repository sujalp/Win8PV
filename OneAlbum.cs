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
        public OneImage(string title, string filepath)
        {
            Title = title;
            ImagePath = filepath;
        }

        public string Title { get; set; }
    }
    class Images : ObservableCollection<OneImage>
    {
    }
}
