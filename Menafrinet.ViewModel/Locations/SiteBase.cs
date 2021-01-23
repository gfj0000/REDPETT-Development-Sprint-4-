using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel.Locations
{
    public class SiteBase : ISite
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int SystemLevel { get; set; }

        public SiteBase(string name)
        {
            this.Name = name;
        }

        public SiteBase(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }
    }
}
