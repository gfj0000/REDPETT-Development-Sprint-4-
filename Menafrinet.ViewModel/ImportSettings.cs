using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel
{
    public struct ImportSettings
    {
        public DirectoryInfo InputDirectory;
        public DateTime DateImported;
        public string DestinationLanguage;
        public Epi.View CaseForm;
        public Epi.Data.IDbDriver Database;
        public Epi.Project Project;
        public string ProjectPath;
        public DirectoryInfo ArchiveDirectory;
    }
}
