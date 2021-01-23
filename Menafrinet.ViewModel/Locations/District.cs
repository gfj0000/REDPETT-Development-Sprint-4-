using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel.Locations
{
    public class District : SiteBase
    {
        public string Region { get; set; }

        public District(string name, string code)
            : base(name, code)
        {
        }

        public District(string name, string code, string region)
            : base(name, code)
        {
            this.Region = region;
        }
    }
}
