using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel.Locations
{
    public interface ISite
    {
        string Name { get; set; }
        string Code { get; set; }
        int SystemLevel { get; set; }
    }
}
