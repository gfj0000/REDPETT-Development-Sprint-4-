using MultiLang;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Epi;
using Epi.ImportExport;
using Epi.ImportExport.Filters;
using Epi.ImportExport.ProjectPackagers;
using Menafrinet.ViewModel;
using Menafrinet.View.Controls;
using System.Threading;
using Ionic.Zip;
using System.Diagnostics;
using System.Data.OleDb;
using Epi.Data;
using Epi.Menu;
using System.Data;

namespace Menafrinet.View.Controls
{
    /// <summary>
    /// Interaction logic for MergePanel.xaml
    /// </summary>
    public partial class MergePanel : UserControl
    {
        public delegate void ImportCompletedHandler(List<ImportInfo> info);

        private DataHelper DataHelper
        {
            get
            {
                return (this.DataContext as DataHelper);
            }
        }

        //private ConsultantPanel conPanel
        //{
        //    get
        //    {
        //        return (this.DataContext as ConsultantPanel);
        //    }
        //}

        public bool IsMerging { get; private set; }

        string prjDataTimeStamp;
        private List<ImportInfo> importInfos;
        object permvar;
        object permvar2;
        object permInst;
        string SiteCodePerm_str;
        string UserSettingResident_str;
        string PermWhichInst_str;
        bool UpdateSitesResidents = false;
        string appDir = Environment.CurrentDirectory;
        string InstOnly = "0";

        public MergePanel()
        {
            InitializeComponent();
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(ml.ml_string(121, "Copy and paste the data package or multiple packages in this window that you want to import."), ml.ml_string(141, "Message"), MessageBoxButton.OK, MessageBoxImage.Information);

            if (result == MessageBoxResult.OK)
            {
                FileInfo fi = new FileInfo(DataHelper.Project.FilePath);
                DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(fi.Directory.FullName, Properties.Settings.Default.DataImportFolder));
                Process.Start("explorer.exe", di.FullName); //MLHIDE
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnImport.IsEnabled = false;
                Cursor = Cursors.Wait;

                // open merge folder on click of Import button
                //{
                //    FileInfo fo = new FileInfo(DataHelper.Project.FilePath);
                //    DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(fo.Directory.FullName, Properties.Settings.Default.DataImportFolder));
                //    System.Diagnostics.Process.Start("explorer.exe", di.FullName);

                //}

                //Thread.Sleep(10000);

                if (!(Application.Current as App).CanMerge)
                {
                    MessageBox.Show("You can not merge the data, while generating an export file.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    btnImport.IsEnabled = true;
                    Cursor = Cursors.Arrow;
                    return;
                }

                txtImportStatus.Text = String.Empty;

                ImportSettings settings = new ImportSettings();

                settings.DestinationLanguage = Properties.Settings.Default.Language.ToLower();
                settings.Project = DataHelper.Project;
                settings.ProjectPath = DataHelper.ProjectPath;
                settings.CaseForm = DataHelper.CaseForm;
                settings.Database = DataHelper.Database;

                FileInfo fi = new FileInfo(DataHelper.Project.FilePath);

                settings.InputDirectory = new DirectoryInfo(System.IO.Path.Combine(fi.Directory.FullName, Properties.Settings.Default.DataImportFolder));

                settings.ArchiveDirectory = new DirectoryInfo(System.IO.Path.Combine(settings.InputDirectory.FullName, Properties.Settings.Default.DataImportArchiveFolder));


                if (settings.InputDirectory.GetFiles("*.edp7").Length == 0)
                {
                    MessageBox.Show(ml.ml_string(119, "There are no data packages in the import folder."), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    btnImport.IsEnabled = true;
                    Cursor = Cursors.Arrow;
                    return;
                }

                permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm");

                if (permvar2 != null)
                {
                    if (permvar2.ToString() == "3")
                    {
                        permvar = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident");
                        UserSettingResident_str = permvar.ToString();
                    }
                    else
                    {
                        permvar = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC");
                        UserSettingResident_str = permvar.ToString();
                    }

                    permInst = EpiInfoMenuManager.GetPermanentVariableValue("PermWhichInst");
                    PermWhichInst_str = permInst.ToString();

                }

                if (permvar == null || permvar.ToString() == "" || permvar2 == null || permvar2.ToString() == "")
                {
                    MessageBox.Show(ml.ml_string(118, "You can not merge data if user settings are not properly configured."), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    CallbackSetStatusMessage(String.Format(ml.ml_string(118, "You can not merge data if user settings are not properly configured."), "test")); //MLHIDE

                    return;
                }


                CallbackSetStatusMessage(String.Format(ml.ml_string(122, "Opening - decrypting package..."), "test")); //MLHIDE

                DateTime dt = DateTime.UtcNow;
                string dateDisplayValue = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:s}", dt); //MLHIDE
                dateDisplayValue = dateDisplayValue.Replace(':', '-'); // The : must be replaced otherwise the encryption fails

                btnImport.IsEnabled = false;
                importProgressBar.IsIndeterminate = true;
                (Application.Current as App).CanMerge = false;
                (Application.Current as App).CanTransmit = false;

                this.DataHelper.TaskbarProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;

                BackgroundWorker exportWorker = new BackgroundWorker();
                exportWorker.DoWork += new DoWorkEventHandler(importWorker_DoWork);
                exportWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(importWorker_RunWorkerCompleted);
                exportWorker.RunWorkerAsync(settings);

                btnImport.IsEnabled = true;
                Cursor = Cursors.Arrow;

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);

                // Get the bottom stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                var method = frame.GetMethod().ReflectedType.FullName;
                var path = frame.GetFileName();

                MessageBox.Show(ml.ml_string(137, "An error occurred exporting:") + "\n" + ex.Message + "\n" + "Line: " + line + "\n" + "Method: " + method + "\n" + "Path: " + path); //MLHIDE

                return;
            }
        }

        void importWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<ImportInfo> infos = (List<ImportInfo>)e.Result;
            this.Dispatcher.BeginInvoke(new ImportCompletedHandler(UpdateImportInfo), infos);
            (Application.Current as App).CanMerge = true;
            (Application.Current as App).CanTransmit = true;

            this.DataHelper.TaskbarProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
        }

        void importWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ImportSettings settings = (ImportSettings)e.Argument;
            List<ImportInfo> infos = MergeDataPackages(settings);
            e.Result = infos;
        }

        private List<ImportInfo> MergeDataPackages(ImportSettings settings)
        {

            //try
            //{

            List<ImportInfo> importInfos = new List<ImportInfo>();
            List<FileInfo> packagePaths = new List<FileInfo>();
            List<string> unimportedPackagePaths = new List<string>();

            foreach (System.IO.FileInfo f in settings.InputDirectory.GetFiles("*.edp7")) //MLHIDE
            {
                string fileName = f.FullName;

                if (!fileName.EndsWith(".edp7") ||
                    !f.Name.StartsWith("ProjectTracking_")
                    )
                {
                    unimportedPackagePaths.Add(fileName);
                    CallbackSetStatusMessage(String.Format(ml.ml_string(120, "Package {0} was not included in the merge because of incorrect file names or validation errors."), f.Name));
                }
                else
                {
                    packagePaths.Add(f);
                }
            }


            foreach (FileInfo packagePath in packagePaths)
            {
                //try
                //{
                string fileName = packagePath.FullName;
                //string str = Configuration.DecryptFileToString(fileName, String.Empty);

                //str = str.Remove(0, 24);

                //string plainText = ImportExportHelper.UnZip(fileName);
                ////string plainText = ImportExportHelper.UnZip("ProjectTracking_WIDB ATL Staff_2020-03-11T03-25-15.edp7");

                ////string fileName = packagePath.FullName;
                //str = Configuration.DecryptFileToString(fileName, String.Empty);

                //str = str.Remove(0, 24);

                ////string plainText = ImportExportHelper.UnZip(str);
                XmlDocument xmlDataPackage = new XmlDocument();
                //xmlDataPackage.LoadXml(plainText);

                //xmlDataPackage.Save(@"C:\Users\gfj0\Desktop\New folder\TempXML.xml");

                #region Xml validation
                // First check - never import a package that didn't originate at the proper 'level'. For example,
                // a district shouldn't import from the MoH. System levels are included as attributes in the data
                // package so we can check for that before merging the data.
                //int sourceSystemLevel = int.Parse(xmlDataPackage.ChildNodes[0].Attributes["SystemLevel"].Value.ToString());
                //int currentSystemLevel = Properties.Settings.Default.SystemLevel;

                //if (!(
                //     (currentSystemLevel == 1 && sourceSystemLevel == 0) || // NRL receives data from district
                //     (currentSystemLevel == 1 && sourceSystemLevel == 1) || // NRL receives data from NRL
                //     (currentSystemLevel == 1 && sourceSystemLevel == 2) || // NRL receives data from region
                //                                                            //(currentSystemLevel == 1 && sourceSystemLevel == 3) || // NRL receives data from MOH
                //                                                            //(currentSystemLevel == 1 && sourceSystemLevel == 4) || // NRL receives data from MenAfriNet
                //     (currentSystemLevel == 0 && sourceSystemLevel == 0) || // District receives data from District
                //     (currentSystemLevel == 0 && sourceSystemLevel == 1) || // District receives data from NRL
                //     (currentSystemLevel == 0 && sourceSystemLevel == 2) || // District receives data from Region
                //                                                            //(currentSystemLevel == 0 && sourceSystemLevel == 3) || // District receives data from MoH 
                //                                                            //(currentSystemLevel == 0 && sourceSystemLevel == 4) || // District receives data from MenAfriNet
                //     (currentSystemLevel == 2 && sourceSystemLevel == 0) || // Region receives data from District
                //     (currentSystemLevel == 2 && sourceSystemLevel == 1) || // Region receives data from NRL
                //     (currentSystemLevel == 2 && sourceSystemLevel == 2) || // Region receives data from Region
                //                                                            //(currentSystemLevel == 2 && sourceSystemLevel == 3) || // Region receives data from MOH
                //                                                            //(currentSystemLevel == 2 && sourceSystemLevel == 4) || // Region receives data from MenAfriNet
                //    (currentSystemLevel == 3 && sourceSystemLevel == 0) || // MoH receives data from District
                //    (currentSystemLevel == 3 && sourceSystemLevel == 2) || // MoH receives data from Region
                //    (currentSystemLevel == 3 && sourceSystemLevel == 1) || // MoH receives data from NRL
                //                                                           //(currentSystemLevel == 3 && sourceSystemLevel == 4) || // MoH receives data from MenAfriNet
                //     (currentSystemLevel == 4 && sourceSystemLevel == 3) || // Menafrinet receives data from MoH
                //     (sourceSystemLevel == 3)                               // Any system receives MoH data
                //     ))
                //{
                //    CallbackSetStatusMessage(String.Format("Paquet {0} n'a pas été inclus dans la fusion. C'est au niveau du système est {1}, mais le niveau actuel du système est {2}.", packagePath.Name, sourceSystemLevel.ToString(), currentSystemLevel.ToString()));
                //    continue;
                //}
                #endregion // Xml validation

                //string language = Properties.Settings.Default.Language.ToLower();

                // This is commented out due to field names is now only Engligh
                //if (!language.Equals("en-us") && ((!language.Equals("en-gb") || (language.Equals("en-GB")))))
                //{
                //    CallbackSetStatusMessage(String.Format("Translating field names in data package from {0}...", settings.DestinationLanguage));
                //    xmlDataPackage = Core.Common.TranslateDataPackageFieldsFromInvariant(xmlDataPackage, settings.DestinationLanguage);
                //    CallbackSetStatusMessage("Translation of field names complete.");
                //}

                string currentAppSiteCode = Properties.Settings.Default.SiteCode;

                //string siteNameToLookFor = "Districts"; db name change
                //string siteNameToLookFor = "CountryCode";
                //if (Properties.Settings.Default.SystemLevel == 2)
                //{
                //    //siteNameToLookFor = "drs"; db name change to Region
                //    siteNameToLookFor = "RegCode";
                //    currentAppSiteCode = Properties.Settings.Default.SiteCode.Substring(4, 3);
                //}

                // filter on district or region
                //if (currentSystemLevel == 0 || currentSystemLevel == 2)
                //{
                //    List<XmlNode> nodesToRemove = new List<XmlNode>();

                //    foreach (XmlNode node1 in xmlDataPackage.ChildNodes)
                //    {
                //        foreach (XmlNode node2 in node1.ChildNodes) // records in form
                //        {
                //            foreach (XmlNode node3 in node2.ChildNodes) // fields in record
                //            {

                //                if (!node3.Name.Equals("FieldMetadata", StringComparison.OrdinalIgnoreCase))
                //                {
                //                    foreach (XmlNode node4 in node3.ChildNodes)
                //                    {
                //                        foreach (XmlNode node5 in node4.ChildNodes)
                //                        {
                //                            if (node5.Name.Equals("Field") && node5.Attributes[0].Value.Equals(siteNameToLookFor, StringComparison.OrdinalIgnoreCase))
                //                            {
                //                                string distCodeValue = node5.InnerText;
                //                                //string distCodeValue = "MAI-KID-TIN";

                //                                if (!distCodeValue.Equals(currentAppSiteCode, StringComparison.OrdinalIgnoreCase))
                //                                {
                //                                    nodesToRemove.Add(node4);
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }

                //    for (int i = nodesToRemove.Count - 1; i >= 0; i--)
                //    {
                //        nodesToRemove[i].ParentNode.RemoveChild(nodesToRemove[i]);
                //    }

                //}


                //Epi.Fields.Field epidField = settings.CaseForm.Fields["EpidShow"];
                //Epi.Fields.Field epidField = settings.CaseForm.Fields["EpidNumber"]; db name change chng bk to orig


                XmlMultiKeyDataUnpackager xmlMultiKeyUP = new XmlMultiKeyDataUnpackager(settings.CaseForm, xmlDataPackage);
                //xmlUP.StatusChanged += new UpdateStatusEventHandler(CallbackSetStatusMessage);
                //xmlUP.UpdateProgress += new SetProgressBarDelegate(CallbackSetProgressBar);
                //xmlUP.ResetProgress += new SimpleEventHandler(CallbackResetProgressBar);
                //xmlMultiKeyUP.MessageGenerated += new UpdateStatusEventHandler(CallbackSetStatusMessage);
                //xmlUP.ImportFinished += new EventHandler(xmlUP_ImportFinished);
                xmlMultiKeyUP.Append = true;
                //xmlMultiKeyUP.KeyFields.Add(epidField);
                xmlMultiKeyUP.Update = true;
                xmlMultiKeyUP.Unpackage();
                importInfos.Add(xmlMultiKeyUP.ImportInfo);

                prjDataTimeStamp = packagePath.Name.Replace("ProjectTracking_", ""); //MLHIDE
                                                                                     //prjDataTimeStamp = packagePath.Name.Replace(".edp7", "");

                //CallbackSetStatusMessage(String.Format(ml.ml_string(123, "Merging data for: ") + packagePath.Name, "test"));
                CallbackSetStatusMessage(String.Format(ml.ml_string(179, "Merging data for: ") + packagePath.Name, "test"));


                xmlMultiKeyUP.MessageGenerated += new UpdateStatusEventHandler(CallbackSetStatusMessage);

                using (ZipFile zip = ZipFile.Read("Projects\\TrackingMaster\\DataImports\\" + packagePath.Name)) //MLHIDE
                {
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\Projects.edp7")) //MLHIDE
                    {
                        File.Delete("Projects\\TrackingMaster\\ImportExport\\Projects.edp7"); //MLHIDE
                    }
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ProjectsA.edp7")) //MLHIDE
                    {
                        File.Delete("Projects\\TrackingMaster\\ImportExport\\ProjectsA.edp7"); //MLHIDE
                    }
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ProjectsB.edp7")) //MLHIDE
                    {
                        File.Delete("Projects\\TrackingMaster\\ImportExport\\ProjectsB.edp7"); //MLHIDE
                    }

                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\TraineeInformation.edp7")) //MLHIDE
                    {
                        File.Delete("Projects\\TrackingMaster\\ImportExport\\TraineeInformation.edp7"); //MLHIDE
                    }

                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ConsultantInfo.edp7")) //MLHIDE
                    {
                        File.Delete("Projects\\TrackingMaster\\ImportExport\\ConsultantInfo.edp7"); //MLHIDE
                    }

                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\CodeFieldSites.edp7")) //MLHIDE
                    {
                        File.Delete("Projects\\TrackingMaster\\ImportExport\\CodeFieldSites.edp7"); //MLHIDE
                    }
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\Frontline.edp7")) //MLHIDE
                    {
                        File.Delete("Projects\\TrackingMaster\\ImportExport\\Frontline.edp7"); //MLHIDE
                    }
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\Attachments.zip")) //MLHIDE
                    {
                        File.Delete("Projects\\TrackingMaster\\ImportExport\\Attachments.zip"); //MLHIDE
                    }
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ExportActivityLog.csv")) //MLHIDE
                    {
                        File.Delete("Projects\\TrackingMaster\\ImportExport\\ExportActivityLog.csv"); //MLHIDE
                    }
                    zip.ExtractAll("");

                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\Projects.edp7")) //MLHIDE
                    {
                        System.IO.File.Move("Projects\\TrackingMaster\\ImportExport\\Projects.edp7", "Projects\\TrackingMaster\\DataImports\\Projects.edp7"); //MLHIDE
                    }
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ProjectsA.edp7")) //MLHIDE
                    {
                        System.IO.File.Move("Projects\\TrackingMaster\\ImportExport\\ProjectsA.edp7", "Projects\\TrackingMaster\\DataImports\\ProjectsA.edp7"); //MLHIDE
                    }
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ProjectsB.edp7")) //MLHIDE
                    {
                        System.IO.File.Move("Projects\\TrackingMaster\\ImportExport\\ProjectsB.edp7", "Projects\\TrackingMaster\\DataImports\\ProjectsB.edp7"); //MLHIDE
                    }
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\TraineeInformation.edp7")) //MLHIDE
                    {
                        System.IO.File.Move("Projects\\TrackingMaster\\ImportExport\\TraineeInformation.edp7", "Projects\\TrackingMaster\\DataImports\\TraineeInformation.edp7"); //MLHIDE
                    }
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ConsultantInfo.edp7")) //MLHIDE
                    {
                        System.IO.File.Move("Projects\\TrackingMaster\\ImportExport\\ConsultantInfo.edp7", "Projects\\TrackingMaster\\DataImports\\ConsultantInfo.edp7"); //MLHIDE
                    }
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\CodeFieldSites.edp7")) //MLHIDE
                    {
                        System.IO.File.Move("Projects\\TrackingMaster\\ImportExport\\CodeFieldSites.edp7", "Projects\\TrackingMaster\\DataImports\\CodeFieldSites.edp7"); //MLHIDE
                    }
                    if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\Frontline.edp7")) //MLHIDE
                    {
                        System.IO.File.Move("Projects\\TrackingMaster\\ImportExport\\Frontline.edp7", "Projects\\TrackingMaster\\DataImports\\Frontline.edp7"); //MLHIDE
                    }
                    //if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\Attachments.zip"))
                    //{
                    //    System.IO.File.Move("Projects\\TrackingMaster\\ImportExport\\Attachments.zip", "Projects\\TrackingMaster\\DataImports\\Attachments.zip");
                    //}
                    //if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ExportActivityLog.csv"))
                    //{
                    //    System.IO.File.Move("Projects\\TrackingMaster\\ImportExport\\ExportActivityLog.csv", "Projects\\TrackingMaster\\DataImports\\ExportActivityLog.csv");
                    //}
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\Projects.edp7")) //MLHIDE
                {

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;

                    Process process = new Process();
                    process.StartInfo.FileName = "DataUnpackager"; //MLHIDE
                    process.StartInfo.Arguments = "/package:Projects\\TrackingMaster\\DataImports\\Projects.edp7 /project:Projects\\TrackingMaster\\TrackingMasterNew.prj /view:Projects /autorun:true /minimized:true"; //MLHIDE
                    process.StartInfo.ErrorDialog = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //process.Exited += new EventHandler(myProcess_Exited);
                    process.Start();
                    process.WaitForExit();    // Wait up to five minutes.
                                              //process.Kill();
                                              //txtTransmit1.Text += "Residents  A D D E D...";
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\ProjectsA.edp7")) //MLHIDE
                {

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;

                    Process process = new Process();
                    process.StartInfo.FileName = "DataUnpackager"; //MLHIDE
                    process.StartInfo.Arguments = "/package:Projects\\TrackingMaster\\DataImports\\ProjectsA.edp7 /project:Projects\\TrackingMaster\\TrackingMasterNew.prj /view:Projects /autorun:true /minimized:true"; //MLHIDE
                    process.StartInfo.ErrorDialog = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //process.Exited += new EventHandler(myProcess_Exited);
                    process.Start();
                    process.WaitForExit();    // Wait up to five minutes.
                                              //process.Kill();
                                              //txtTransmit1.Text += "Residents  A D D E D...";
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\ProjectsB.edp7")) //MLHIDE
                {

                    ProcessStartInfo startInfoB = new ProcessStartInfo();
                    startInfoB.RedirectStandardOutput = true;
                    startInfoB.RedirectStandardError = true;
                    startInfoB.UseShellExecute = false;
                    startInfoB.CreateNoWindow = true;

                    Process processB = new Process();
                    processB.StartInfo.FileName = "DataUnpackager"; //MLHIDE
                    processB.StartInfo.Arguments = "/package:Projects\\TrackingMaster\\DataImports\\ProjectsB.edp7 /project:Projects\\TrackingMaster\\TrackingMasterNew.prj /view:Projects /autorun:true /minimized:true"; //MLHIDE
                    processB.StartInfo.ErrorDialog = true;
                    processB.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //process.Exited += new EventHandler(myProcess_Exited);
                    processB.Start();
                    processB.WaitForExit();    // Wait up to five minutes.
                                               //process.Kill();
                                               //txtTransmit1.Text += "Residents  A D D E D...";

                    //CallbackSetStatusMessage(String.Format(ml.ml_string(124, "Proyectos  AÑADIDO ..."), "test"));
                    CallbackSetStatusMessage(String.Format(ml.ml_string(144, "Projects A D D E D..."), "test"));


                    //this.Dispatcher.BeginInvoke(new SetProgressBarDelegate(SetProgressBar), 50);

                    //txtImportStatus.Text += "Projects A D D E D" + Environment.NewLine;
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\ConsultantInfo.edp7")) //MLHIDE
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "DataUnpackager"; //MLHIDE
                    process.StartInfo.Arguments = "/package:Projects\\TrackingMaster\\DataImports\\ConsultantInfo.edp7 /project:Projects\\TrackingMaster\\TrackingMasterNew.prj /view:ConsultantInfo /autorun:true /minimized:true"; //MLHIDE
                    process.StartInfo.ErrorDialog = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.CreateNoWindow = true;
                    //process.Exited += new EventHandler(myProcess_Exited);
                    process.Start();
                    process.WaitForExit();    // Wait up to five minutes.
                                              //process.Kill();
                                              //txtTransmit1.Text += "Residents  A D D E D...";

                    //this.Dispatcher.BeginInvoke(new SetProgressBarDelegate(SetProgressBar), 75);

                    //CallbackSetStatusMessage(String.Format(ml.ml_string(125, "Consultores AÑADIDO ..."), "test"));
                    CallbackSetStatusMessage(String.Format(ml.ml_string(180, "Consultants A D D E D..."), "test"));


                    //txtImportStatus.Text += "Consultants A D D E D" + Environment.NewLine;
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\TraineeInformation.edp7")) //MLHIDE
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "DataUnpackager"; //MLHIDE
                    process.StartInfo.Arguments = "/package:Projects\\TrackingMaster\\DataImports\\TraineeInformation.edp7 /project:Projects\\TrackingMaster\\TrackingMasterNew.prj /view:TraineeInformation /autorun:true /minimized:true"; //MLHIDE
                    process.StartInfo.ErrorDialog = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process.StartInfo.CreateNoWindow = true;
                    //process.Exited += new EventHandler(myProcess_Exited);
                    process.Start();
                    process.WaitForExit();    // Wait up to five minutes.
                                              //process.Kill();
                                              //txtTransmit1.Text += "Residents  A D D E D...";

                    //txtImportStatus.Text += "Trainees A D D E D" + Environment.NewLine;

                    //CallbackSetStatusMessage(String.Format(ml.ml_string(126, "Trainees A D D E D..."), "test"));
                    CallbackSetStatusMessage(String.Format(ml.ml_string(181, "Residents A D D E D..."), "test"));


                    //if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\" + packagePath.Name))
                    //{
                    //    File.Move("Projects\\TrackingMaster\\DataImports\\Projects.edp7", "Archive");
                    //}

                    UpdateSitesResidents = true;

                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\CodeFieldSites.edp7")) //MLHIDE
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "DataUnpackager"; //MLHIDE
                    process.StartInfo.Arguments = "/package:Projects\\TrackingMaster\\DataImports\\CodeFieldSites.edp7 /project:Projects\\TrackingMaster\\TrackingMasterNew.prj /view:CodeFieldSites /autorun:true /minimized:true"; //MLHIDE
                    process.StartInfo.ErrorDialog = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process.StartInfo.CreateNoWindow = true;
                    //process.Exited += new EventHandler(myProcess_Exited);
                    process.Start();
                    process.WaitForExit();    // Wait up to five minutes.
                                              //process.Kill();
                                              //txtTransmit1.Text += "Residents  A D D E D...";

                    //txtImportStatus.Text += "Trainees A D D E D" + Environment.NewLine;

                    //CallbackSetStatusMessage(String.Format(ml.ml_string(126, "Trainees A D D E D..."), "test"));
                    CallbackSetStatusMessage(ml.ml_string(192, "Field Sites A D D E D..."));

                    //if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\" + packagePath.Name))
                    //{
                    //    File.Move("Projects\\TrackingMaster\\DataImports\\Projects.edp7", "Archive");
                    //}

                    UpdateSitesResidents = true;

                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\Frontline.edp7")) //MLHIDE
                {
                    Process process2 = new Process();
                    process2.StartInfo.FileName = "DataUnpackager"; //MLHIDE
                    process2.StartInfo.Arguments = "/package:Projects\\TrackingMaster\\DataImports\\Frontline.edp7 /project:Projects\\TrackingMaster\\TrackingMasterNew.prj /view:Frontline /autorun:true /minimized:true"; //MLHIDE
                    process2.StartInfo.ErrorDialog = true;
                    process2.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process2.StartInfo.CreateNoWindow = true;
                    //process.Exited += new EventHandler(myProcess_Exited);
                    process2.Start();
                    process2.WaitForExit();    // Wait up to five minutes.
                                               //process.Kill();
                                               //txtTransmit1.Text += "Residents  A D D E D...";

                    //txtImportStatus.Text += "Trainees A D D E D" + Environment.NewLine;

                    //CallbackSetStatusMessage(String.Format(ml.ml_string(126, "Trainees A D D E D..."), "test"));
                    CallbackSetStatusMessage(ml.ml_string(204, "Frontline A D D E D..."));


                    //if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\" + packagePath.Name))
                    //{
                    //    File.Move("Projects\\TrackingMaster\\DataImports\\Projects.edp7", "Archive");
                    //}

                }


                if (permvar2 != null)
                {
                    SiteCodePerm_str = permvar2.ToString();
                }

                object permvar4 = EpiInfoMenuManager.GetPermanentVariableValue("PermInstOnly"); //MLHIDE

                if (permvar4 != null && permvar4.ToString() != "")
                {
                    InstOnly = permvar4.ToString();
                }

                //gfjnow

                //if (PermWhichInst_str = )
                //{

                LogActImport(DateTime.Now, PermWhichInst_str, SiteCodePerm_str, InstOnly);

                //}

                if (SiteCodePerm_str == "3" || (InstOnly == "1" && SiteCodePerm_str != "1" && SiteCodePerm_str != "3"))
                {

                    OleDbConnection connect = new OleDbConnection();
                    connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB"; //MLHIDE
                    connect.Open();

                    OleDbCommand command = new OleDbCommand();
                    command.Connection = connect;

                    if (System.IO.File.Exists(Environment.CurrentDirectory + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj")) //MLHIDE
                    {
                        Epi.Project project3 = new Epi.Project(Environment.CurrentDirectory + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj");  //MLHIDE //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

                        string tab = "TraineesFilter"; //MLHIDE
                        IDbDriver db3 = project3.CollectedData.GetDatabase();
                        //try
                        //{
                        var tabex = db3.TableExists(tab);

                        if (tabex == true)
                        {
                            command.CommandText = "Drop Table TraineesFilter"; //MLHIDE
                            command.ExecuteNonQuery();

                        }
                        //}
                        //catch
                        //{

                        //}
                    }
                    if (SiteCodePerm_str != "3" && InstOnly == "1")
                    {
                        command.CommandText = "SELECT Trainees8.ResidentName2, Trainees.FKEY INTO TraineesFilter FROM Trainees INNER JOIN Trainees8 ON Trainees.GlobalRecordId = Trainees8.GlobalRecordId WHERE((([PermWhichInst]) =[UniversityTrainingInstitution]));"; //MLHIDE
                        command.Parameters.AddWithValue("PermWhichInst", PermWhichInst_str); //MLHIDE
                    }
                    else
                    {
                        command.CommandText = "SELECT Trainees8.ResidentName2, Trainees.FKEY INTO TraineesFilter FROM Trainees INNER JOIN Trainees8 ON Trainees.GlobalRecordId = Trainees8.GlobalRecordId WHERE((([UserSettingResident]) =[ResidentName2]));"; //MLHIDE
                        command.Parameters.AddWithValue("UserSettingResident", UserSettingResident_str); //MLHIDE
                    }

                    command.ExecuteNonQuery();
                    command.CommandText = "DELETE DISTINCTROW Trainees.* FROM TraineesFilter RIGHT JOIN Trainees ON TraineesFilter.FKEY = Trainees.FKEY WHERE (((TraineesFilter.FKEY) Is Null));"; //MLHIDE
                    command.ExecuteNonQuery();
                    command.CommandText = "DELETE DISTINCTROW Trainees8.*, TraineesFilter.FKEY, Trainees8.ResidentName2 FROM TraineesFilter RIGHT JOIN(Trainees8 LEFT JOIN Trainees ON Trainees8.GlobalRecordId = Trainees.GlobalRecordId) ON TraineesFilter.FKEY = Trainees.FKEY WHERE(((TraineesFilter.FKEY)Is Null));"; //MLHIDE
                    command.ExecuteNonQuery();
                    command.CommandText = "DELETE DISTINCTROW Projects.*, [Trainees]![FKEY] AS Expr1 FROM Trainees RIGHT JOIN Projects ON Trainees.FKEY = Projects.GlobalRecordId WHERE ([Trainees]![FKEY]) is null;"; //MLHIDE
                    command.ExecuteNonQuery();
                    command.CommandText = "DELETE DISTINCTROW Projects4.*, Projects.UniqueKey FROM Projects RIGHT JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId WHERE (((Projects.UniqueKey) Is Null));"; //MLHIDE
                    command.ExecuteNonQuery();
                    command.CommandText = "DELETE DISTINCTROW Projects5.*, Projects.UniqueKey FROM Projects RIGHT JOIN Projects5 ON Projects.GlobalRecordId = Projects5.GlobalRecordId WHERE (((Projects.UniqueKey) Is Null));"; //MLHIDE
                    command.ExecuteNonQuery();
                    command.CommandText = "DELETE DISTINCTROW Projects6.*, Projects.UniqueKey FROM Projects RIGHT JOIN Projects6 ON Projects.GlobalRecordId = Projects6.GlobalRecordId WHERE(((Projects.UniqueKey)Is Null));"; //MLHIDE

                    command.ExecuteNonQuery();

                    connect.Close();

                    CallbackSetStatusMessage(String.Format(ml.ml_string(128, "Projects filtered for ") + UserSettingResident_str + "...", "test"));

                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\Projects.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\DataImports\\Projects.edp7"); //MLHIDE
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\ProjectsA.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\DataImports\\ProjectsA.edp7"); //MLHIDE
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\ProjectsB.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\DataImports\\ProjectsB.edp7"); //MLHIDE
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\TraineeInformation.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\DataImports\\TraineeInformation.edp7"); //MLHIDE
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\ConsultantInfo.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\DataImports\\ConsultantInfo.edp7"); //MLHIDE
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\CodeFieldSites.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\DataImports\\CodeFieldSites.edp7"); //MLHIDE
                }
                if (System.IO.File.Exists("Projects\\TrackingMaster\\DataImports\\Frontline.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\DataImports\\Frontline.edp7"); //MLHIDE
                }
                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\Attachments.zip")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\ImportExport\\Attachments.zip"); //MLHIDE
                }
                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ExportActivityLog.csv")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\ImportExport\\ExportActivityLog.csv"); //MLHIDE
                }



                //XmlDataUnpackager xmlUP = new Epi.ImportExport.ProjectPackagers.XmlDataUnpackager(settings.CaseForm, xmlDataPackage);
                ////xmlUP.StatusChanged += new UpdateStatusEventHandler(CallbackSetStatusMessage);
                ////xmlUP.UpdateProgress += new SetProgressBarDelegate(CallbackSetProgressBar);
                ////xmlUP.ResetProgress += new SimpleEventHandler(CallbackResetProgressBar);
                //xmlUP.MessageGenerated += new UpdateStatusEventHandler(CallbackSetStatusMessage);
                ////xmlUP.ImportFinished += new EventHandler(xmlUP_ImportFinished);
                //xmlUP.Append = true;
                //xmlUP.Update = true;
                //xmlUP.Unpackage();
                //importInfos.Add(xmlUP.ImportInfo);


                //FileInfo fi = new FileInfo(packagePath);

                string archiveFile = System.IO.Path.Combine(settings.ArchiveDirectory.FullName, packagePath.Name);


                //DirectoryInfo importDirectory = new DirectoryInfo(Environment.CurrentDirectory + "\\Projects\\TrackingMaster\\DataImports");

                //var files2 = importDirectory.GetFiles("ProjectTracking_*.*", SearchOption.TopDirectoryOnly);


                //foreach (FileInfo fi in files2)
                //{
                string file = packagePath.Name.ToString().Replace(@"ProjectTracking_", @"");  // "xls",@"" //MLHIDE
                file = file.Remove(file.IndexOf("_"));
                //file = file.ToString().Replace(@".cvs7", @"");
                //}
                //string siteCode = xmlDataPackage.ChildNodes[0].Attributes["SiteCode"].Value;
                //CallbackUpdateSiteStatus(siteCode);
                CallbackUpdateSiteStatus(file);
                //}

                if (!File.Exists(archiveFile))
                {
                    File.Move(fileName, archiveFile);
                }
                else
                {
                    File.Delete(fileName);
                }

                //CallbackSetStatusMessage(String.Format(ml.ml_string(145, "Archiving: ") + packagePath.Name + "\n ", "test"));
                CallbackSetStatusMessage(String.Format(ml.ml_string(182, "Archiving: ") + packagePath.Name + "\n ", "test"));
                //CallbackSetStatusMessage(String.Format("\n " + packagePath.Name, "test"));
                //+Environment.NewLine

                //string siteCode = xmlDataPackage.ChildNodes[0].Attributes["SiteCode"].Value;

                //FileInfo projectFileInfo = new FileInfo(Environment.CurrentDirectory + "\\Projects\\TrackingMaster\\DataImports");
                //DirectoryInfo projectDirectory = projectFileInfo.Directory;

                //var files = projectDirectory.GetFiles("ProjectTracking_*.*", SearchOption.TopDirectoryOnly);

                //}
                //catch (Exception ex)
                //{
                //    var st = new StackTrace(ex, true);

                //    // Get the bottom stack frame
                //    var frame = st.GetFrame(st.FrameCount - 1);
                //    // Get the line number from the stack frame
                //    var line = frame.GetFileLineNumber();
                //    var method = frame.GetMethod().ReflectedType.FullName;
                //    var path = frame.GetFileName();

                //    MessageBox.Show(ml.ml_string(137, "An error occurred exporting:") + "\n" + ex.Message + "\n" + "Line: " + line + "\n" + "Method: " + method + "\n" + "Path: " + path); //MLHIDE

                //    CallbackSetStatusMessage(ex.Message);
                //    //info.AddError(ex.Message, String.Empty);
                //    //e.Result = ex;
                //}
            }

            //var instance = new DataEntryPanel()
            //instance.dgRecords_Sub();

            return importInfos;
            //} //Gerald gfj0 must put back
            //AddNotificationStatusMessage(string.Format(ImportExportSharedStrings.START_BATCH_IMPORT, packagePaths.Count.ToString(), txtPackageFile.Text));
            //catch (Exception ex)
            //{
            //    var st = new StackTrace(ex, true);

            //    // Get the bottom stack frame
            //    var frame = st.GetFrame(st.FrameCount - 1);
            //    // Get the line number from the stack frame
            //    var line = frame.GetFileLineNumber();
            //    var method = frame.GetMethod().ReflectedType.FullName;
            //    var path = frame.GetFileName();

            //    MessageBox.Show(ml.ml_string(137, "An error occurred exporting:") + "\n" + ex.Message + "\n" + "Line: " + line + "\n" + "Method: " + method + "\n" + "Path: " + path); //MLHIDE

            //    MessageBox.Show("An error occurred trying to export \"{0}\":" + "\n" + ex.Message);
            //    return importInfos;
            //}
        }

        // gfj site update???
        private void CallbackUpdateSiteStatus(string code)
        {
            this.Dispatcher.BeginInvoke(new UpdateStatusEventHandler(UpdateSiteStatus), code);
        }

        private void UpdateSiteStatus(string code)
        {
            DataHelper.UpdateSite(code);
        }

        private void SetProgressBar(double value)
        {
            importProgressBar.Value = value;
            this.DataHelper.TaskbarProgressValue = value / 100;
        }

        private void SetStatusMessage(string message)
        {
            txtImportStatus.AppendText(DateTime.Now.ToString() + " | " + message + "\n");
        }

        private void CallbackSetProgressBar(double value)
        {
            this.Dispatcher.BeginInvoke(new SetProgressBarDelegate(SetProgressBar), value);
        }

        private void CallbackSetStatusMessage(string message)
        {
            this.Dispatcher.BeginInvoke(new UpdateStatusEventHandler(SetStatusMessage), message);
        }

        private void UpdateImportInfo(List<ImportInfo> infos)
        {
            Properties.Settings.Default.UserLastMerged = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            Properties.Settings.Default.DateLastMerged = DateTime.Now;
            Properties.Settings.Default.PackagesLastMerged = infos.Count;

            int total = 0;

            foreach (ImportInfo info in infos)
            {
                total = total + info.TotalRecordsAppended;
            }

            Properties.Settings.Default.RecordsLastMerged = total;
            Properties.Settings.Default.Save();

            importProgressBar.Value = 0;
            importProgressBar.IsIndeterminate = false;
            btnImport.IsEnabled = true;

            SetStatusMessage(ml.ml_string(129, "Import successfully completed."));

            if (UpdateSitesResidents == true)
            {
                DataHelper.FillResidents_Sub();
                UpdateSitesResidents = false;
            }

            // gfj0 12/14/2020


            var permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm"); //MLHIDE

            string UserSettingPERM = null;

            if (permvar2 != null)
            {
                if (permvar2.ToString() == "3")
                {
                    UserSettingPERM = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident").ToString();  //MLHIDE
                }
                else
                {
                    UserSettingPERM = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC").ToString(); //MLHIDE
                }
            }

            OleDbConnection connect = new OleDbConnection();

            connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB";   //MLHIDE

            connect.Open(); // ATTACHMENT 

            OleDbCommand command = new OleDbCommand();

            command.Connection = connect;

            //if (SiteCodePerm_str != "3" && InstOnly == "1")
            //{
            command.CommandText = "UPDATE Projects4 INNER JOIN(Projects INNER JOIN ((Requiredwrittenmaterials INNER JOIN Requiredwrittenmaterials13 ON Requiredwrittenmaterials.GlobalRecordId = Requiredwrittenmaterials13.GlobalRecordId) INNER JOIN ActivityLog ON Requiredwrittenmaterials13.AttachGUID = ActivityLog.GUID) ON Projects.GlobalRecordId = Requiredwrittenmaterials.FKEY) ON Projects4.GlobalRecordId = Projects.GlobalRecordId SET Projects4.HasAttachment = 'Yes'";   //MLHIDE

            //command.CommandText = "UPDATE Projects4 INNER JOIN(Projects INNER JOIN ((Requiredwrittenmaterials INNER JOIN Requiredwrittenmaterials13 ON Requiredwrittenmaterials.GlobalRecordId = Requiredwrittenmaterials13.GlobalRecordId) INNER JOIN ActivityLog ON Requiredwrittenmaterials13.AttachGUID = ActivityLog.GUID) ON Projects.GlobalRecordId = Requiredwrittenmaterials.FKEY) ON Projects4.GlobalRecordId = Projects.GlobalRecordId SET Projects4.HasAttachment = 'Yes' WHERE ((ActivityLog.CreatedBy) <> '" + UserSettingPERM + "') OR (((ActivityLog.CreatedLevel)= 3 And " + permvar2 + "<> 3)) OR (((ActivityLog.CreatedLevel)= 2 And " + permvar2 + "<> 3))";   //MLHIDE

            command.ExecuteNonQuery();
            //}

            connect.Close();

            DataHelper.RepopulateCollections(); // gfj0 this where the refresh is!!!

            // Content="{x:Static ml:MultiLang._53}"
            ((ConsultantPanel)Application.Current.Windows[1].FindName("panelConsultant")).dgRecordsConsultants_Sub(); //MLHIDE

            ((ResidentPanel)Application.Current.Windows[1].FindName("panelResident")).dgRecordsResidents_Sub(); //MLHIDE

        }

        internal void ResetHeight()
        {
            dgSites.Visibility = System.Windows.Visibility.Collapsed;

            grdHome.UpdateLayout();

            double maxHeight = grdHome.ActualHeight;
            maxHeight = maxHeight - 100;

            if (maxHeight <= 0) maxHeight = 0;

            dgSites.MaxHeight = maxHeight;
            dgSites.Height = maxHeight;
            dgSites.Visibility = System.Windows.Visibility.Visible;

            txtImportStatus.Height = Math.Min(360, Math.Max(40, grdHome.ActualHeight - 310));
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetHeight();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // only show the site merge status for the moh gfj

            permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm"); //MLHIDE

            string SiteCodePerm_str = "3";

            if (permvar2 != null && permvar2.ToString() != "")
            {
                SiteCodePerm_str = permvar2.ToString();
            }

            if (SiteCodePerm_str == "3")
            {
                panelSiteMergeStatus.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                panelSiteMergeStatus.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void btnOpenModeEmploi_Click(object sender, RoutedEventArgs e)
        {

            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            string fileName = "\\Resources\\ImportUserManual.pdf"; //MLHIDE

            string fullFileName = System.IO.Path.GetDirectoryName(a.Location) + fileName;
            if (System.IO.File.Exists(fullFileName))
            {
                string commandText = fullFileName;// System.IO.Path.GetDirectoryName(a.Location) + fileName; //"\\Projects\\Menafrinet\\Fusion Mode d Emploi.pdf";

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = commandText;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            else
            {
                MessageBox.Show("Please contact the application developer if there is an error.");
            }

            //System.Diagnostics.Process.Start("AcroRd32.exe", "Fusion Mode d Emploi.pdf");
        }

        public void LogActImport(DateTime ReceivedDate, string PermWhichInst_str, string SiteCodePerm_str, string InstOnly)
        {
            try
            {
                string mySelectQuery = "SELECT * FROM ExportActivityLog.csv"; //MLHIDE
                OleDbConnection myConnection = new OleDbConnection
                        ("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\ImportExport\\; " + "Extended Properties=\"text;HDR=NO;FMT=Delimited\""); //MLHIDE
                OleDbDataAdapter dsCmd = new OleDbDataAdapter(mySelectQuery, myConnection);

                DataTable dt = new DataTable();

                //Fill the DataSet object gfj0
                if (dsCmd.Equals(null))
                {
                    return;
                }

                try
                {
                    dsCmd.Fill(dt); //MLHIDE
                                    //Create a XML document with the table data
                                    //int cnt;
                }
                catch
                {
                    return;
                }


                foreach (DataRow ActRow in dt.Rows)
                {
                    string WhichInst = null;
                    string Institution = null;
                    string InstitutionTXT = null;
                    string ResidentAttchedby = null;
                    string ResidentAttchedbyTXT = null;
                    string ResidentAttchedbyLBL = null;

                    string LogGUID = "'" + ActRow.ItemArray[0].ToString() + "', ";
                    string LogGUIDTXT = ActRow.ItemArray[0].ToString();
                    string fileName = "'" + ActRow.ItemArray[1].ToString().Replace("|", ",") + "', ";
                    string fileNameTXT = ActRow.ItemArray[1].ToString().Replace("|", ",") + "', ";

                    if (!String.IsNullOrEmpty(ActRow.ItemArray[26].ToString()))
                    {
                        WhichInst = ActRow.ItemArray[26].ToString().Replace("|", ",");
                        Institution = "'" + ActRow.ItemArray[26].ToString().Replace("|", ",") + "', ";
                        InstitutionTXT = "Institution, "; //MLHIDE
                    }

                    string CreatedBy_str = ActRow.ItemArray[2].ToString().Replace("|", ",");
                    string CreatedBy = "'" + ActRow.ItemArray[2].ToString().Replace("|", ",") + "', ";

                    if (!String.IsNullOrEmpty(ActRow.ItemArray[37].ToString()))
                    {
                        ResidentAttchedbyLBL = ActRow.ItemArray[37].ToString().Replace("|", ",");
                        ResidentAttchedby = "'" + ActRow.ItemArray[37].ToString().Replace("|", ",") + "', ";
                        ResidentAttchedbyTXT = "ResidentAttchedby, "; //MLHIDE
                    }


                    OleDbConnection ActConnect = new OleDbConnection();

                    ActConnect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB";   //MLHIDE

                    ActConnect.Open();

                    OleDbCommand command = new OleDbCommand();

                    command.Connection = ActConnect;

                    //command.CommandText = "SELECT DISTINCT ActivityLog.GUID FROM ActivityLog INNER JOIN (Requiredwrittenmaterials13 INNER JOIN ((Projects INNER JOIN (Trainees8 INNER JOIN Trainees ON Trainees8.GlobalRecordId = Trainees.GlobalRecordId) ON Projects.GlobalRecordId = Trainees.FKEY) INNER JOIN Requiredwrittenmaterials ON Projects.GlobalRecordId = Requiredwrittenmaterials.FKEY) ON Requiredwrittenmaterials13.GlobalRecordId = Requiredwrittenmaterials.GlobalRecordId) ON ActivityLog.GUID = Requiredwrittenmaterials13.AttachGUID GROUP BY ActivityLog.GUID HAVING (((ActivityLog.GUID)='" + LogGUIDTXT + "'));"; //MLHIDE

                    //command.CommandText = "SELECT DISTINCT ActivityLog.GUID, Trainees8.ResidentName2 FROM ActivityLog INNER JOIN(Requiredwrittenmaterials13 INNER JOIN((Projects INNER JOIN(Trainees8 INNER JOIN Trainees ON Trainees8.GlobalRecordId = Trainees.GlobalRecordId) ON Projects.GlobalRecordId = Trainees.FKEY) INNER JOIN Requiredwrittenmaterials ON Projects.GlobalRecordId = Requiredwrittenmaterials.FKEY) ON Requiredwrittenmaterials13.GlobalRecordId = Requiredwrittenmaterials.GlobalRecordId) ON ActivityLog.GUID = Requiredwrittenmaterials13.AttachGUID GROUP BY ActivityLog.GUID, Trainees8.ResidentName2 HAVING (((ActivityLog.GUID)='" + LogGUIDTXT + "') AND ((Trainees8.ResidentName2) ='" + UserSettingResident_str + "'));"; //MLHIDE

                    string GroupAttachmentCHK = null;
                    string GroupAttachment = null;
                    string GroupAttachmentTXT = null;

                    if (!String.IsNullOrEmpty(ActRow.ItemArray[36].ToString()))
                    {
                        GroupAttachmentCHK = ActRow.ItemArray[36].ToString();
                        GroupAttachment = "'" + ActRow.ItemArray[36].ToString() + "', ";
                        GroupAttachmentTXT = "GroupAttachment, "; //MLHIDE
                    }

                    object chkTrainee = null;

                    bool isTrue = false;

                    if (GroupAttachmentCHK == "Yes" && SiteCodePerm_str == "3") //MLHIDE
                    {

                        command.CommandText = "SELECT Trainees8.ResidentName2 FROM((Projects INNER JOIN (Requiredwrittenmaterials INNER JOIN Requiredwrittenmaterials13 ON Requiredwrittenmaterials.GlobalRecordId = Requiredwrittenmaterials13.GlobalRecordId) ON Projects.GlobalRecordId = Requiredwrittenmaterials.FKEY) INNER JOIN Trainees ON Projects.GlobalRecordId = Trainees.FKEY) INNER JOIN Trainees8 ON Trainees.GlobalRecordId = Trainees8.GlobalRecordId WHERE(((Requiredwrittenmaterials13.AttachGUID) = '" + LogGUIDTXT + "') AND((Trainees8.ResidentName2) ='" + UserSettingResident_str + "'));"; //MLHIDE

                        //command.CommandText = "SELECT DISTINCT ActivityLog.GUID FROM ActivityLog INNER JOIN(Requiredwrittenmaterials13 INNER JOIN((Projects INNER JOIN(Trainees8 INNER JOIN Trainees ON Trainees8.GlobalRecordId = Trainees.GlobalRecordId) ON Projects.GlobalRecordId = Trainees.FKEY) INNER JOIN Requiredwrittenmaterials ON Projects.GlobalRecordId = Requiredwrittenmaterials.FKEY) ON Requiredwrittenmaterials13.GlobalRecordId = Requiredwrittenmaterials.GlobalRecordId) ON ActivityLog.GUID = Requiredwrittenmaterials13.AttachGUID GROUP BY ActivityLog.GUID, Trainees8.ResidentName2 HAVING(((ActivityLog.GUID)= '" + LogGUIDTXT + "') AND((Trainees8.ResidentName2) ='" + UserSettingResident_str + "'));"; //MLHIDE

                        chkTrainee = command.ExecuteScalar();
                    }

                    if (chkTrainee != null)
                    {
                        isTrue = true;
                    }

                    ActConnect.Close();

                    // Skip first row which in this case is a header with column names
                    if ((SiteCodePerm_str == "1") || (SiteCodePerm_str == "2" && InstOnly == "0") || (SiteCodePerm_str == "2" && WhichInst == PermWhichInst_str && InstOnly == "1") || ((SiteCodePerm_str == "3" && (ResidentAttchedbyLBL == UserSettingResident_str || isTrue == true)))) //MLHIDE
                    {
                        string DateLastSent = null;
                        string DateLastSentTXT = null;
                        string CreatedDate = null;
                        string CreatedDateTXT = null;
                        string LastSentBy = null;
                        string LastSentByTXT = null;

                        string CreatedLevel = null;
                        string CreatedLevelTXT = null;
                        string SentResident = null;
                        string SentResidentTXT = null;
                        string SentByResident = null;
                        string SentByResidentTXT = null;
                        string SentDateResident = null;
                        string SentDateResidentTXT = null;

                        string SentPC = null;
                        string SentPCTXT = null;
                        string SentByPC = null;
                        string SentByPCTXT = null;
                        string SentDatePC = null;
                        string SentDatePCTXT = null;

                        string SentNational = null;
                        string SentNationalTXT = null;
                        string SentByNational = null;
                        string SentByNationalTXT = null;
                        string SentDateNational = null;
                        string SentDateNationalTXT = null;

                        string ReceivedResident = null;
                        string ReceivedResidentTXT = null;
                        string ReceivedByResident = null;
                        string ReceivedByResidentTXT = null;
                        string ReceivedDateResident = null;
                        string ReceivedDateResidentTXT = null;

                        string ReceivedPC = null;
                        string ReceivedPCTXT = null;
                        string ReceivedByPC = null;
                        string ReceivedByPCTXT = null;
                        string ReceivedDatePC = null;
                        string ReceivedDatePCTXT = null;

                        string ReceivedNational = null;
                        string ReceivedNationalTXT = null;
                        string ReceivedByNational = null;
                        string ReceivedByNationalTXT = null;
                        string ReceivedDateNational = null;
                        string ReceivedDateNationalTXT = null;

                        string DeletedResident = null;
                        string DeletedResidentTXT = null;
                        string DeletedByResident = null;
                        string DeletedByResidentTXT = null;
                        string DeletedDateResident = null;
                        string DeletedDateResidentTXT = null;

                        string DeletedPC = null;
                        string DeletedPCTXT = null;
                        string DeletedByPC = null;
                        string DeletedByPCTXT = null;
                        string DeletedDatePC = null;
                        string DeletedDatePCTXT = null;


                        string DeletedNational = null;
                        string DeletedNationalTXT = null;
                        string DeletedByNational = null;
                        string DeletedByNationalTXT = null;
                        string DeletedDateNational = null;
                        string DeletedDateNationalTXT = null;

                        string ActivityComments = null;
                        string ActivityCommentsTXT = null;

                        string ReceivedBy = UserSettingResident_str;
                        DateTime ReceivedDate2 = DateTime.Now;

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[3].ToString()))
                        {
                            CreatedDate = "#" + ActRow.ItemArray[3].ToString() + "#, ";
                            CreatedDateTXT = "CreatedDate, "; //MLHIDE
                        }


                        if (!String.IsNullOrEmpty(ActRow.ItemArray[4].ToString()) && ActRow.ItemArray[4].ToString() != "0")
                        {
                            CreatedLevel = ActRow.ItemArray[4].ToString() + ", ";
                            CreatedLevelTXT = "CreatedLevel , "; //MLHIDE
                        }


                        if (!String.IsNullOrEmpty(ActRow.ItemArray[5].ToString()) && ActRow.ItemArray[5].ToString() != "0")
                        {
                            SentResident = ActRow.ItemArray[5].ToString() + ", ";
                            SentResidentTXT = "SentResident, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[6].ToString()))
                        {
                            SentByResident = "'" + ActRow.ItemArray[6].ToString().Replace("|", ",") + "', ";
                            SentByResidentTXT = "SentByResident, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[7].ToString()))
                        {
                            SentDateResident = "#" + ActRow.ItemArray[7].ToString() + "#, ";
                            SentDateResidentTXT = "SentDateResident, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[8].ToString()) && ActRow.ItemArray[8].ToString() != "0")
                        {
                            SentPC = ActRow.ItemArray[8].ToString() + ", ";
                            SentPCTXT = "SentPC, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[9].ToString()))
                        {
                            SentByPC = "'" + ActRow.ItemArray[9].ToString().Replace("|", ",") + "', ";
                            SentByPCTXT = "SentByPC, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[10].ToString()))
                        {
                            SentDatePC = "#" + ActRow.ItemArray[10].ToString() + "#, ";
                            SentDatePCTXT = "SentDatePC, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[11].ToString()) && ActRow.ItemArray[11].ToString() != "0")
                        {
                            SentNational = ActRow.ItemArray[11].ToString() + ", ";
                            SentNationalTXT = "SentNational, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[12].ToString()))
                        {
                            SentByNational = "'" + ActRow.ItemArray[12].ToString().Replace("|", ",") + "', ";
                            SentByNationalTXT = "SentByNational, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[13].ToString()))
                        {
                            SentDateNational = "#" + ActRow.ItemArray[13].ToString() + "#, ";
                            SentDateNationalTXT = "SentDateNational, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[14].ToString()))
                        {
                            LastSentBy = "'" + ActRow.ItemArray[14].ToString().ToString().Replace("|", ",") + "', ";
                            LastSentByTXT = "LastSentBy, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[15].ToString()))
                        {
                            DateLastSent = "#" + ActRow.ItemArray[15].ToString() + "#, ";
                            DateLastSentTXT = "DateLastSent, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[16].ToString()) && ActRow.ItemArray[16].ToString() != "0")
                        {
                            ReceivedResident = ActRow.ItemArray[16].ToString() + ", ";
                            ReceivedResidentTXT = "ReceivedResident, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[17].ToString()))
                        {
                            ReceivedByResident = "'" + ActRow.ItemArray[17].ToString().Replace("|", ",") + "', ";
                            ReceivedByResidentTXT = "ReceivedByResident, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[18].ToString()))
                        {
                            ReceivedDateResident = "#" + ActRow.ItemArray[18].ToString() + "#, ";
                            ReceivedDateResidentTXT = "ReceivedDateResident, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[19].ToString()) && ActRow.ItemArray[19].ToString() != "0")
                        {
                            ReceivedPC = ActRow.ItemArray[19].ToString() + ", ";
                            ReceivedPCTXT = "ReceivedPC, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[20].ToString()))
                        {
                            ReceivedByPC = "'" + ActRow.ItemArray[20].ToString().Replace("|", ",") + "', ";
                            ReceivedByPCTXT = "ReceivedByPC, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[21].ToString()))
                        {
                            ReceivedDatePC = "#" + ActRow.ItemArray[21].ToString() + "#, ";
                            ReceivedDatePCTXT = "ReceivedDatePC, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[22].ToString()) && ActRow.ItemArray[22].ToString() != "0")
                        {
                            ReceivedNational = ActRow.ItemArray[22].ToString() + ", ";
                            ReceivedNationalTXT = "ReceivedNational, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[23].ToString()))
                        {
                            ReceivedByNational = "'" + ActRow.ItemArray[23].ToString().Replace("|", ",") + "', ";
                            ReceivedByNationalTXT = "ReceivedByNational, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[24].ToString()))
                        {
                            ReceivedDateNational = "#" + ActRow.ItemArray[24].ToString() + "#, ";
                            ReceivedDateNationalTXT = "ReceivedDateNational, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[27].ToString()))
                        {
                            DeletedResident = ActRow.ItemArray[27].ToString() + ", ";
                            DeletedResidentTXT = "DeletedResident, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[28].ToString()))
                        {
                            DeletedByResident = "'" + ActRow.ItemArray[28].ToString().Replace("|", ",") + "', ";
                            DeletedByResidentTXT = "DeletedByResident, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[29].ToString()))
                        {
                            DeletedDateResident = "#" + ActRow.ItemArray[29].ToString() + "#, ";
                            DeletedDateResidentTXT = "DeletedDateResident, "; //MLHIDE
                        }
                        if (!String.IsNullOrEmpty(ActRow.ItemArray[30].ToString()))
                        {
                            DeletedPC = ActRow.ItemArray[30].ToString() + ", ";
                            DeletedPCTXT = "DeletedPC, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[31].ToString()))
                        {
                            DeletedByPC = "'" + ActRow.ItemArray[31].ToString().Replace("|", ",") + "', ";
                            DeletedByPCTXT = "ReceivedByPC, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[32].ToString()))
                        {
                            DeletedDatePC = "#" + ActRow.ItemArray[32].ToString() + "#, ";
                            DeletedDatePCTXT = "DeletedDatePC, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[33].ToString()))
                        {
                            DeletedNational = ActRow.ItemArray[33].ToString() + ", ";
                            DeletedNationalTXT = "DeletedNational, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[34].ToString()))
                        {
                            DeletedByNational = "'" + ActRow.ItemArray[34].ToString().Replace("|", ",") + "', ";
                            DeletedByNationalTXT = "DeletedByNational, "; //MLHIDE
                        }

                        if (!String.IsNullOrEmpty(ActRow.ItemArray[35].ToString()))
                        {
                            DeletedDateNational = "#" + ActRow.ItemArray[35].ToString() + "#, ";
                            DeletedDateNationalTXT = "DeletedDateNational, "; //MLHIDE
                        }

                        if (permvar2.ToString() == "1") //MLHIDE
                        {
                            if (ActRow.ItemArray[22].ToString() == "0")
                            {
                                ReceivedNational = "1" + ",";
                                ReceivedNationalTXT = "ReceivedNational, "; //MLHIDE
                                ReceivedByNational = "'" + UserSettingResident_str + "', ";
                                ReceivedByNationalTXT = "ReceivedByNational, "; //MLHIDE
                                ReceivedDateNational = "#" + DateTime.Now.ToString() + "#, ";
                                ReceivedDateNationalTXT = "ReceivedDateNational, "; //MLHIDE
                            }
                        }

                        if (permvar2.ToString() == "2") //MLHIDE
                        {
                            if (ActRow.ItemArray[19].ToString() == "0")
                            {
                                ReceivedPC = "1" + ",";
                                ReceivedPCTXT = "ReceivedPC, "; //MLHIDE
                                ReceivedByPC = "'" + UserSettingResident_str + "', ";
                                ReceivedByPCTXT = "ReceivedByPC, "; //MLHIDE
                                ReceivedDatePC = "#" + DateTime.Now.ToString() + "#, ";
                                ReceivedDatePCTXT = "ReceivedDatePC, "; //MLHIDE
                            }
                        }

                        if (permvar2.ToString() == "3") //MLHIDE
                        {
                            if (ActRow.ItemArray[16].ToString() == "0")
                            {
                                ReceivedResident = "1" + ", ";
                                ReceivedResidentTXT = "ReceivedResident, "; //MLHIDE
                                ReceivedByResident = "'" + UserSettingResident_str + "', ";
                                ReceivedByResidentTXT = "ReceivedByResident, "; //MLHIDE
                                ReceivedDateResident = "#" + DateTime.Now.ToString() + "#, ";
                                ReceivedDateResidentTXT = "ReceivedDateResident, "; //MLHIDE
                            }
                        }

                        //if (!String.IsNullOrEmpty(ActRow.ItemArray[25].ToString()))
                        //{
                        ActivityComments = "'" + ActRow.ItemArray[24].ToString().Replace("|", ",") + "'";
                        ActivityCommentsTXT = "ActivityComments"; //MLHIDE
                                                                  //}

                        //OleDbConnection ActConnect = new OleDbConnection();

                        //ActConnect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB";   //MLHIDE

                        ActConnect.Open();

                        //OleDbCommand command = new OleDbCommand();

                        command.Connection = ActConnect;

                        command.CommandText = "SELECT count(*) from ActivityLog WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE

                        var count = command.ExecuteScalar();

                        if (Convert.ToInt32(count) == 0)
                        {

                            if (permvar2.ToString() == "1") //MLHIDE
                            {

                            }
                            if (permvar2.ToString() == "2") //MLHIDE
                            {

                            }
                            if (permvar2.ToString() == "3") //MLHIDE
                            {

                            }

                            command.CommandText = "INSERT INTO ActivityLog ([GUID], FileName, CreatedBy, " + CreatedLevelTXT + CreatedDateTXT + SentResidentTXT + SentByResidentTXT + SentDateResidentTXT + SentPCTXT + SentByPCTXT + SentDatePCTXT + SentNationalTXT + SentByNationalTXT + SentDateNationalTXT + LastSentByTXT + DateLastSentTXT + ReceivedResidentTXT + ReceivedByResidentTXT + ReceivedDateResidentTXT + ReceivedPCTXT + ReceivedByPCTXT + ReceivedDatePCTXT + ReceivedNationalTXT + ReceivedByNationalTXT + ReceivedDateNationalTXT + InstitutionTXT + DeletedResidentTXT + DeletedByResidentTXT + DeletedDateResidentTXT + DeletedPCTXT + DeletedByPCTXT + DeletedDatePCTXT + DeletedNationalTXT + DeletedByNationalTXT + DeletedDateNationalTXT + GroupAttachmentTXT + ResidentAttchedbyTXT + ActivityCommentsTXT + ")  VALUES (" + LogGUID + fileName + CreatedBy + CreatedLevel + CreatedDate + SentResident + SentByResident + SentDateResident + SentPC + SentByPC + SentDatePC + SentNational + SentByNational + SentDateNational + LastSentBy + DateLastSent + ReceivedResident + ReceivedByResident + ReceivedDateResident + ReceivedPC + ReceivedByPC + ReceivedDatePC + ReceivedNational + ReceivedByNational + ReceivedDateNational + Institution + DeletedResident + DeletedByResident + DeletedDateResident + DeletedPC + DeletedByPC + DeletedDatePC + DeletedNational + DeletedByNational + DeletedDateNational + GroupAttachment + ResidentAttchedby + ActivityComments + ");"; //MLHIDE
                            command.ExecuteNonQuery();
                            ActConnect.Close();

                        }
                        else
                        { // Last check
                            command.CommandText = "SELECT ActivityLog.ReceivedResident FROM ActivityLog WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE

                            var ResRec = command.ExecuteScalar();

                            command.CommandText = "SELECT ActivityLog.ReceivedPC FROM ActivityLog WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE

                            var PCRec = command.ExecuteScalar();

                            command.CommandText = "SELECT ActivityLog.ReceivedNational FROM ActivityLog WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE

                            var NatRec = command.ExecuteScalar();

                            command.CommandText = "SELECT ActivityLog.SentResident FROM ActivityLog WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE

                            var LogSentRes = command.ExecuteScalar();

                            command.CommandText = "SELECT ActivityLog.SentPC FROM ActivityLog WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE

                            var LogSentPC = command.ExecuteScalar();

                            command.CommandText = "SELECT ActivityLog.SentNational FROM ActivityLog WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE

                            var LogSentNat = command.ExecuteScalar();

                            //if (ResUpdate == "0") //MLHIDE
                            //{
                            //    //if (ActRow.ItemArray[5].ToString() == "1")
                            //    //{
                            //    command.CommandText = "UPDATE ActivityLog SET ActivityLog.ReceivedResident = 1, ActivityLog.ReceivedByResident = '" + UserSettingResident_str + "', ReceivedDateResident = #" + DateTime.Now.ToString() + "# WHERE(((ActivityLog.GUID) ='" + (ActRow.ItemArray[0].ToString()) + "') AND ((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND ((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                            //    //}
                            //}

                            //if (permvar2.ToString() == "2") //MLHIDE
                            //{

                            if (ActRow.ItemArray[5].ToString() != "0" && LogSentRes.ToString() == "0")
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.SentResident = " + ActRow.ItemArray[5].ToString() + ", ActivityLog.SentByResident = '" + ActRow.ItemArray[6].ToString() + "', ActivityLog.SentDateResident = #" + ActRow.ItemArray[7].ToString() + "# WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }

                            if (ActRow.ItemArray[8].ToString() != "0" && LogSentPC.ToString() == "0")
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.SentPC = " + ActRow.ItemArray[8].ToString() + ", ActivityLog.SentByPC = '" + ActRow.ItemArray[9].ToString() + "', ActivityLog.SentDatePC = #" + ActRow.ItemArray[10].ToString() + "# WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }

                            if (ActRow.ItemArray[11].ToString() != "0" && LogSentNat.ToString() == "0")
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.SentNational = " + ActRow.ItemArray[11].ToString() + ", ActivityLog.SentByNational = '" + ActRow.ItemArray[12].ToString() + "', ActivityLog.SentDateNational = #" + ActRow.ItemArray[13].ToString() + "# WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }

                            if (!String.IsNullOrEmpty(ActRow.ItemArray[16].ToString()) && ActRow.ItemArray[16].ToString() != "0" && ResRec.ToString() == "0") //MLHIDE
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.ReceivedResident = " + ActRow.ItemArray[16].ToString() + ", ActivityLog.ReceivedByResident = '" + ActRow.ItemArray[17].ToString() + "', ReceivedDateResident = #" + ActRow.ItemArray[18].ToString() + "# WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }

                            if (permvar2.ToString() == "3" && ActRow.ItemArray[5].ToString() != "0" && ResRec.ToString() == "0") //MLHIDE
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.ReceivedResident = 1, ActivityLog.ReceivedByResident = '" + UserSettingResident_str + "', ReceivedDateResident = #" + DateTime.Now.ToString() + "# WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND ((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }

                            if (!String.IsNullOrEmpty(ActRow.ItemArray[19].ToString()) && ActRow.ItemArray[19].ToString() != "0" && PCRec.ToString() == "0")
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.ReceivedPC = " + ActRow.ItemArray[19].ToString() + ", ActivityLog.ReceivedByPC = '" + ActRow.ItemArray[20].ToString() + "', ReceivedDatePC = #" + ActRow.ItemArray[21].ToString() + "# WHERE(((ActivityLog.GUID) ='" + (ActRow.ItemArray[0].ToString()) + "') AND ((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND ((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }

                            if (permvar2.ToString() == "2" && ActRow.ItemArray[8].ToString() != "0" && PCRec.ToString() == "0") //MLHIDE
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.ReceivedPC = 1, ActivityLog.ReceivedByPC = '" + UserSettingResident_str + "', ReceivedDatePC = #" + DateTime.Now.ToString() + "# WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND ((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }

                            if (!String.IsNullOrEmpty(ActRow.ItemArray[22].ToString()) && ActRow.ItemArray[22].ToString() != "0" && NatRec.ToString() == "0")
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.ReceivedNational = " + ActRow.ItemArray[22].ToString() + ", ActivityLog.ReceivedByNational = '" + ActRow.ItemArray[23].ToString() + "', ReceivedDateNational = #" + ActRow.ItemArray[24].ToString() + "# WHERE(((ActivityLog.GUID) ='" + (ActRow.ItemArray[0].ToString()) + "') AND ((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND ((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }

                            if (UserSettingResident_str == "1" && ActRow.ItemArray[11].ToString() != "0" && NatRec.ToString() == "0") //MLHIDE
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.ReceivedNational = 1, ActivityLog.ReceivedByNational = '" + UserSettingResident_str + "', ReceivedDateNational = #" + DateTime.Now.ToString() + "# WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND ((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }

                            command.CommandText = "SELECT ActivityLog.DeletedResident FROM ActivityLog WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE

                            var DelRes = command.ExecuteScalar();

                            if (ActRow.ItemArray[27].ToString() != "0" && DelRes.ToString() == "0") //MLHIDE
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.DeletedResident = 1, ActivityLog.DeletedByResident = '" + ActRow.ItemArray[28].ToString() + "', DeletedDateResident = #" + ActRow.ItemArray[29].ToString() + "# WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND ((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }

                            command.CommandText = "SELECT ActivityLog.DeletedPC FROM ActivityLog WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE

                            var DelPC = command.ExecuteScalar();

                            if (ActRow.ItemArray[30].ToString() != "0" && DelPC.ToString() == "0") //MLHIDE
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.DeletedPC = 1, ActivityLog.DeletedByPC = '" + ActRow.ItemArray[31].ToString() + "', DeletedDatePC = #" + ActRow.ItemArray[32].ToString() + "# WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND ((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }

                            command.CommandText = "SELECT ActivityLog.DeletedNational FROM ActivityLog WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE

                            var DelNat = command.ExecuteScalar();

                            if (ActRow.ItemArray[33].ToString() != "0" && DelNat.ToString() == "0") //MLHIDE
                            {
                                command.CommandText = "UPDATE ActivityLog SET ActivityLog.DeletedNational = 1, ActivityLog.DeletedByNational = '" + ActRow.ItemArray[34].ToString() + "', DeletedDateNational = #" + ActRow.ItemArray[35].ToString() + "# WHERE(((ActivityLog.GUID) = '" + (ActRow.ItemArray[0].ToString()) + "') AND((ActivityLog.FileName) = '" + (ActRow.ItemArray[1].ToString()) + "') AND ((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                                command.ExecuteNonQuery();
                            }
                            //if (permvar2.ToString() == "1") //MLHIDE
                            //{
                            //    //if (ReceivedResident == "0")
                            //    //{
                            //    command.CommandText = "UPDATE ActivityLog SET ActivityLog.ReceivedNational = 1, ActivityLog.ReceivedByNational = '" + UserSettingResident_str + "', ReceivedDateNational = #" + DateTime.Now.ToString() + "# WHERE(((ActivityLog.GUID) =[AttachGUID]) AND ((ActivityLog.FileName) = [fileName]) AND ((ActivityLog.CreatedDate) = #" + (ActRow.ItemArray[3].ToString()) + "#));"; //MLHIDE
                            //    //}
                        }

                        //command.ExecuteNonQuery();

                        UnzipAttachments(ActRow.ItemArray[0].ToString(), (ActRow.ItemArray[1].ToString()));

                        ActConnect.Close();

                    }

                    //cnt = cnt + 1;
                    //myConnection.Close();
                }

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);

                // Get the bottom stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                var method = frame.GetMethod().ReflectedType.FullName;
                var path = frame.GetFileName();

                MessageBox.Show(ml.ml_string(137, "An error occurred exporting:") + "\n" + ex.Message + "\n" + "Line: " + line + "\n" + "Method: " + method + "\n" + "Path: " + path); //MLHIDE

                //            return;
            }
        }

        public void UnzipAttachments(string ZipGUID, string ZipFileName)
        {
            if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\Attachments.zip")) //MLHIDE
            {

                using (ZipFile zipOpen = ZipFile.Read("Projects\\TrackingMaster\\ImportExport\\Attachments.zip")) //MLHIDE
                {
                    foreach (ZipEntry efzip in zipOpen.Where(x => x.FileName.Contains(ZipGUID) && x.FileName.Contains(ZipFileName)))
                    {
                        efzip.Extract(appDir + "\\FA\\Attachment\\", ExtractExistingFileAction.OverwriteSilently); //MLHIDE
                    }
                }

                using (ZipFile zipADD = ZipFile.Read(appDir + "\\FA\\Attachments\\Attachments.zip")) //MLHIDE
                {

                    DirectoryInfo di = new DirectoryInfo(appDir + "\\FA\\Attachment\\"); //MLHIDE
                    DirectoryInfo[] folder = null;
                    folder = di.GetDirectories();

                    foreach (var rowFolder in folder)
                    {
                        FileInfo fi = new FileInfo(rowFolder.Name.ToString());

                        //FileInfo[] filePaths = Directory.GetFiles(rowFolder.FullName.ToString()); //MLHIDE
                        DirectoryInfo dirInfo = new DirectoryInfo(rowFolder.FullName.ToString());
                        FileInfo[] filePaths = null;
                        filePaths = dirInfo.GetFiles();

                        foreach (FileInfo rowFile in filePaths)
                        {

                            var resultZip = zipADD.Any(entry => entry.FileName.Contains(rowFile.Name.ToString()) && entry.FileName.Contains(rowFolder.Name.ToString()));

                            if (resultZip == false)
                            {
                                zipADD.AddFile(rowFile.FullName.ToString(), rowFolder.Name.ToString());
                                zipADD.Save();
                                ResetAttachmentAlert(ZipGUID, ZipFileName);
                                CallbackSetStatusMessage("Attachment: " + rowFile.Name.ToString() + " Added...");
                                //LogActivity(AttachGUID, DateTime.Now, UserName.ToString());
                            }
                            else
                            {
                                zipADD.UpdateFile(rowFile.FullName.ToString(), rowFolder.Name.ToString());
                                zipADD.Save();
                                CallbackSetStatusMessage("Attachment: " + rowFile.Name.ToString() + " Updated...");
                            }
                        }
                    }
                }

                Directory.Delete(appDir + "\\FA\\Attachment\\", true); //MLHIDE
                Directory.CreateDirectory(appDir + "\\FA\\Attachment\\"); //MLHIDE

            }
        }

        public void ResetAttachmentAlert(string AttachGUID, string AttachFolder)
        { // gfj0 Updates the isClick setting for a newly added attachment

            OleDbConnection connect = new OleDbConnection();
            connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB"; //MLHIDE
            connect.Open();

            OleDbCommand command = new OleDbCommand();
            command.Connection = connect;

            command.CommandText = "UPDATE Requiredwrittenmaterials13 SET Requiredwrittenmaterials13.IsClickAttachment = Null WHERE (((Requiredwrittenmaterials13.AttachGUID)= '" + AttachGUID + "'));"; //MLHIDE

            command.ExecuteNonQuery();

            connect.Close();

        }
    }
}
