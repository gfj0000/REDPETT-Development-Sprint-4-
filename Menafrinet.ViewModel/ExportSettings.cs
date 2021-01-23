using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel
{
    public struct ExportSettings
    {
        public DirectoryInfo OutputDirectory;
        public string FileName;
        public DateTime DateExported;
        public DateTime? StartDate;
        public DateTime? EndDate;
        public ExportType ExportType;
        public string SourceLanguage;
        public Epi.View CaseForm;
        public Epi.Data.IDbDriver Database;
        public Epi.Project Project;
        public string ProjectPath;
        public LabExportTypes LabExportType;
        //public string LabExportDestinationCode; // deprecated
        public DirectoryInfo ArchiveDirectory; // added **
    }

    public enum ExportType
    {
        AllRecords,
        AllRecordsSinceLast,
        LastMonth,
        DateRange
    }
}
