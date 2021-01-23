using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel.Locations
{
    public class MoH : SiteTypeBase
    {
        public override string ToString()
        {
            //return base.ToString();
            return "Ministry of Health";
        }

        public MoH()
            : base()
        {
        }
    }
}
