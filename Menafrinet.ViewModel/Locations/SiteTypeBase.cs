using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel.Locations
{
    public class SiteTypeBase : ISiteType
    {
        public ObservableCollection<ISite> Sites { get; set; }

        public SiteTypeBase()
        {
            Sites = new ObservableCollection<ISite>();
        }
    }
}
