using System;
using System.Collections.Generic;
using System.Text;

namespace Menafrinet.ViewModel
{
    public class SiteMergeStatusViewModel : ObservableObject
    {
        #region Members
        /// <summary>
        /// Represents the case model
        /// </summary>
        public SiteMergeStatus SiteMergeStatus;
        #endregion

        public string SiteName
        {
            get { return SiteMergeStatus.SiteName; }
            set
            {
                if (SiteMergeStatus.SiteName != value)
                {
                    SiteMergeStatus.SiteName = value;
                    RaisePropertyChanged("SiteName");
                }
            }
        }
        //public string region
        //{
        //    get { return SiteMergeStatus.region; }
        //    set
        //    {
        //        if (SiteMergeStatus.region != value)
        //        {
        //            SiteMergeStatus.region = value;
        //            RaisePropertyChanged("Region");
        //        }
        //    }
        //}
        public string SiteCode
        {
            get { return SiteMergeStatus.SiteCode; }
            set
            {
                if (SiteMergeStatus.SiteCode != value)
                {
                    SiteMergeStatus.SiteCode = value;
                    RaisePropertyChanged("SiteCode");
                }
            }
        }
        public DateTime? LastMerged
        {
            get { return SiteMergeStatus.LastMerged; }
            set
            {
                if (SiteMergeStatus.LastMerged != value)
                {
                    SiteMergeStatus.LastMerged = value;
                    RaisePropertyChanged("LastMerged");
                }
            }
        }
        public int DaysElapsed
        {
            get { return SiteMergeStatus.DaysElapsed; }
            set
            {
                if (SiteMergeStatus.DaysElapsed != value)
                {
                    SiteMergeStatus.DaysElapsed = value;
                    RaisePropertyChanged("DaysElapsed");
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SiteMergeStatusViewModel()
        {
            SiteMergeStatus = new SiteMergeStatus();
        }
    }
}
