using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Win8PV
{
    class UniqueNumberGenerator
    {
        public string Create()
        {
            return "F" + Guid.NewGuid().ToString("N");
        }
    }
}
