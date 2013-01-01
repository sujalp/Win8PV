using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8PV
{
    class Month : Image
    {        
        public Month(string month, string filepath, uint count) 
        { 
            M = month; 
            ImagePath = filepath; 
            Count = count;
        }

        public string M     { get; set; }
        public uint   Count { get; set; }
    }
    class Months : ObservableCollection<Month>
    {
    }
}
