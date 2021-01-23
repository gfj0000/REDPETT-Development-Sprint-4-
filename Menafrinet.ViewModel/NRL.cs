using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel.Locations
{
    public class NRL : SiteTypeBase
    {
                public override string ToString()
        {
            //return base.ToString();
            return "National Reference Lab";
        }

        public NRL()
            : base()
        {
        }
    }
}
