﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel.Locations
{
    public interface ISiteType
    {
        ObservableCollection<ISite> Sites { get; set; }
    }
}
