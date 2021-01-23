using System;
using Epi;

namespace Menafrinet.ViewModel
{
    /// <summary>
    /// An interface for the data management layer for the Menafrinet application
    /// </summary>
    public interface IDataHelper
    {
        View CaseForm { get; set; }
        Project Project { get; set; }

        void RepopulateCollections();
    }
}