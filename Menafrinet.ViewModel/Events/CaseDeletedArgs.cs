using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel.Events
{
    public class CaseDeletedArgs : EventArgs
    {
        public CaseDeletedArgs(string caseGuid)
        {
            this.CaseGuid = caseGuid;
        }

        public string CaseGuid { get; private set; }
    }
}
