using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Epi;
//using CDC.SampleShell.Domain;
using Menafrinet.ViewModel;

namespace CDC.SampleShell.Applications
{
    public class ReportsViewModel : ViewModelBase
    {
        private readonly Project _project;
        private string _htmlSource = String.Empty;

        public ObservableCollection<FileInfo> ReportFileNames { get; private set; }
        public ICollectionView ReportFileNamesView { get; private set; }

        public string HtmlSource
        {
            get
            {
                return _htmlSource;
            }
            set
            {
                _htmlSource = value;
                //RaisePropertyChanged("HtmlSource");                   //MLHIDE
            }
        }

        public ReportsViewModel(Project project)
        {
            // pre
            Contract.Requires(project != null);

            // post
            Contract.Ensures(_project != null);

            _project = project;
            ReportFileNames = new ObservableCollection<FileInfo>();

            PopulateFileNames();
        }

        private void PopulateFileNames()
        {
            ReportFileNames.Clear();

            FileInfo projectFileInfo = new FileInfo("Projects\\TrackingMaster\\Reports");
            DirectoryInfo projectDirectory = projectFileInfo.Directory;

            var files = projectDirectory.GetFiles("*.cvs7", SearchOption.TopDirectoryOnly); //MLHIDE

            foreach (FileInfo fi in files)
            {
                ReportFileNames.Add(fi);
            }

            ReportFileNamesView = new ListCollectionView(ReportFileNames);
        }

        private bool CanExecuteRegenerateReports()
        {
            if (ReportFileNames.Count == 0) return false;
            return true;
        }

        public ICommand RegenerateReportsCommand { get { return new RelayCommand(RegenerateReports, CanExecuteRegenerateReports); } }
        private void RegenerateReports()
        {
            foreach (FileInfo fi in ReportFileNames)
            {
                if (fi != null)
                {
                    string fileName = fi.FullName;
                    string htmlFileName = fileName.Replace(".cvs7", ".html"); //MLHIDE

                    System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
                    string commandText = System.IO.Path.GetDirectoryName(a.Location) + "\\AnalysisDashboard.exe"; //MLHIDE

                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = commandText;
                    proc.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\" /minimized:\"{2}\"", fileName, htmlFileName, true);
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                    proc.WaitForExit();
                    HtmlSource = htmlFileName;
                }
            }
        }
    }
}
