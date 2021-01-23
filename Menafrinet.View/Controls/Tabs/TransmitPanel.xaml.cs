using MultiLang;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data.OleDb;
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
using Menafrinet.Core;
using System.Diagnostics;
using Ionic.Zip;
using System.Threading;
using Epi.Menu;
using Epi.Data;
using System.Data;

namespace Menafrinet.View.Controls
{
    /// <summary>
    /// Interaction logic for TransmitPanel.xaml
    /// </summary>
    public partial class TransmitPanel : UserControl
    {
        public delegate void ExportCompletedHandler(ExportInfo info);

        private LabExportTypes LabExportType { get; set; }

        private DataHelper DataHelper
        {
            get
            {
                return (this.DataContext as DataHelper);
            }
        }

        private int elapsedTime;
        private bool eventHandled;
        string appDir = Environment.CurrentDirectory;
        object permvar;
        object permvar2;
        object permvar3;
        string CompVar;
        string exMessage;
        string[] AttachGUIDTab;
        string selcriteria;
        string DCreated;
        int SentNum;
        string SentCompleteTXT;

        public TransmitPanel()
        {
            InitializeComponent();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                btnExport.IsEnabled = false;
                Cursor = Cursors.Wait;

                permvar3 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm"); //MLHIDE


                if (permvar3 != null)
                {
                    if (permvar3.ToString() == "3")
                    {
                        permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident"); //MLHIDE
                    }
                    else
                    {
                        permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC"); //MLHIDE
                    }
                }


                if (permvar2 == null || permvar2.ToString() == "" || permvar3 == null || permvar3.ToString() == "")
                {
                    MessageBox.Show(ml.ml_string(118, "You can not merge data if user settings are not properly configured."), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    CallbackSetStatusMessage(String.Format(ml.ml_string(118, "You can not merge data if user settings are not properly configured."), "test"));

                    return;
                }

                Cursor = Cursors.Wait;

                txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + ml.ml_string(130, "Export processing...") + "\n";

                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\Projects.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\ImportExport\\Projects.edp7"); //MLHIDE
                }
                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ProjectsA.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\ImportExport\\ProjectsA.edp7"); //MLHIDE
                }
                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ProjectB.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\ImportExport\\ProjectsB.edp7"); //MLHIDE
                }
                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ConsultantInfo.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\ImportExport\\ConsultantInfo.edp7"); //MLHIDE
                }
                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\TraineeInformation.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\ImportExport\\TraineeInformation.edp7"); //MLHIDE
                }
                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\codeFieldSites.edp7")) //MLHIDE
                {
                    File.Delete("Projects\\TrackingMaster\\ImportExport\\codeFieldSites.edp7"); //MLHIDE
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

                elapsedTime = 0;
                eventHandled = false;
                string prjToImport;
                string trnToImport;
                string fsToImport;
                string frlToImport;
                string ConToImport;


                if ((bool)(deidentify.IsChecked))
                {
                    AddMessage(String.Format("Residents DeIdentified...")); //MLHIDE
                    prjToImport = "ProjectsDeIdentified"; //MLHIDE

                    AddMessage(String.Format(ml.ml_string(138, "Trainee Information DeIdentified...")));
                    trnToImport = "TraineeInfoDeIdentified"; //MLHIDE

                    AddMessage(String.Format(ml.ml_string(200, "Frontline Information DeIdentified...")));
                    frlToImport = "FrontlineDeIdentified"; //MLHIDE

                    AddMessage(String.Format(ml.ml_string(248, "Frontline Information DeIdentified...")));
                    ConToImport = "ConsultantDeidentified"; //MLHIDE
                }
                else
                {
                    prjToImport = "Projects"; //MLHIDE
                    trnToImport = "TraineeInformation"; //MLHIDE
                    fsToImport = "codeFieldSites"; //MLHIDE
                    frlToImport = "Frontline"; //MLHIDE
                    ConToImport = "ConsultantInfo"; //MLHIDE
                    //AddMessage(String.Format("Residents Names  R E M O V E D..."));
                }


                if ((bool)(AddResidents.IsChecked))
                {

                    txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + ml.ml_string(134, "Exporting residents...") + "\n";

                    if ((bool)(deidentify.IsChecked))
                    {

                        txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + ml.ml_string(139, "Residents de-identified") + "\n";
                    }


                    Process process = new Process();
                    process.StartInfo.FileName = "DataPackager"; //MLHIDE
                    process.StartInfo.Arguments = "script:Projects\\TrackingMaster\\ImportExport\\" + trnToImport + ".pks7 /autorun:true /smallsize:true"; //MLHIDE
                    process.StartInfo.ErrorDialog = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process.StartInfo.CreateNoWindow = true;
                    process.Exited += new EventHandler(myProcess_Exited);
                    process.Start();
                    process.WaitForExit();    // Wait up to five minutes.
                                              //process.Kill();
                                              //txtTransmit1.Text += "Residents  A D D E D...";

                    txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + ml.ml_string(131, "Exporting consultants...") + "\n";

                    if ((bool)(deidentify.IsChecked))
                    {

                        txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + ml.ml_string(132, "Consultants de-identified") + "\n";
                    }


                    Process process1 = new Process();
                    process1.StartInfo.FileName = "DataPackager"; //MLHIDE
                    process1.StartInfo.Arguments = "script:Projects\\TrackingMaster\\ImportExport\\" + ConToImport + ".pks7 /autorun:true /smallsize:true"; //MLHIDE
                    process1.StartInfo.ErrorDialog = true;
                    process1.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process1.StartInfo.CreateNoWindow = true;
                    process1.Exited += new EventHandler(myProcess_Exited);
                    process1.Start();
                    process1.WaitForExit();    // Wait up to five minutes.
                                               //process1.Kill();
                                               //txtTransmit1.Text += "Consultants  A D D E D...";

                    process.StartInfo.Arguments = "script:Projects\\TrackingMaster\\ImportExport\\codeFieldSites.pks7 /autorun:true /smallsize:true"; //MLHIDE
                    process.StartInfo.ErrorDialog = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process.StartInfo.CreateNoWindow = true;
                    process.Exited += new EventHandler(myProcess_Exited);
                    process.Start();
                    process.WaitForExit();    // Wait up to five minutes.
                                              //Epi.Menu.EpiInfoMenuManager.OpenDataPackagerWithScript("Projects\\TrackingMaster\\ImportExport\\TraineeInformation.pks7", true, true);

                    txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + ml.ml_string(193, "Exporting Field Sites...") + "\n";

                }

                txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + ml.ml_string(222, "Exporting project data...") + "\n";

                if ((bool)(deidentify.IsChecked))
                {

                    txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + ml.ml_string(135, "Projects de-identified") + "\n";
                }


                int initiated = 0;

                if (System.IO.File.Exists(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath))
                {
                    Project project3 = new Project(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

                    IDbDriver db3 = project3.CollectedData.GetDatabase();
                    System.Data.DataTable dt3 = db3.GetTableData("Projects", "UniqueKey"); //MLHIDE

                    initiated = dt3.Rows.Count;
                }

                if (initiated > 0)
                {
                    if (permvar3.ToString() == "3")
                    {
                        OleDbConnection connectProj = new OleDbConnection();
                        connectProj.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB"; //MLHIDE
                        connectProj.Open();

                        OleDbCommand commandProj = new OleDbCommand();
                        commandProj.Connection = connectProj;

                        if (permvar3.ToString() == "3")
                        {
                            CompVar = "[LeadResident] = '" + permvar2.ToString() + "'"; //MLHIDE
                        }
                        else
                        {
                            CompVar = "[LeadResident] Is Not Null"; //MLHIDE
                        }

                        commandProj.CommandText = "UPDATE Projects4 SET Projects4.MarkForExport = IIf(" + CompVar + ", 1, 0);"; //MLHIDE

                        //commandProj.Parameters.AddWithValue("UserSettingResident", permvar2);
                        commandProj.ExecuteNonQuery();
                        connectProj.Close();

                        Process process2 = new Process();
                        process2.StartInfo.FileName = "DataPackager"; //MLHIDE
                        process2.StartInfo.Arguments = "script:Projects\\TrackingMaster\\ImportExport\\" + prjToImport + "A.pks7 /autorun:true /smallsize:true"; //MLHIDE
                        process2.StartInfo.ErrorDialog = true;
                        process2.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        process2.StartInfo.CreateNoWindow = false;
                        process2.Exited += new EventHandler(myProcess_Exited);
                        process2.Start();
                        process2.WaitForExit();    // Wait up to five minutes.
                                                   //txtTransmit1.Text += "Projects  A D D E D...";
                                                   //process2.Kill();
                        SetStatusMessage("Projects  A D D E D...");

                        Process process2a = new Process();
                        process2a.StartInfo.FileName = "DataPackager"; //MLHIDE
                        process2a.StartInfo.Arguments = "script:Projects\\TrackingMaster\\ImportExport\\" + prjToImport + "B.pks7 /autorun:true /smallsize:true"; //MLHIDE
                        //process2.StartInfo.ErrorDialog = true;
                        process2a.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        process2a.StartInfo.CreateNoWindow = false;
                        process2.Exited += new EventHandler(myProcess_Exited);
                        process2a.Start();
                        process2a.WaitForExit();    // Wait up to five minutes.
                                                    //txtTransmit1.Text += "Projects  A D D E D...";
                                                    //process2.Kill();
                    }
                    else
                    {
                        Process process2a = new Process();
                        process2a.StartInfo.FileName = "DataPackager"; //MLHIDE
                        process2a.StartInfo.Arguments = "script:Projects\\TrackingMaster\\ImportExport\\" + prjToImport + ".pks7 /autorun:true /smallsize:true"; //MLHIDE
                        //process2.StartInfo.ErrorDialog = true;
                        process2a.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        process2a.StartInfo.CreateNoWindow = false;
                        process2a.EnableRaisingEvents = true;
                        process2a.Exited += new EventHandler(myProcess_Exited);
                        process2a.Start();
                        process2a.WaitForExit();    // Wait up to five minutes.
                        SetStatusMessage("Projects  A D D E D...");
                    }
                }

                int initiated2 = 0;

                if (System.IO.File.Exists(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath))
                {
                    Project project3 = new Project(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

                    IDbDriver db3 = project3.CollectedData.GetDatabase();
                    System.Data.DataTable dt3 = db3.GetTableData("Frontline", "UniqueKey"); //MLHIDE

                    initiated2 = dt3.Rows.Count;
                }

                if (initiated2 > 0)
                {
                    Process process3 = new Process();
                    process3.StartInfo.FileName = "DataPackager"; //MLHIDE
                    process3.StartInfo.Arguments = "script:Projects\\TrackingMaster\\ImportExport\\" + frlToImport + ".pks7 /autorun:true /smallsize:true"; //MLHIDE
                    process3.StartInfo.ErrorDialog = true;
                    process3.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process3.StartInfo.CreateNoWindow = true;
                    process3.Exited += new EventHandler(myProcess_Exited);
                    process3.Start();
                    process3.WaitForExit();    // Wait up to five minutes.
                                               //Epi.Menu.EpiInfoMenuManager.OpenDataPackagerWithScript("Projects\\TrackingMaster\\ImportExport\\TraineeInformation.pks7", true, true);

                    txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + ml.ml_string(207, "Exporting Frontline data...") + "\n";

                    if ((bool)(deidentify.IsChecked))
                    {

                        txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + ml.ml_string(135, "Projects de-identified") + "\n";
                    }
                }


                //Epi.Menu.EpiInfoMenuManager.OpenDataPackagerWithScript("Projects\\TrackingMaster\\ImportExport\\ConsultantInfo.pks7", true, true);

                Properties.Settings.Default.DateLastExport = DateTime.Now;
                Properties.Settings.Default.UserLastExport = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                //Properties.Settings.Default.RecordsLastExport = info.TotalRecordsPackaged;
                Properties.Settings.Default.Save();

                ZipFiles();

                txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + ml.ml_string(136, "Export sucessfully completed.") + "\n\n";

                this.Cursor = Cursors.Arrow;
                btnExport.IsEnabled = true;


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

        //AddMessage(String.Format("Field Sites  A D D E D..."));
        private void ZipFiles()
        {
            //object permvar2;

            using (ZipFile zip = new ZipFile())
            {
                if ((bool)(AddResidents.IsChecked))
                {
                    zip.AddFile("Projects\\TrackingMaster\\ImportExport\\ConsultantInfo.edp7"); //MLHIDE
                    zip.AddFile("Projects\\TrackingMaster\\ImportExport\\TraineeInformation.edp7"); //MLHIDE
                    zip.AddFile("Projects\\TrackingMaster\\ImportExport\\codeFieldSites.edp7"); //MLHIDE
                }

                //LoadConfig();
                //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml"); //MLHIDE
                //object permvar3 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm");

                //if (permvar3 != null)
                //{
                //    if (permvar3.ToString() == "3")
                //    {
                //        permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident");
                //    }
                //    else
                //    {
                //        permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC");
                //    }


                DateTime dt = DateTime.UtcNow;
                string dateDisplayValue = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:s}", dt); //MLHIDE
                dateDisplayValue = dateDisplayValue.Replace(':', '-'); // The : must be replaced otherwise the encryption fails


                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ProjectsA.edp7")) //MLHIDE
                {
                    zip.AddFile("Projects\\TrackingMaster\\ImportExport\\ProjectsA.edp7"); //MLHIDE
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ProjectsB.edp7")) //MLHIDE
                {
                    zip.AddFile("Projects\\TrackingMaster\\ImportExport\\ProjectsB.edp7"); //MLHIDE
                }

                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\Frontline.edp7")) //MLHIDE
                {
                    zip.AddFile("Projects\\TrackingMaster\\ImportExport\\Frontline.edp7"); //MLHIDE
                }

                PrepZip();

                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\Attachments.zip")) //MLHIDE
                {
                    zip.AddFile("Projects\\TrackingMaster\\ImportExport\\Attachments.zip"); //MLHIDE
                }

                ExportLog();

                if (System.IO.File.Exists("Projects\\TrackingMaster\\ImportExport\\ExportActivityLog.csv")) //MLHIDE
                {
                    zip.AddFile("Projects\\TrackingMaster\\ImportExport\\ExportActivityLog.csv"); //MLHIDE
                }

                //zip.Save(appDir + "\\Projects\\TrackingMaster\\DataExports\\ProjectTracking_" + permvar2.ToString().Replace(", ", "_") + "_" + dateDisplayValue + ".edp7");

                string expFile;
                if ((bool)(deidentify.IsChecked))
                {
                    expFile = appDir + "\\Projects\\TrackingMaster\\DataExports\\ProjectTracking_" + permvar2.ToString() + "_" + dateDisplayValue + ml.ml_string(220, "(Deidentified)"); //MLHIDE
                }
                else
                {
                    expFile = appDir + "\\Projects\\TrackingMaster\\DataExports\\ProjectTracking_" + permvar2.ToString() + "_" + dateDisplayValue; //MLHIDE
                }
                if ((bool)(AddResidents.IsChecked))
                {
                    expFile += ml.ml_string(221, "(FULL)") + ".edp7";  //MLHIDE
                }
                else
                {
                    expFile += ".edp7";  //MLHIDE
                }

                zip.Save(expFile);

                //}

            }

            //}


            //Open the Export folder.
            {
                //Tells the user to copy the *.edp7 file from the DataExports folder when the package is created.
                //MessageBox.Show("Copy the file from the DataExports folder", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                MessageBoxResult result = MessageBox.Show(ml.ml_string(140, "Copy the ReDPeTT file from the Exports folder."), ml.ml_string(141, "Message"), MessageBoxButton.OK, MessageBoxImage.Information);

                if (result == MessageBoxResult.OK)
                {
                    //FileInfo fo = new FileInfo(DataHelper.Project.FilePath);
                    DirectoryInfo di = new DirectoryInfo(appDir + "\\Projects\\TrackingMaster\\DataExports"); //MLHIDE
                    System.Diagnostics.Process.Start("explorer.exe", di.FullName); //MLHIDE
                }
            }
            //Rmvd 8/31
            ////Tells the user to copy the *.edp7 file from the DataExports folder when the package is created.
            ////MessageBox.Show("Copy the file from the DataExports folder", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            //MessageBox.Show("Copiez le fichier de données du Exportations dossier.", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        // Handle Exited event and display process information.
        private void myProcess_Exited(object sender, System.EventArgs e)
        {

            eventHandled = true;
            //Console.WriteLine("Exit time:    {0}\r\n" +
            //    "Exit code:    {1}\r\nElapsed time: {2}", process.ExitTime, process.ExitCode, elapsedTime);
        }

        private void AddMessage(string message)
        {
            //Contract.Requires(message != null);

            //Contract.Ensures(ExportMessages != null);

            //ExportMessages += (message + Environment.NewLine);
        }

        private void packager_MessageGenerated(string message)
        {
            AddMessage(message);
        }

        private void packager_UpdateProgress(double progress)
        {
            //Contract.Requires(progress >= 0);

            //Contract.Ensures(Progress >= 0);

            //Progress = progress;
        }


        void exportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ExportInfo info = (ExportInfo)e.Result;
            this.Dispatcher.BeginInvoke(new ExportCompletedHandler(UpdateExportInfo), info);
            (Application.Current as App).CanMerge = true;
            (Application.Current as App).CanTransmit = true;

            this.DataHelper.TaskbarProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
            this.DataHelper.TaskbarProgressValue = 1.0;
        }

        void exportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ExportSettings settings = (ExportSettings)e.Argument;
            ExportInfo info = this.CreateDataPackage(settings);
            e.Result = info;
        }

        internal void ResetHeight()
        {
            grdHome.UpdateLayout();
            txtTransmit1.Height = Math.Min(260, Math.Max(40, grdHome.ActualHeight - 410));
        }

        private void SetProgressBar(double value)
        {
            if (Double.IsInfinity(value)) // problem with Epi Info code, sometimes it comes back as infinity and crashes the app
            {
                value = 0;
            }
            //exportProgressBar.Value = value;
            this.DataHelper.TaskbarProgressValue = value / 100;
        }

        private void SetStatusMessage(string message)
        {
            txtTransmit1.Text = txtTransmit1.Text + DateTime.Now.ToString() + " | " + message + "\n";
        }

        private void CallbackSetProgressBar(double value)
        {
            this.Dispatcher.BeginInvoke(new SetProgressBarDelegate(SetProgressBar), value);
        }

        private void CallbackSetStatusMessage(string message)
        {
            this.Dispatcher.BeginInvoke(new UpdateStatusEventHandler(SetStatusMessage), message);
        }

        public Epi.ImportExport.ProjectPackagers.ExportInfo CreateDataPackage(ExportSettings settings)
        {
            Epi.Fields.Field epidField = settings.CaseForm.Fields["EpidShow"];
            //Epi.Fields.Field epidField = settings.CaseForm.Fields["EpidNumber"]; db name change chng bk to orig
            XmlDataPackager xmlDataPackager = new XmlDataPackager(settings.CaseForm, "Menafrinet");
            xmlDataPackager.UpdateProgress += new SetProgressBarDelegate(CallbackSetProgressBar);
            xmlDataPackager.StatusChanged += new UpdateStatusEventHandler(CallbackSetStatusMessage);
            //xmlDataPackager.FieldsToNull = new Dictionary<string, List<string>>();
            xmlDataPackager.KeyFields.Add(epidField);
            RowFilters rowFilters = new RowFilters(settings.Database);
            XmlDocument xmlDoc = null;
            ExportInfo info = null;

            List<string> fieldsToNull = new List<string>();

            switch (Properties.Settings.Default.SystemLevel)
            {
                case 0: // We're a district.
                        // Remove all fields from the package related to lab data.
                        // Because field names may change across languages, let's just grab all the fields on pages 2, 3, and 4 and add them to the "remove" list
                        //fieldsToNull = new List<string>() { "ReceivedDateNRL", "SpecimenIDNRL", "SpecimenEPIDNRL", "SpecimenConditionNRL", "OtherTubeNRL", "TINRL", "CryotubeNRL", "SpecimenConditionNRL", "AspectNRL", "LabTestCYTOLOGIENRL", "WhiteCellCountNRL", "CSFDifferentialNRL", "PolyPercentNRL", "MonoPercentNRL", "TDR_MeningitisNRL", "GramNRL", "GramOtherNRL", "LatexNRL", "CultureNRL", "CultureOtherNRL", "TestPerformed", "DateCultureNRL", "PCRType", "DatePCR", "PCR", "SPECIMENTEMP", "PCRRNASEPRESULT", "PCRRNASEPCT", "PCRSPECIESRESULT", "PCRSPECIESCT", "PCRSPECIESINT", "PCRSGSTRESULT", "PCRSGSTCT", "SeroGroupType", "FinalResultNRL", "FinalResultOtherNRL", "Ceftriaxone", "Penicillin", "Chloramphenicol", "OtherAntibiotic", "OtherAntibioticSensitivity", "AntiBiogramTestUsed", "DateFinalResultNRLsentDistrict", "DateFinalResultNRLsentRegion", "DateFinalResultNRLsentMoH", "ObservationsNRL"  };

                    foreach (Epi.Page page in settings.CaseForm.Pages)
                    {
                        foreach (Epi.Fields.RenderableField field in page.Fields)
                        {
                            if (field is Epi.Fields.IDataField && page.Name.ToLower().StartsWith("Lab") && !fieldsToNull.Contains(field.Name))
                            {
                                //fieldsToNull.Add(field.Name);

                            }
                        }
                    }
                    break;
                case 1: // We're the national reference lab.
                    // Remove data from all page 1 fields except the EPID field, which we need for matching, and
                    // then also remove some additional fields the customer specified which should never go to anyone
                    //fieldsToNull = new List<string>() { "FamilyName", "DateCultureNRL", "PolyPercentNRL", "METHODSHIPMENT", "SPECIMENTEMP", "PCRRNASEPRESULT", "PCRRNASEPCT", "PCRSPECIESRESULT", "PCRSPECIESCT", "PCRSPECIESINT", "PCRSGSTRESULT", "PCRSGSTCT", "PCRSGSTINT", "PCRFINALINT", "PCRCOMMENTS" };
                    //fieldsToNull = new List<string>() { "Region", "District", "HealthFacility", "FamilyName", "FirstName", "DateOfBirth", "AgeYear", "AgeMonths", "ParentName", "Village", "Sex", "Neighborhood", "DistrictOfResidence", "UrbanRural", "PhoneNum", "Address", "DateConsultation", "DateOnset", "VaccinationStatus", "MenA", "Hib", "PCV", "DoseMenA", "DoseHIB", "DosePneumo", "SourceMenA", "SourceHib", "SourcePCV", "DateMenA", "DateHib", "DatePCV","MenAC", "MenACW", "MenACWY", "VaccineUnknown", "SourceMenAC", "SourceACW", "SourceACWY", "VaccineUnknown", "DateMenAC", "DateACW", "DateACWY", "InOutPatient", "SpecimenCollected", "TDR_Meningitis", "Outcome", "DateSpecimenCollected", "ASPECT", "DateNotificationDistrict", "SentDateDistrict", "reporterHF", "ReporterHFPhone" };

                    foreach (Epi.Page page in settings.CaseForm.Pages)
                    {
                        foreach (Epi.Fields.RenderableField field in page.Fields)
                        {
                            //if (field is Epi.Fields.IDataField && !page.Name.ToLower().StartsWith("lab") && !fieldsToNull.Contains(field.Name) && (!field.Name.Equals("EpidShow") && !field.Name.Equals("EPIDNUMBER")))
                            //if (field is Epi.Fields.IDataField && !page.Name.ToLower().StartsWith("lab") && !fieldsToNull.Contains(field.Name) && (!field.Name.Equals("EpidNumber") && !field.Name.Equals("EPIDNUMBER"))) DB name change chng bk to orig
                            {
                                //fieldsToNull.Add(field.Name);
                            }
                        }
                    }
                    break;
                case 2: // we're a region
                        // The district's demographics data section should never be overwritten durning a transmission from the region.
                        // Remove data from all page 1 fields except the EPID field, which we need for matching, and
                        // then also remove some additional fields the customer specified which should never go to anyone
                        // { "Region", "District", "HealthFacility", "FamilyName", "FirstName", "DateOfBirth", "AgeYear", "AgeMonths", "ParentName", "Village", "Sex", "Neighborhood", "DistrictOfResidence", "UrbanRural", "PhoneNum", "Address", "DateConsultation", "DateOnset", "VaccinationStatus", "MenA", "Hib", "PCV", "DoseMenA", "DoseHIB", "DosePneumo", "SourceMenA", "SourceHib", "SourcePCV", "DateMenA", "DateHib", "DatePCV", "MenAC", "MenACW", "MenACWY", "VaccineUnknown", "SourceMenAC", "SourceACW", "SourceACWY", "VaccineUnknown", "DateMenAC", "DateACW", "DateACWY", "InOutPatient", "SpecimenCollected", "TDR_Meningitis", "Outcome", "DateSpecimenCollected", "ASPECT", "DateNotificationDistrict", "SentDateDistrict", "reporterHF", "ReporterHFPhone", "ReceivedDateNRL", "SpecimenIDNRL", "SpecimenEPIDNRL", "SpecimenConditionNRL", "OtherTubeNRL", "TINRL", "CryotubeNRL", "SpecimenConditionNRL", "AspectNRL", "LabTestCYTOLOGIENRL", "WhiteCellCountNRL", "CSFDifferentialNRL", "PolyPercentNRL", "MonoPercentNRL", "TDR_MeningitisNRL", "GramNRL", "GramOtherNRL", "LatexNRL", "CultureNRL", "CultureOtherNRL", "TestPerformed", "DateCultureNRL", "PCRType", "DatePCR", "PCR", "SPECIMENTEMP", "PCRRNASEPRESULT", "PCRRNASEPCT", "PCRSPECIESRESULT", "PCRSPECIESCT", "PCRSPECIESINT", "PCRSGSTRESULT", "PCRSGSTCT", "SeroGroupType", "FinalResultNRL", "FinalResultOtherNRL", "Ceftriaxone", "Penicillin", "Chloramphenicol", "OtherAntibiotic", "OtherAntibioticSensitivity", "AntiBiogramTestUsed", "DateFinalResultNRLsentDistrict", "DateFinalResultNRLsentRegion", "DateFinalResultNRLsentMoH", "ObservationsNRL" };
                        //fieldsToNull = new List<string>() { "Region", "District", "HealthFacility", "FamilyName", "FirstName", "DateOfBirth", "AgeYear", "AgeMonths", "ParentName", "Village", "Sex", "Neighborhood", "DistrictOfResidence", "UrbanRural", "PhoneNum", "Address", "DateConsultation", "DateOnset", "VaccinationStatus", "MenA", "Hib", "PCV", "DoseMenA", "DoseHIB", "DosePneumo", "SourceMenA", "SourceHib", "SourcePCV", "DateMenA", "DateHib", "DatePCV", "MenAC", "MenACW", "MenACWY", "VaccineUnknown", "SourceMenAC", "SourceACW", "SourceACWY", "VaccineUnknown", "DateMenAC", "DateACW", "DateACWY", "InOutPatient", "SpecimenCollected", "TDR_Meningitis", "Outcome", "DateSpecimenCollected", "ASPECT", "DateNotificationDistrict", "DateSentDistrict", "reporterHF", "ReporterHFPhone" };

                    foreach (Epi.Page page in settings.CaseForm.Pages)
                    {
                        foreach (Epi.Fields.RenderableField field in page.Fields)
                        {
                            //if (field is Epi.Fields.IDataField && !page.Name.ToLower().StartsWith("lab") && !fieldsToNull.Contains(field.Name) && (!field.Name.Equals("EpidShow") && !field.Name.Equals("EPIDNUMBER")))
                            //if (field is Epi.Fields.IDataField && !page.Name.ToLower().StartsWith("lab") && !fieldsToNull.Contains(field.Name) && (!field.Name.Equals("EpidNumber") && !field.Name.Equals("EPIDNUMBER"))) DB name change chng bk to orig
                            {
                                //fieldsToNull.Add(field.Name);
                            }
                        }
                    }
                    break;
                case 3: // we're an MoH
                        // The district's demographics data section should never be overwritten durning a transmission from the region.
                        // Remove data from all page 1 fields except the EPID field, which we need for matching, and
                        // then also remove some additional fields the customer specified which should never go to anyone

                    if (settings.LabExportType == LabExportTypes.ToDistrict)
                    {
                        // null fields that shouldn't be over-written when the district imports this data
                        //fieldsToNull = new List<string>() { "Region", "District", "HealthFacility", "FamilyName", "FirstName", "DateOfBirth", "AgeYear", "AgeMonths", "ParentName", "Village", "Sex", "Neighborhood", "DistrictOfResidence", "UrbanRural", "PhoneNum", "Address", "DateConsultation", "DateOnset", "VaccinationStatus", "MenA", "Hib", "PCV", "DoseMenA", "DoseHIB", "DosePneumo", "SourceMenA", "SourceHib", "SourcePCV", "DateMenA", "DateHib", "DatePCV", "MenAC", "MenACW", "MenACWY", "VaccineUnknown", "SourceMenAC", "SourceACW", "SourceACWY", "VaccineUnknown", "DateMenAC", "DateACW", "DateACWY", "InOutPatient", "SpecimenCollected", "TDR_Meningitis", "Outcome", "DateSpecimenCollected", "ASPECT", "DateNotificationDistrict", "DateSentDistrict", "reporterHF", "ReporterHFPhone", "ReceivedDateNRL", "SpecimenIDNRL", "SpecimenEPIDNRL", "SpecimenConditionNRL", "OtherTubeNRL", "TINRL", "CryotubeNRL", "SpecimenConditionNRL", "AspectNRL", "LabTestCYTOLOGIENRL", "WhiteCellCountNRL", "CSFDifferentialNRL", "PolyPercentNRL", "MonoPercentNRL", "TDR_MeningitisNRL", "GramNRL", "GramOtherNRL", "LatexNRL", "CultureNRL", "CultureOtherNRL", "TestPerformed", "DateCultureNRL", "PCRType", "DatePCR", "PCR", "SPECIMENTEMP", "PCRRNASEPRESULT", "PCRRNASEPCT", "PCRSPECIESRESULT", "PCRSPECIESCT", "PCRSPECIESINT", "PCRSGSTRESULT", "PCRSGSTCT", "SeroGroupType", "FinalResultNRL", "FinalResultOtherNRL", "Ceftriaxone", "Penicillin", "Chloramphenicol", "OtherAntibiotic", "OtherAntibioticSensitivity", "AntiBiogramTestUsed", "DateFinalResultNRLsentDistrict", "DateFinalResultNRLsentRegion", "DateFinalResultNRLsentMoH", "ObservationsNRL" };
                    }

                    break;
                case 4: // we're Menafrinet
                        // Should never get here... what is Menafrinet going to transmit?? Data flow model does not include going anywhere from this level.
                        // The district's demographics data section should never be overwritten durning a transmission from the region.
                        // Remove data from all page 1 fields except the EPID field, which we need for matching, and
                        // then also remove some additional fields the customer specified which should never go to anyone
                        //    // { "Region", "District", "HealthFacility", "FamilyName", "FirstName", "DateOfBirth", "AgeYear", "AgeMonths", "ParentName", "Village", "Sex", "Neighborhood", "DistrictOfResidence", "UrbanRural", "PhoneNum", "Address", "DateConsultation", "DateOnset", "VaccinationStatus", "MenA", "Hib", "PCV", "DoseMenA", "DoseHIB", "DosePneumo", "SourceMenA", "SourceHib", "SourcePCV", "DateMenA", "DateHib", "DatePCV", "MenAC", "MenACW", "MenACWY", "VaccineUnknown", "SourceMenAC", "SourceACW", "SourceACWY", "VaccineUnknown", "DateMenAC", "DateACW", "DateACWY", "InOutPatient", "SpecimenCollected", "TDR_Meningitis", "Outcome", "DateSpecimenCollected", "ASPECT", "DateNotificationDistrict", "DateSentDistrict", "reporterHF", "ReporterHFPhone", "ReceivedDateNRL", "SpecimenIDNRL", "SpecimenEPIDNRL", "SpecimenConditionNRL", "OtherTubeNRL", "TINRL", "CryotubeNRL", "SpecimenConditionNRL", "AspectNRL", "LabTestCYTOLOGIENRL", "WhiteCellCountNRL", "CSFDifferentialNRL", "PolyPercentNRL", "MonoPercentNRL", "TDR_MeningitisNRL", "GramNRL", "GramOtherNRL", "LatexNRL", "CultureNRL", "CultureOtherNRL", "TestPerformed", "DateCultureNRL", "PCRType", "DatePCR", "PCR", "SPECIMENTEMP", "PCRRNASEPRESULT", "PCRRNASEPCT", "PCRSPECIESRESULT", "PCRSPECIESCT", "PCRSPECIESINT", "PCRSGSTRESULT", "PCRSGSTCT", "SeroGroupType", "FinalResultNRL", "FinalResultOtherNRL", "Ceftriaxone", "Penicillin", "Chloramphenicol", "OtherAntibiotic", "OtherAntibioticSensitivity", "AntiBiogramTestUsed", "DateFinalResultNRLsentDistrict", "DateFinalResultNRLsentRegion", "DateFinalResultNRLsentMoH", "ObservationsNRL" };
                        //fieldsToNull = new List<string>() { "Region", "District", "HealthFacility", "FamilyName", "FirstName", "DateOfBirth", "AgeYear", "AgeMonths", "ParentName", "Village", "Sex", "Neighborhood", "DistrictOfResidence", "UrbanRural", "PhoneNum", "Address", "DateConsultation", "DateOnset", "VaccinationStatus", "MenA", "Hib", "PCV", "DoseMenA", "DoseHIB", "DosePneumo", "SourceMenA", "SourceHib", "SourcePCV", "DateMenA", "DateHib", "DatePCV", "MenAC", "MenACW", "MenACWY", "VaccineUnknown", "SourceMenAC", "SourceACW", "SourceACWY", "VaccineUnknown", "DateMenAC", "DateACW", "DateACWY", "InOutPatient", "SpecimenCollected", "TDR_Meningitis", "Outcome", "DateSpecimenCollected", "ASPECT", "DateNotificationDistrict", "DateSentDistrict", "reporterHF", "ReporterHFPhone" };

                    //foreach (Epi.Page page in settings.CaseForm.Pages)
                    //{
                    //    foreach (Epi.Fields.RenderableField field in page.Fields)
                    //    {
                    //        if (field is Epi.Fields.IDataField && !page.Name.ToLower().StartsWith("lab") && !fieldsToNull.Contains(field.Name) && (!field.Name.Equals("EpidShow") && !field.Name.Equals("EPIDNUMBER")))
                    //        //if (field is Epi.Fields.IDataField && !page.Name.ToLower().StartsWith("lab") && !fieldsToNull.Contains(field.Name) && (!field.Name.Equals("EpidNumber") && !field.Name.Equals("EPIDNUMBER"))) DB name change chng bk to orig
                    //        {
                    //            //fieldsToNull.Add(field.Name);
                    //        }
                    //    }
                    //}
                    break;
            }

            xmlDataPackager.FieldsToNull.Add(settings.CaseForm.Name, fieldsToNull);

            string compressedText = String.Empty;

            FileInfo exportFileInfo = new FileInfo(System.IO.Path.Combine(settings.OutputDirectory.FullName, settings.FileName + ".edp7"));

            FileInfo fi_Copy = new FileInfo(settings.ProjectPath);

            if (File.Exists("Projects\\Menafrinet\\Menafrinet_NE1.mdb"))
            {
                File.Delete("Projects\\Menafrinet\\Menafrinet_NE1.mdb");
            }

            File.Copy(@"Projects\\Menafrinet\\Menafrinet_NE.mdb", @"Projects\\Menafrinet\\Menafrinet_NE1.mdb"); //MLHIDE

            /*   DirectoryInfo di = fi.Directory*/
            ;

            OleDbConnection connect = new OleDbConnection();                       // \\Projects\\Menafrinet\\Menafrinet_NE1.mdb
            connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Projects\Menafrinet\Menafrinet_NE1.mdb"; //MLHIDE
            connect.Open();

            OleDbCommand command = new OleDbCommand();
            command.Connection = connect;

            command.CommandText = "UPDATE FichedeNotification1 SET FichedeNotification1.Region = Null=Null, FichedeNotification1.RegCode = Null, FichedeNotification1.District = Null, FichedeNotification1.DistCode = Null, FichedeNotification1.HealthFacility = Null, FichedeNotification1.FamilyName = Null, FichedeNotification1.FirstName = Null, FichedeNotification1.DateOfBirth = Null, FichedeNotification1.AgeYear = Null, FichedeNotification1.AgeMonths = Null, FichedeNotification1.ParentName = Null, FichedeNotification1.Village = Null, FichedeNotification1.Sex = Null, FichedeNotification1.Neighborhood = Null, FichedeNotification1.DistrictOfResidence = Null, FichedeNotification1.UrbanRural = Null, FichedeNotification1.PhoneNum = Null, FichedeNotification1.Address = Null, FichedeNotification1.DateConsultation = Null, FichedeNotification1.DateOnset = Null WHERE FichedeNotification1.OriginLevel = 'DIS';"; //MLHIDE

            command.ExecuteNonQuery();
            // gfj0 check this out connect.Close();

            FileInfo fi = new FileInfo("Projects\\Menafrinet\\Menafrinet_NE1.mdb"); //MLHIDE

            xmlDataPackager.Filters = new Dictionary<string, RowFilters>();

            if (settings.LabExportType == LabExportTypes.ToDistrict)
            {
                //TextRowFilterCondition tfc = new TextRowFilterCondition("[DistCode] = @DistCode", "DistCode", "@DistCode", settings.LabExportDestinationCode);
                //tfc.Description = "DistCode is equal to " + settings.LabExportDestinationCode;
                //rowFilters.Add(tfc);
            }
            else if (settings.LabExportType == LabExportTypes.ToRegion)
            {
                //TextRowFilterCondition tfc = new TextRowFilterCondition("[RegCode] = @RegCode", "RegCode", "@RegCode", settings.LabExportDestinationCode);
                //tfc.Description = "RegCode is equal to " + settings.LabExportDestinationCode;
                //rowFilters.Add(tfc);
            }

            switch (settings.ExportType)
            {
                case ExportType.AllRecordsSinceLast:
                    // arbitrary, test this on first run to see what happens when date shows up as 1/1/0001
                    if (Properties.Settings.Default.DateLastExport.Year > 1920)
                    {
                        DateTime lastExport = Properties.Settings.Default.DateLastExport;
                        DateTimeRowFilterCondition sinceLastFc = new DateTimeRowFilterCondition("[LastSaveTime] >= @LastSaveTime", "LastSaveTime", "@LastSaveTime", lastExport);
                        sinceLastFc.Description = "Last save time is greater than or equal to " + lastExport.ToString();
                        rowFilters.Add(sinceLastFc);
                    }
                    xmlDataPackager.Filters.Add(settings.CaseForm.Name, rowFilters);
                    break;
                //case ExportType.LastMonth:
                //    DateTime lastMonth = DateTime.Today.AddDays(-30);
                //    DateRowFilterCondition dfc = new DateRowFilterCondition("[LastSaveTime] >= @LastSaveTime", "LastSaveTime", "@LastSaveTime", lastMonth);
                //    dfc.Description = "Last save time is greater than or equal to " + lastMonth.ToString();
                //    rowFilters.Add(dfc);
                //    xmlDataPackager.Filters.Add(settings.CaseForm.Name, rowFilters);

                //    //txtTransmit1.Text = "'Mois' sélectionné \nExport terminée avec succès. \nRecords emballés: " + info.TotalRecordsPackaged;

                //    break;
                case ExportType.DateRange:
                    if (!settings.StartDate.HasValue || !settings.EndDate.HasValue)
                    {
                        //txtTransmit1.Text = "ERREUR: Date de Début et Date de Fin ne peut pas être vide.";
                        return info;
                    }
                    else if (settings.StartDate.Value > settings.EndDate.Value)
                    {
                        return info;
                    }

                    DateTime newUpper = new DateTime(settings.EndDate.Value.Year, settings.EndDate.Value.Month, settings.EndDate.Value.Day, 23, 59, 59);

                    DateTimeRowFilterCondition lower = new DateTimeRowFilterCondition("[LastSaveTime] >= @LastSaveTime1", "LastSaveTime", "@LastSaveTime1", settings.StartDate);
                    DateTimeRowFilterCondition upper = new DateTimeRowFilterCondition("[LastSaveTime] <= @LastSaveTime2", "LastSaveTime", "@LastSaveTime2", newUpper);

                    lower.Description = "Last Save Time >= lower";
                    upper.Description = "Last Save Time <= upper";

                    lower.Description = "Last save time is greater than or equal to " + settings.StartDate.Value.ToString();
                    lower.Description = "Last save time is less than or equal to " + newUpper.ToString();

                    rowFilters.Add(lower);
                    rowFilters.Add(upper);

                    xmlDataPackager.Filters.Add(settings.CaseForm.Name, rowFilters);
                    break;
                default:
                    xmlDataPackager.Filters.Add(settings.CaseForm.Name, rowFilters);
                    break;
            }

            try
            {
                xmlDoc = xmlDataPackager.PackageForm();

                XmlAttribute codeAttribute = xmlDoc.CreateAttribute("SiteCode");
                XmlAttribute levelAttribute = xmlDoc.CreateAttribute("SystemLevel");

                codeAttribute.Value = Properties.Settings.Default.SiteCode;
                levelAttribute.Value = Properties.Settings.Default.SystemLevel.ToString();

                xmlDoc.ChildNodes[0].Attributes.Append(codeAttribute);
                xmlDoc.ChildNodes[0].Attributes.Append(levelAttribute);

                info = xmlDataPackager.ExportInfo;

                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                XmlWriter writer = XmlWriter.Create("data.xml", setting);
                xmlDoc.Save(writer);

                if (!settings.SourceLanguage.ToLower().Equals("en-us"))
                {
                    CallbackSetStatusMessage(String.Format("Translating field names in data package from {0}...", settings.SourceLanguage));
                    xmlDoc = Core.Common.TranslateDataPackageFieldsToInvariant(xmlDoc, settings.SourceLanguage);
                    CallbackSetStatusMessage("Translation of field names complete.");
                }

                compressedText = ImportExportHelper.Zip(xmlDoc.OuterXml);
                compressedText = "[[EPIINFO7_DATAPACKAGE]]" + compressedText;

                Configuration.EncryptStringToFile(compressedText, exportFileInfo.FullName, string.Empty);
            }
            catch (Exception ex)
            {
                CallbackSetStatusMessage(ex.Message);
                info = new ExportInfo();
                info.Succeeded = false;
            }

            rowFilters.Clear();

            return info;
        }

        private void UpdateExportInfo(ExportInfo info)
        {
            if (info.Succeeded)
            {
                Properties.Settings.Default.DateLastExport = DateTime.Now;
                Properties.Settings.Default.UserLastExport = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                Properties.Settings.Default.RecordsLastExport = info.TotalRecordsPackaged;
                Properties.Settings.Default.Save();

                SetStatusMessage("Export complete. " + info.TotalRecordsPackaged.ToString() + " records exported.");
            }

            if (info.Succeeded)
            {
                Properties.Settings.Default.DateLastExport = info.ExportInitiated;
                Properties.Settings.Default.UserLastExport = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                Properties.Settings.Default.RecordsLastExport = info.TotalRecordsPackaged;
                Properties.Settings.Default.Save();
                SetStatusMessage("Export complete. " + info.TotalRecordsPackaged.ToString() + " records exported.");
            }
            else
            {
                SetStatusMessage("Export failed. " + info.TotalRecordsPackaged.ToString() + " records exported.");
            }

            //exportProgressBar.Value = 0;
            btnExport.IsEnabled = true;
            //listboxExportTypes.IsEnabled = true;
        }


        //private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        //{
        //    FileInfo fi = new FileInfo(DataHelper.Project.FilePath);
        //    DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(fi.Directory.FullName, Properties.Settings.Default.DataExportFolder));
        //    System.Diagnostics.Process.Start("explorer.exe", di.FullName);
        //}

        //Commented out the purge function below since there is now an Archive feature for the transmission tab.       
        //private void btnClean_Click(object sender, RoutedEventArgs e)
        //{
        //    FileInfo fi = new FileInfo(DataHelper.Project.FilePath);
        //    DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(fi.Directory.FullName, Properties.Settings.Default.DataExportFolder));

        //    DateTime now = DateTime.Now;
        //    DateTime minusThirty = now.AddDays(-30);

        //    List<FileInfo> filesToDelete = new List<FileInfo>();

        //    foreach (var file in di.GetFiles("*.edp7"))
        //    {
        //        if (file.CreationTime < minusThirty)
        //        {
        //            filesToDelete.Add(file);
        //        }
        //    }

        //    if (filesToDelete.Count > 0)
        //    {
        //        MessageBoxResult result = MessageBox.Show(filesToDelete.Count.ToString() + " transmission files are more than 30 days old and will be permanently deleted. Proceed?", "Proceed", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //        if (result == MessageBoxResult.Yes)
        //        {
        //            foreach (FileInfo fileInfo in filesToDelete)
        //            {
        //                try
        //                {
        //                    fileInfo.Delete();
        //                }
        //                catch
        //                { 
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("No transmission files are more than 30 days old. No files will be purged", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //}

        private void grdHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetHeight();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //if (Properties.Settings.Default.SystemLevel == 3)
            //{
            //    panelLabExportType.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    panelLabExportType.Visibility = System.Windows.Visibility.Collapsed;
            //}
            permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm"); //MLHIDE

            string SiteCodePerm_str = "3";

            if (permvar2 != null && permvar2.ToString() != "")
            {
                SiteCodePerm_str = permvar2.ToString();
            }

            object ExportOpt = EpiInfoMenuManager.GetPermanentVariableValue("PermExportOpt"); //MLHIDE

            if (ExportOpt != null)
            {
                if (ExportOpt.ToString() == "1" || (ExportOpt.ToString() == "0" && SiteCodePerm_str == "3"))
                {
                    DeidentifyBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    deidentify.Visibility = System.Windows.Visibility.Visible;
                }

                if (ExportOpt.ToString() == "1" || (ExportOpt.ToString() == "0" && SiteCodePerm_str == "3"))

                {
                    deidentify.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    deidentify.Visibility = System.Windows.Visibility.Visible;
                }

                if (ExportOpt.ToString() == "1" || (ExportOpt.ToString() == "0" && SiteCodePerm_str == "3"))

                {
                    AddResidents.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    AddResidents.Visibility = System.Windows.Visibility.Visible;
                }
            }

        }


        private void btnOpenModeEmploi(object sender, RoutedEventArgs e)
        {


            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            string fileName = "\\Resources\\ExportUserManual.pdf";

            string fullFileName = System.IO.Path.GetDirectoryName(a.Location) + fileName;
            if (System.IO.File.Exists(fullFileName))
            {
                string commandText = fullFileName;// System.IO.Path.GetDirectoryName(a.Location) + fileName; //"\\Projects\\Menafrinet\\Transmission des Données Mode d Emploi.pdf";

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = commandText;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            else
            {
                System.Windows.MessageBox.Show("Please contact the application developer if there is an error.");
            }

            //System.Diagnostics.Process.Start("AcroRd32.exe", "Transmission des Données Mode d Emploi.pdf");

        }

        //public void Button_Click(object sender, RoutedEventArgs e)
        // {
        //     //File.Copy("Menafrinet_NE.mdb", "Menafrinet_NEBak.mdb", true);
        //     //File.Copy("../../Epi Info 7/build/release/Projects/Menafrinet/Menafrinet_NE.mdb", "Backups/Menafrinet_NEBak.mdb", true);


        // }

        //private void btnCreateDBBackup_Click(object sender, RoutedEventArgs e)
        //{
        //FileInfo fi = new FileInfo(DataHelper.Project.FilePath);
        //DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(fi.Directory.FullName, Properties.Settings.Default.DataImportFolder));
        //System.Diagnostics.Process.Start("explorer.exe", di.FullName);

        //    //File.Copy("Menafrinet_NE.mdb", "Menafrinet_NEBak.mdb", true);
        //    File.Copy("Epi Info 7/build/release/Projects/Menafrinet/Menafrinet_NE.mdb", "Menafrinet_NEBak.mdb", true);

        //}


        public void PrepZip()
        {
            try
            {
                string connString =
                    "Provider=Microsoft.Jet.OLEDB.4.0" + ";data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.mdb"; //MLHIDE

                string UniqueGUID = "SELECT DISTINCT ActivityLog.GUID, ActivityLog.FileName, ActivityLog.CreatedDate FROM ActivityLog;"; //MLHIDE

                using (OleDbConnection connect = new OleDbConnection(connString))
                {
                    connect.Open();
                    OleDbCommand command = new OleDbCommand(UniqueGUID, connect); //MLHIDE
                    DataTable AttachGUIDUnique = new DataTable();
                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    adapter.SelectCommand = command;
                    adapter.Fill(AttachGUIDUnique);
                    adapter.Dispose();
                    //connect.Close();

                    foreach (DataRow UG in AttachGUIDUnique.Rows)
                    { //gfj0

                        if (permvar3.ToString() == "3")
                        {
                            selcriteria = "SELECT TOP 1 * FROM ActivityLog WHERE ActivityLog.FileName = '" + UG.ItemArray[1].ToString() + "' AND (ActivityLog.SentResident = 0 OR ActivityLog.SentResident = 1) AND ActivityLog.[GUID] = '" + UG.ItemArray[0].ToString() + "' ORDER BY ActivityLog.CreatedDate DESC;"; //MLHIDE
                        }

                        if (permvar3.ToString() == "2")
                        {
                            selcriteria = "SELECT TOP 1 * FROM ActivityLog WHERE ActivityLog.FileName = '" + UG.ItemArray[1].ToString() + "' AND (ActivityLog.SentPC = 0 OR ActivityLog.SentPC = 1) AND ActivityLog.[GUID] = '" + UG.ItemArray[0].ToString() + "' ORDER BY ActivityLog.CreatedDate DESC;"; //MLHIDE
                        }

                        if (permvar3.ToString() == "1")
                        {
                            selcriteria = "SELECT TOP 1 * FROM ActivityLog WHERE ActivityLog.FileName = '" + UG.ItemArray[1].ToString() + "' AND (ActivityLog.SentNational = 0 OR ActivityLog.SentNational = 1) AND ActivityLog.[GUID] = '" + UG.ItemArray[0].ToString() + "' ORDER BY ActivityLog.CreatedDate DESC;"; //MLHIDE
                        }

                        OleDbCommand command2 = new OleDbCommand(selcriteria, connect); //MLHIDE
                        DataTable AttachGUIDtab = new DataTable();
                        OleDbDataAdapter adapter2 = new OleDbDataAdapter();
                        adapter2.SelectCommand = command2;
                        adapter2.Fill(AttachGUIDtab);
                        adapter2.Dispose();
                        connect.Close();

                        if (AttachGUIDtab.Rows.Count > 0)
                        {
                            foreach (DataRow AttachGUID in AttachGUIDtab.Rows)
                            {
                                try
                                {

                                    if ((AttachGUID.ItemArray[4].ToString() == "3" && AttachGUID.ItemArray[22].ToString() == "0" && AttachGUID.ItemArray[27].ToString() == "0")
                                    ||
                                    (AttachGUID.ItemArray[4].ToString() == "2" && AttachGUID.ItemArray[16].ToString() == "0" && AttachGUID.ItemArray[30].ToString() == "0")
                                    ||
                                    (AttachGUID.ItemArray[4].ToString() == "1" && AttachGUID.ItemArray[33].ToString() == "0" && (((AttachGUID.ItemArray[16].ToString() == "0" || AttachGUID.ItemArray[16].ToString() == "0") && permvar3.ToString() == "1") || (AttachGUID.ItemArray[16].ToString() == "0" && permvar3.ToString() == "3") || AttachGUID.ItemArray[19].ToString() == "0" && permvar3.ToString() == "2")))
                                    {


                                        using (ZipFile zipGet = ZipFile.Read(appDir + "\\FA\\Attachments\\Attachments.zip")) //MLHIDE
                                        {
                                            foreach (ZipEntry ef in zipGet.Where(x => x.FileName.Contains(AttachGUID.ItemArray[0].ToString()) && x.FileName.Contains(AttachGUID.ItemArray[1].ToString())))
                                            {
                                                ef.Extract(appDir + "\\FA\\Attachment\\", ExtractExistingFileAction.OverwriteSilently); //MLHIDE
                                            }
                                        }

                                        using (ZipFile zipADD = new ZipFile(appDir + "\\Projects\\TrackingMaster\\ImportExport\\Attachments.zip")) //MLHIDE
                                        {
                                            var resultZip = zipADD.Any(entry => entry.FileName.Contains(AttachGUID.ItemArray[1].ToString()) && entry.FileName.Contains(AttachGUID.ItemArray[0].ToString()));

                                            if (resultZip == false)
                                            {   // TEST for BEST COMPRESSION GFJ0
                                                zipADD.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                                                zipADD.AddFile(appDir + "\\FA\\Attachment\\" + AttachGUID.ItemArray[0].ToString() + "\\" + AttachGUID.ItemArray[1].ToString(), AttachGUID.ItemArray[0].ToString()); //MLHIDE
                                                try
                                                {
                                                    zipADD.Save();
                                                    CallbackSetStatusMessage("Attachment: " + AttachGUID.ItemArray[1].ToString() + " included...");
                                                }
                                                catch (Exception)
                                                {
                                                    CallbackSetStatusMessage("ERROR: File attachment: " + AttachGUID.ItemArray[1].ToString() + " not found.");
                                                }

                                            }
                                            else
                                            {
                                                //zipADD.UpdateFile(rowFile.FullName.ToString(), rowFolder.Name.ToString());
                                                //zipADD.Save();
                                                //CallbackSetStatusMessage("Attachment: " + rowFile.Name.ToString() + " included...");
                                                //LogActivity(rowFolder.Name.ToString(), rowFile.Name.ToString(), DateTime.Now                          permvar2.ToString(), 1);
                                            }
                                        }

                                        SentNum = 1;
                                        SentCompleteTXT = null; //MLHIDE

                                        if (AttachGUID.ItemArray[19].ToString() == "1" && AttachGUID.ItemArray[22].ToString() == "1" && AttachGUID.ItemArray[4].ToString() == "3")
                                        {
                                            SentCompleteTXT = "SentResident = 2 "; //MLHIDE
                                        }

                                        if (AttachGUID.ItemArray[16].ToString() == "1" && AttachGUID.ItemArray[22].ToString() == "1" && AttachGUID.ItemArray[4].ToString() == "2")
                                        {
                                            SentCompleteTXT = "SentPC = 2 "; //MLHIDE
                                        }

                                        if (AttachGUID.ItemArray[16].ToString() == "1" && AttachGUID.ItemArray[19].ToString() == "1" && AttachGUID.ItemArray[4].ToString() == "1")
                                        {
                                            SentCompleteTXT = "SentNational = 2 "; //MLHIDE
                                        }
                                    }

                                    LogActExport(AttachGUID.ItemArray[0].ToString(), AttachGUID.ItemArray[1].ToString(), DateTime.Now, permvar2.ToString(), SentNum, AttachGUID.ItemArray[3].ToString());

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error: " + ex.ToString());
                                }
                            }
                        }
                    }
                }

                Directory.Delete(appDir + "\\FA\\Attachment\\", true); //MLHIDE
                Directory.CreateDirectory(appDir + "\\FA\\Attachment\\"); //MLHIDE

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

                MessageBox.Show(ml.ml_string(137, "An error occurred exporting in Export Block:") + "\n" + ex.Message + "\n" + "Line: " + line + "\n" + "Method: " + method + "\n" + "Path: " + path); //MLHIDE

                return;
            }
        }


        public void LogActExport(string AttachGUID, string fileName, DateTime SentDate, string UserName, int SentNum, string DCreated)
        {
            try
            {

                string connString =
                    "Provider=Microsoft.Jet.OLEDB.4.0" + ";data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.mdb"; //MLHIDE

                using (OleDbConnection connect = new OleDbConnection(connString))
                {
                    string SentLabel = null;

                    connect.Open();
                    //OleDbCommand command = new OleDbCommand("INSERT INTO ActivityLog ( SentDate, [GUID], CreatedBy, Sent, FileName )  VALUES (#" + SentDate + "#, '" + AttachGUID + "', '" + UserName + "', 1, '" + fileName + "');", connect);
                    //OleDbCommand command = new OleDbCommand("UPDATE ActivityLog SET [Sent] = '1', [SentDate] = #" + SentDate + "#, [SentBy] = '" + UserName + "' WHERE [Sent] = 0 AND [GUID] = '" + AttachGUID + "' AND [FileName] = '" + fileName + "'", connect);  //MLHIDE
                    //OleDbCommand command = new OleDbCommand("UPDATE(SELECT TOP 1 CreatedDate, Sent FROM ActivityLog WHERE [Sent] = 0 AND [GUID] = @AttachGUID AND [FileName] = @fileName ORDER BY CreatedDate DESC) AS a SET a.Sent = 1;", connect); //MLHIDE
                    //OleDbCommand command3 = new OleDbCommand("UPDATE(SELECT TOP 1 CreatedDate, Sent, SentBy FROM ActivityLog WHERE [Sent] = 0 AND [GUID] = @AttachGUID AND [FileName] = @fileName ORDER BY CreatedDate DESC) AS a SET a.Sent = 1, a.SentDate = #" + DateTime.Now + "#, SentBy = '" + permvar2 + "';", connect); //MLHIDE

                    if (permvar3.ToString() == "1")
                    {
                        SentLabel = "National"; //MLHIDE
                    }
                    if (permvar3.ToString() == "2")
                    {
                        SentLabel = "PC"; //MLHIDE
                    }
                    if (permvar3.ToString() == "3")
                    {
                        SentLabel = "Resident"; //MLHIDE
                    }


                    OleDbCommand command3 = new OleDbCommand("UPDATE ActivityLog SET ActivityLog.Sent" + SentLabel + " = " + SentNum + ", ActivityLog.SentBy" + SentLabel + " = '" + permvar2 + "', ActivityLog.[SentDate" + SentLabel + "] = #" + DateTime.Now + "# WHERE ActivityLog.[GUID]=[@AttachGUID] AND ActivityLog.[FileName]=[@fileName] AND ActivityLog.[CreatedDate]=#" + DCreated + "#; ", connect); //MLHIDE
                    command3.Parameters.AddWithValue("@AttachGUID", AttachGUID); //MLHIDE
                    command3.Parameters.AddWithValue("@fileName", fileName); //MLHIDE
                    command3.ExecuteNonQuery();

                    if (SentCompleteTXT != null)
                    {
                        command3 = new OleDbCommand("UPDATE ActivityLog SET " + SentCompleteTXT + " WHERE ActivityLog.[GUID]=[@AttachGUID] AND ActivityLog.[FileName]=[@fileName] AND ActivityLog.[CreatedDate]=#" + DCreated + "#; ", connect); //MLHIDE
                        command3.Parameters.AddWithValue("@AttachGUID", AttachGUID); //MLHIDE
                        command3.Parameters.AddWithValue("@fileName", fileName); //MLHIDE
                        command3.ExecuteNonQuery();
                        connect.Close();
                    }

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

                return;
            }
        }

        public void ExportLog()
        {
            try
            {

                Project project3 = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //MLHIDE
                IDbDriver db = project3.CollectedData.GetDatabase();

                string SelCriteriaLog = null;

                if (permvar3.ToString() == "3")
                {   // gfj0 check now
                    //SelCriteriaLog = "SELECT * FROM ActivityLog WHERE (ActivityLog.Received = 0) OR (ActivityLog.Received = 1) OR (ActivityLog.Sent = 2 AND ActivityLog.Received = 3) ORDER BY ActivityLog.SentDate DESC;";  //MLHIDE
                    //SelCriteriaLog = "SELECT * FROM ActivityLog WHERE ((ActivityLog.ReceivedPC = 0) OR (ActivityLog.ReceivedPC = 1) OR ((ActivityLog.SentNational = 1 AND ActivityLog.ReceivedNational = 2)) OR ((ActivityLog.ReceivedNational = 0) OR (ActivityLog.ReceivedNational = 1) OR (ActivityLog.SentNational = 1 AND ActivityLog.ReceivedNational = 2))) ORDER BY ActivityLog.SentDate DESC;";  //MLHIDE
                    //SelCriteriaLog = "SELECT * FROM ActivityLog WHERE (ActivityLog.ReceivedResident = 0 OR ActivityLog.ReceivedNational = 0);";  //MLHIDE
                    SelCriteriaLog = "SELECT * FROM ActivityLog WHERE (ActivityLog.ReceivedNational = 0);";  //MLHIDE
                }

                if (permvar3.ToString() == "2")
                {
                    //SelCriteriaLog = "SELECT * FROM ActivityLog WHERE (ActivityLog.ReceivedResident = 0 OR ActivityLog.ReceivedPC = 0 OR ActivityLog.ReceivedNational = 0);";  //MLHIDE
                    SelCriteriaLog = "SELECT * FROM ActivityLog WHERE (ActivityLog.ReceivedNational = 0);";  //MLHIDE
                }

                if (permvar3.ToString() == "1")
                {
                    SelCriteriaLog = "SELECT * FROM ActivityLog WHERE (ActivityLog.ReceivedResident = 0);";  //MLHIDE

                }

                Query selectQuery = DataHelper.Database.CreateQuery(SelCriteriaLog); //MLHIDE

                DataTable dt = DataHelper.Database.Select(selectQuery);

                string csv = string.Empty;

                //Add the Header row for CSV file.

                int cnt = 0;

                foreach (DataRow dRow in dt.Rows)
                {
                    csv += dt.Rows[cnt]["GUID"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["FileName"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["CreatedBy"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["CreatedDate"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["CreatedLevel"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["SentResident"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["SentByResident"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["SentDateResident"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["SentPC"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["SentByPC"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["SentDatePC"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["SentNational"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["SentByNational"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["SentDateNational"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["LastSentBy"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["DateLastSent"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["ReceivedResident"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["ReceivedByResident"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["ReceivedDateResident"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["ReceivedPC"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["ReceivedByPC"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["ReceivedDatePC"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["ReceivedNational"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["ReceivedByNational"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["ReceivedDateNational"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["ActivityComments"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["Institution"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["DeletedResident"].ToString() + ',';  // MLHIDE
                    csv += dt.Rows[cnt]["DeletedByResident"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["DeletedDateResident"].ToString() + ',';  // MLHIDE
                    csv += dt.Rows[cnt]["DeletedPC"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["DeletedByPC"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["DeletedDatePC"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["DeletedNational"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["DeletedByNational"].ToString().Replace(",", "|") + ','; // MLHIDE
                    csv += dt.Rows[cnt]["DeletedDateNational"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["GroupAttachment"].ToString() + ','; // MLHIDE
                    csv += dt.Rows[cnt]["ResidentAttchedby"].ToString().Replace(",", "|"); // MLHIDE
                    csv += "\r\n";

                    cnt = cnt + 1;
                }

                //Exporting to CSV
                File.WriteAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\ExportActivityLog.csv", csv); //MLHIDE

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
    }
}
