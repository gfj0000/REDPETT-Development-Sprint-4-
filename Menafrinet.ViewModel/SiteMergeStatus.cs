using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel
{
    public class SiteMergeStatus
    {
        public string SiteName { get; set; }
        public string SiteCode { get; set; }
        public DateTime? LastMerged { get; set; }
        public int DaysElapsed { get; set; }
    }
}
