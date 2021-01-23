using MultiLang;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
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
using System.Xml;
using Menafrinet.ViewModel;
using Epi;
using Epi.Data;
using Epi.Fields;
using System.Data.OleDb;
using System.Threading;
using Epi.Menu;

namespace Menafrinet.View.Controls
{
    /// <summary>
    /// Interaction logic for DataEntryPanel.xaml
    /// </summary>
    public partial class DataEntryPanel : UserControl
    {

        string appDir = Environment.CurrentDirectory;

        public DataEntryPanel()
        {
            InitializeComponent();

            #region Populate column list from Xml document
            XmlDocument doc = new XmlDocument();

            doc.Load(appDir + "\\ColumnNames.xml"); //MLHIDE

            XmlNode root = doc.SelectSingleNode("Columns"); //MLHIDE

            string culture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();

            foreach (XmlNode node in root.ChildNodes)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                column.IsReadOnly = true;

                string fieldName = node.Attributes[0].Value;

                XmlNodeList nodes = node.SelectNodes("Translations/Translation");
                foreach (XmlNode transNode in nodes)
                {
                    if (transNode.Attributes[0].Value.Equals(culture))
                    {
                        column.Header = transNode.InnerText;
                        break;
                    }
                }

                Binding binding = new Binding(fieldName);
                if (fieldName.StartsWith("Date"))
                {
                    binding.StringFormat = "dd/MM/yyyy"; //MLHIDE CAN FIXED DATE FORMAT
                }
                column.Binding = binding;
                dgRecords.Columns.Add(column);
            }
            #endregion // Populate column list from Xml document
        }

        int Uniquekey;
        bool eventhandled;
        int initiated;
        int elapsedTime;



        public DataHelper DataHelper
        {
            get
            {
                return this.DataContext as DataHelper;
            }
        }

        public IEnumerable<object> Database { get; private set; }

        private void btnPrintRecords_Click(object sender, RoutedEventArgs e)
        {
            string baseFileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString("N"); //MLHIDE

            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append(Menafrinet.Core.Common.GetHtmlHeader().ToString());

            htmlBuilder.AppendLine("<table width=\"100%\" style=\"border: 0px; padding: 0px; margin: 0px; clear: left; width:100%; \">"); //MLHIDE
            htmlBuilder.AppendLine(" <tr style=\"border: 0px;\">"); //MLHIDE
            htmlBuilder.AppendLine("  <td width=\"50%\" style=\"border: 0px;\">"); //MLHIDE
            htmlBuilder.AppendLine(ml.ml_string(157, "   <p style=\"font-size: 13pt; font-weight: bold; clear: left; text-decoration: underline;\">Resident Driven Project Tracking Tool (ReDPeTT)</p>"));
            htmlBuilder.AppendLine(ml.ml_string(156, "   <p style=\"font-size: 13pt; font-weight: bold; text-decoration: underline;\">Project Line List</p>"));
            htmlBuilder.AppendLine(ml.ml_string(155, "   <p style=\"font-size: 13pt; font-weight: bold;\">Date printed: ") + DateTime.Now.ToShortDateString() + "</p>");

            if (!String.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                htmlBuilder.AppendLine(ml.ml_string(168, "   <p style=\"font-size: 13pt; font-weight: bold;\">Filter: ") + txtSearch.Text.Trim() + "</p>");
            }

            htmlBuilder.AppendLine("  </td>"); //MLHIDE
            htmlBuilder.AppendLine(" </tr>"); //MLHIDE
            htmlBuilder.AppendLine("</table>"); //MLHIDE


            htmlBuilder.AppendLine("<p style=\"font-weight: bold; clear: left;\">&nbsp;</p>"); //MLHIDE

            htmlBuilder.AppendLine("<table style=\"width: 1200px; border: 4px solid black;\" align=\"left\">"); //MLHIDE
            htmlBuilder.AppendLine("<thead>"); //MLHIDE
            htmlBuilder.AppendLine("<tr style=\"border-top: 0px solid black;\">"); //MLHIDE
            htmlBuilder.AppendLine(ml.ml_string(153, "<th style=\"width: 180px;\">Project Title</th>"));
            htmlBuilder.AppendLine(ml.ml_string(146, "<th style=\"width: 140px;\">Date Assigned</th>"));
            htmlBuilder.AppendLine(ml.ml_string(147, "<th style=\"width: 140px;\">Lead Resident</th>"));
            htmlBuilder.AppendLine(ml.ml_string(148, "<th style=\"width: 80px;\">Lead Cohort</th>"));
            htmlBuilder.AppendLine("<th style=\"width: 100px;\">Attachment</th>"); //MLHIDE
            htmlBuilder.AppendLine(ml.ml_string(149, "<th style=\"width: 90px;\">Mentor Full Name</th>"));
            htmlBuilder.AppendLine(ml.ml_string(150, "<th style=\"width: 90px;\">Supervisor Full Name</th>"));
            htmlBuilder.AppendLine(ml.ml_string(191, "<th style=\"width: 90px;\">Project Status</th>"));
            //htmlBuilder.AppendLine("<th style=\"width: 100px;\">Health Facility</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 140px;\">District</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 30px;\">Year</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 30px;\">Epi<br/>Week</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 100px;\">Classification</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 100px;\">Outcome</th>");
            //Added Created
            htmlBuilder.AppendLine(ml.ml_string(151, "<th style=\"width: 60px;\">Created</th>"));
            //Added Updated
            htmlBuilder.AppendLine(ml.ml_string(178, "<th style=\"width: 60px;\">Updated</th>"));
            htmlBuilder.AppendLine("</tr>"); //MLHIDE
            htmlBuilder.AppendLine("</thead>"); //MLHIDE

            htmlBuilder.AppendLine("<tbody>"); //MLHIDE

            foreach (CaseViewModel caseVM in DataHelper.FilteredCaseCollection)
            {
                if (caseVM.RecStatus.ToString() == "1")
                {
                    htmlBuilder.AppendLine("<tr>"); //MLHIDE
                }
                else
                {
                    htmlBuilder.AppendLine("<tr bgcolor = Lightgrey>"); //MLHIDE
                }
                htmlBuilder.AppendLine("<td>" + caseVM.projecttitle + "</td>"); //MLHIDE
                htmlBuilder.AppendLine("<td>" + caseVM.DateAssigned + "</td>"); //MLHIDE
                htmlBuilder.AppendLine("<td>" + caseVM.LeadResident + "</td>"); //MLHIDE
                htmlBuilder.AppendLine("<td>" + caseVM.leadcohort + "</td>"); //MLHIDE
                htmlBuilder.AppendLine("<td>" + caseVM.HasAttachment + "</td>"); //MLHIDE
                htmlBuilder.AppendLine("<td>" + caseVM.MentorFullName + "</td>"); //MLHIDE

                //if (caseVM.AgeYears.HasValue)
                //{
                //    htmlBuilder.AppendLine("<td align=\"right\">" + caseVM.AgeYears.ToString() + "</td>");
                //}
                //else
                //{
                //    htmlBuilder.AppendLine("<td>&nbsp;</td>");
                //}
                htmlBuilder.AppendLine("<td>" + caseVM.SupervisorFullName + "</td>"); //MLHIDE
                htmlBuilder.AppendLine("<td>" + caseVM.ProjectStatus + "</td>"); //MLHIDE

                //htmlBuilder.AppendLine("<td>" + caseVM.DistrictReporting + "</td>");
                //if (caseVM.Year.HasValue)
                //{
                //    htmlBuilder.AppendLine("<td align=\"right\">" + caseVM.Year.ToString() + "</td>");
                //}
                //else
                //{
                //    htmlBuilder.AppendLine("<td>&nbsp;</td>");
                //}
                //if (caseVM.EpiWeek.HasValue)
                //{
                //    htmlBuilder.AppendLine("<td align=\"right\">" + caseVM.EpiWeek.ToString() + "</td>");
                //}
                //else
                //{
                //    htmlBuilder.AppendLine("<td>&nbsp;</td>");
                //}
                //htmlBuilder.AppendLine("<td>" + caseVM.Classification + "</td>");
                //htmlBuilder.AppendLine("<td>" + caseVM.Outcome + "</td>");

                if (caseVM.DateRecordCreated.HasValue)
                {
                    htmlBuilder.AppendLine("<td align=\"right\">" + caseVM.DateRecordCreated.Value.ToShortDateString() + "</td>"); //MLHIDE
                }
                else
                {
                    htmlBuilder.AppendLine("<td>&nbsp;</td>"); //MLHIDE
                }

                if (caseVM.DateRecordUpdated.HasValue)
                {
                    htmlBuilder.AppendLine("<td align=\"right\">" + caseVM.DateRecordUpdated.Value.ToShortDateString() + "</td>"); //MLHIDE
                }
                else
                {
                    htmlBuilder.AppendLine("<td>&nbsp;</td>"); //MLHIDE
                }

                htmlBuilder.AppendLine("</tr>"); //MLHIDE
            }

            htmlBuilder.AppendLine("</tbody>"); //MLHIDE
            htmlBuilder.AppendLine("</table>"); //MLHIDE

            string fileName = baseFileName + ".html"; //MLHIDE

            System.IO.FileStream fstream = System.IO.File.OpenWrite(fileName);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fstream);
            sw.WriteLine(htmlBuilder.ToString());
            sw.Close();
            sw.Dispose();

            if (!string.IsNullOrEmpty(fileName))
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "\"" + fileName + "\"";
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
        }

        private void btnNewRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnNewRecord.IsEnabled = false;
                Cursor = Cursors.Wait;

                //Epi.Enter.EnterUIConfig uiConfig = Core.Common.GetNewCaseConfig(DataHelper.CaseForm);
                //Epi.Windows.Enter.EnterMainForm emf = new Epi.Windows.Enter.EnterMainForm(DataHelper.Project, DataHelper.CaseForm, uiConfig);
                //emf.RecordSaved += new SaveRecordEventHandler(emfCases_RecordSaved);
                //emf.ShowDialog();
                //emf.RecordSaved -= new SaveRecordEventHandler(emfCases_RecordSaved);

                System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
                string appDir = Environment.CurrentDirectory;
                //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
                string commandText = appDir + "\\Enter.exe"; //MLHIDE

                if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj")) //MLHIDE
                {
                    eventhandled = false;
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = commandText;
                    proc.Exited += new EventHandler(RefreshDatagrid);
                    proc.EnableRaisingEvents = true;
                    proc.StartInfo.Arguments = "/project: " + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj /view:projects /record:* /title:ReDPPeT Projects Entry"; //MLHIDE
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                    //proc.WaitForExit();

                }

                const int SleepAmount = 500;
                while (!eventhandled)
                {
                    //elapsedTime += SleepAmount;
                    //if (elapsedTime > 630000)
                    //{
                    //    MessageBox.Show("Times UP!");
                    //    break;
                    //}

                    Thread.Sleep(SleepAmount); // Wait, and...
                }

                //MessageBox.Show("Jump to repop already!");

                //Epi.Menu.EpiInfoMenuManager.SetPermanentVariable("Permanent_Date_Beg", datePicker1_str);

                HasAttachment();

                DataHelper.RepopulateCollections();

                //Application.Current.MainWindow.Activate(); // Try to focus
                //Application.Current.MainWindow.Focus(); // Focus
                //Application.Current.MainWindow.UpdateLayout(); // ForceUpdate Layout for the entire Window
                // clear outthe 

                dgRecords.Items.Refresh();
                dgRecords.UpdateLayout();

                this.Cursor = Cursors.Arrow;
                btnNewRecord.IsEnabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("An exception occured while opening a record. Case ID: {0}. Please give this message to the application developer.\n", ex.Message));
            }
        }

        void RefreshDatagrid(object sender, EventArgs e)
        {
            eventhandled = true;
        }


        public void emfCases_RecordSaved(object sender, SaveRecordEventArgs e)
        {
            //RefreshCases();
            string caseGuid = e.RecordGuid;
            if (e.Form == DataHelper.CaseForm)
            {
                ((this.DataContext) as DataHelper).UpdateOrAddCase.Execute(caseGuid);
            }
        }

        private void dgRecords_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dgRecords.SelectedItems.Count == 1)
                {

                    Cursor = Cursors.Wait;

                    IInputElement element = e.MouseDevice.DirectlyOver;
                    if (element != null && element is FrameworkElement)
                    {
                        if (((FrameworkElement)element).Parent is DataGridCell)
                        {
                            EditCase();
                        }
                    }

                    this.Cursor = Cursors.Arrow;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("An exception occured while opening a record. Case ID: {0}. Please give this message to the application developer.\n", ex.Message));
            }
        }

        private void EditCase()
        {
            //Epi.Enter.EnterUIConfig uiConfig = Core.Common.GetExistingCaseConfig(DataHelper.CaseForm);
            //Epi.Windows.Enter.EnterMainForm emf = new Epi.Windows.Enter.EnterMainForm(DataHelper.Project, DataHelper.CaseForm, uiConfig);

            int uniqueKey = ((CaseViewModel)dgRecords.SelectedItem).Case.UniqueKey;

            //emf.LoadRecord(uniqueKey);
            //emf.RecordSaved += new SaveRecordEventHandler(emfCases_RecordSaved);
            //emf.ShowDialog();
            //emf.RecordSaved -= new SaveRecordEventHandler(emfCases_RecordSaved);

            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            string appDir = Environment.CurrentDirectory;
            //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
            string commandText = appDir + "\\Enter.exe"; //MLHIDE

            if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj")) //MLHIDE
            {
                eventhandled = false;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = commandText;
                proc.Exited += new EventHandler(RefreshDatagrid);
                proc.EnableRaisingEvents = true;
                proc.StartInfo.Arguments = "/project: " + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj /view:projects /record:" + uniqueKey; //MLHIDE
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
                //proc.WaitForExit();


                const int SleepAmount = 500;
                while (!eventhandled)
                {
                    Thread.Sleep(SleepAmount); // Wait, and...
                }

                dgRecords.Items.Refresh();
                dgRecords.UpdateLayout();
                //SetAlert(uniqueKey);

                HasAttachment();

                DataHelper.RepopulateCollections(); //gfj

            }


            this.Cursor = Cursors.Arrow;
        }

        internal void ResetHeight()
        {
            dgRecords.Visibility = System.Windows.Visibility.Collapsed;

            grdMain.UpdateLayout();

            double maxHeight = grdMain.ActualHeight;
            maxHeight = maxHeight - 380;

            if (maxHeight <= 0) maxHeight = 0;

            dgRecords.MaxHeight = maxHeight;
            dgRecords.Height = maxHeight;
            dgRecords.Visibility = System.Windows.Visibility.Visible;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetHeight();
        }

        private void CaseActionsRowControl_DeleteCaseRequested(object sender, EventArgs e)
        {
            CaseViewModel caseVM = ((sender as CaseActionsRowControl).DataContext as CaseViewModel);
            DeleteCase(caseVM);
        }

        private void DeleteCase(CaseViewModel caseVM)
        {
            System.Windows.Forms.DialogResult result = Epi.Windows.MsgBox.ShowQuestion("Are you sure you wish to delete this case?");
            if (result.Equals(System.Windows.Forms.DialogResult.Yes))
            {
                ////Added to password protect delete cases from grid.
                //if (result.Equals(System.Windows.Forms.DialogResult.Yes))
                //{
                //    System.Windows.Forms.DialogResult verify = Epi.Windows.MsgBox.ShowQuestion("Please enter your password.");
                //}
                try
                {
                    string caseGuid = caseVM.Guid;
                    ((this.DataContext) as DataHelper).DeleteCase.Execute(caseGuid);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("An exception occured while trying to delete a case. Case ID: {0}. Please give this message to the application developer.\n{1}", caseVM.EPID, ex.Message));
                }
            }
        }

        private void dgRecords_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OverviewRefresh();
        }

        private void dgRecords_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //dgRecords.UpdateLayout();

            OverviewRefresh();

        }

        private void dgRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OverviewRefresh();
        }

        private void dgRecords_SelectionChangedgfj(object sender, SelectionChangedEventArgs e)
        {
            //dgRecords.UpdateLayout();

            OverviewRefresh();
        }

        //private void btnDeleteRecord_Click(object sender, RoutedEventArgs e)
        //{
        //    foreach (System.Windows.Forms.DataGridViewRow item in this.dgRecords.SelectedItems)
        //    {
        //        dgRecords.Rows.RemoveAt(item.Index);
        //    }

        //}
        string resText = null;

        private void OverviewRefresh()
        {
            //try
            //{

            string appDir = Environment.CurrentDirectory;

            int UniqueID = 1;

            if ((CaseViewModel)dgRecords.SelectedItem == null)
            {
                ProjecClassificationtLabel.Text = null;
                ICD10CodeLabel.Text = null;
                ICD10NameLabel.Text = null;
                RecordEnteredbyLabel.Text = null;
                DiseaseCategorySuspectedLabel.Text = null;
                ProjectType.Text = null;
                UniversityInstitution.Text = null;
                Setting.Text = null;
                City.Text = null;
                SuspectedDiseaseLabel.Text = null;
                ResidentsAssignedtoProject.Text = null;
                //HasAttachmentLabel.Text = null;
                return;
            }
            else
            {
                UniqueID = ((CaseViewModel)dgRecords.SelectedItem).Case.UniqueKey;

            }

            ResidentNames(UniqueID);

            string connString = "Provider=Microsoft.Jet.OLEDB.4.0" + ";data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.mdb"; //MLHIDE

            //string queryString = "SELECT Projects.*, Projects4.* FROM Projects INNER JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId;";

            string queryString = "SELECT Projects.*, Projects4.*, (SELECT TOP 1 [codetype2].[type] FROM codetype2 WHERE[Projects4].[Type] = Left([codetype2].[type],2) AND [codetype2].[type] = [Projects4].[Type] & Mid([codetype2]![type],3)) AS TypeLabel, (SELECT TOP 1 [codeprojectclassification1].[projectclassification] FROM codeprojectclassification1 WHERE[Projects4].[projectclassification ] = Left([codeprojectclassification1].[projectclassification],2) AND[codeprojectclassification1].[projectclassification] = [Projects4].[projectclassification ] & Mid([codeprojectclassification1]![projectclassification],3)) AS ProjectClassificationLabel, (SELECT TOP 1 [codesetting1].[setting] FROM codesetting1 WHERE[Projects4].[setting] = Left( [codesetting1].[setting],3) AND [codesetting1].[setting] = [Projects4].[setting] & Mid([codesetting1]![setting],4)) AS SettingLabel FROM Projects INNER JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId WHERE(((Projects.[UniqueKey]) = " + UniqueID + "));"; //MLHIDE

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                OleDbCommand cmd = new OleDbCommand(queryString, conn);

                conn.Open();

                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // GFJ
                    ProjecClassificationtLabel.Text = null;
                    ICD10CodeLabel.Text = null;
                    ICD10NameLabel.Text = null;
                    RecordEnteredbyLabel.Text = null;
                    DiseaseCategorySuspectedLabel.Text = null;
                    ProjectType.Text = null;
                    UniversityInstitution.Text = null;
                    Setting.Text = null;
                    City.Text = null;
                    SuspectedDiseaseLabel.Text = null;
                    ResidentsAssignedtoProject.Text = null;
                    //HasAttachmentLabel.Text = null;


                    if (!string.IsNullOrEmpty((string)reader["ProjectClassificationLabel"].ToString()))
                    {
                        ProjecClassificationtLabel.Text = (string)reader["ProjectClassificationLabel"];
                    }
                    if (!string.IsNullOrEmpty((string)reader["ICD10CodeSuspected"].ToString()))
                    {
                        ICD10CodeLabel.Text = (string)reader["ICD10CodeSuspected"];
                    }
                    if (!string.IsNullOrEmpty((string)reader["GeneralDiseaseNameSuspected"].ToString()))
                    {
                        ICD10NameLabel.Text = (string)reader["GeneralDiseaseNameSuspected"];
                    }
                    if (!string.IsNullOrEmpty((string)reader["RecordEnteredBy"].ToString()))
                    {
                        RecordEnteredbyLabel.Text = (string)reader["RecordEnteredBy"];
                    }
                    if (!string.IsNullOrEmpty((string)reader["DiseaseCategorySuspected"].ToString()))
                    {
                        DiseaseCategorySuspectedLabel.Text = (string)reader["DiseaseCategorySuspected"];
                    }
                    if (!string.IsNullOrEmpty((string)reader["TypeLabel"].ToString()))
                    {
                        ProjectType.Text = (string)reader["TypeLabel"];
                    }
                    if (!string.IsNullOrEmpty((string)reader["UniversityTrainingInstitution"].ToString()))
                    {
                        UniversityInstitution.Text = (string)reader["UniversityTrainingInstitution"];
                    }
                    if (!string.IsNullOrEmpty((string)reader["SettingLabel"].ToString()))
                    {
                        Setting.Text = (string)reader["SettingLabel"];
                    }
                    if (!string.IsNullOrEmpty((string)reader["City"].ToString()))
                    {
                        City.Text = (string)reader["City"];
                    }
                    if (!string.IsNullOrEmpty((string)reader["DiseaseSuspected"].ToString()))
                    {
                        SuspectedDiseaseLabel.Text = (string)reader["DiseaseSuspected"];
                    }
                    if (resText != null)
                    {
                        ResidentsAssignedtoProject.Text = resText;
                    }
                    //if (!string.IsNullOrEmpty((string)reader["HasAttachment"].ToString()))
                    //{
                    //    HasAttachmentLabel.Text = (string)reader["HasAttachment"];
                    //}

                    // *** ===================== OLD Definition ====================***
                    //ProjecClassificationtLabel.Text = reader.GetValue(50).ToString();
                    //ICD10CodeLabel.Text = reader.GetValue(36).ToString();
                    //ICD10NameLabel.Text = reader.GetValue(33).ToString();
                    //RecordEnteredbyLabel.Text = reader.GetValue(37).ToString();
                    //DiseaseCategorySuspectedLabel.Text = reader.GetValue(39).ToString(); // Done here
                    //ProjectType.Text = reader.GetValue(49).ToString();
                    //UniversityInstitution.Text = reader.GetValue(35).ToString();
                    //Setting.Text = reader.GetValue(51).ToString();
                    //City.Text = reader.GetValue(24).ToString();
                    //SuspectedDiseaseLabel.Text = reader.GetValue(23).ToString();
                }

                reader.Close();
                conn.Close();

            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("An error occurred with overview" + ex.Message);
            //    return;
            //}
        }



        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            DataTable results;

            object permAlert = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm");

            if ((CaseViewModel)dgRecords.SelectedItem != null)
            {

                int Uniquekey = ((CaseViewModel)dgRecords.SelectedItem).Case.UniqueKey;

                //SetAlert(Uniquekey);

                string appDir = Environment.CurrentDirectory;

                string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.mdb";

                results = new DataTable();


                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM Projects INNER JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId WHERE (((Projects.UniqueKey)=" + Uniquekey + "));", conn);

                    conn.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

                    //if (adapter == null)

                    adapter.Fill(results);

                    int i = results.Rows.Count;

                    if (results.Rows.Count > 0)
                    {
                        OleDbConnection connect = new OleDbConnection();
                        connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB"; //MLHIDE
                        //connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Projects\TrackingMaster\TrackingMasterNew.MDB";
                        connect.Open();
                        OleDbCommand ChkCommand = new OleDbCommand();
                        ChkCommand.Connection = connect;
                        //ChkCommand.CommandText = "UPDATE Projects INNER JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId SET Projects4.LastUpdate = " + permAlert.ToString() + " WHERE(((Projects.UniqueKey) = " + Uniquekey + "));";
                        //ChkCommand.CommandText = "UPDATE Projects INNER JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId SET [Projects4].[Reviewed] = 1 WHERE([Projects].[UniqueKey] = " + Uniquekey + ");";
                        ChkCommand.CommandText = "UPDATE Projects INNER JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId SET [Projects4].[Reviewed] =" + Convert.ToInt32(permAlert.ToString()) + " WHERE(((Projects.UniqueKey) = " + Uniquekey + "));"; //MLHIDE
                        ChkCommand.ExecuteNonQuery();
                        connect.Close();
                    }

                }

                //SetAlert2(Uniquekey, Convert.ToInt32(permAlert.ToString()));

                DataHelper.RepopulateCollections();

            }

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string appDir = Environment.CurrentDirectory;

            DataTable results;

            if ((CaseViewModel)dgRecords.SelectedItem != null)
            {

                int Uniquekey = ((CaseViewModel)dgRecords.SelectedItem).Case.UniqueKey;

                appDir = Environment.CurrentDirectory;

                string connString =
                    "Provider=Microsoft.Jet.OLEDB.4.0" +
                    ";data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.mdb";

                results = new DataTable();


                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM Projects INNER JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId WHERE (((Projects.UniqueKey)=" + Uniquekey + "));", conn);

                    conn.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

                    //if (adapter == null)

                    adapter.Fill(results);

                    int i = results.Rows.Count;

                    if (results.Rows.Count > 0)
                    {
                        OleDbConnection connect = new OleDbConnection();
                        connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB"; //MLHIDE
                        //connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Projects\TrackingMaster\TrackingMasterNew.MDB";
                        connect.Open();
                        OleDbCommand ChkCommand = new OleDbCommand();
                        ChkCommand.Connection = connect;
                        ChkCommand.CommandText = "UPDATE Projects INNER JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId SET [Projects4].[Reviewed] = 0 WHERE([Projects].[UniqueKey] = " + Uniquekey + ");"; //MLHIDE
                        ChkCommand.ExecuteNonQuery();
                        connect.Close();
                    }

                }

                //SetAlert2(Uniquekey, 0);

                DataHelper.RepopulateCollections();
                var instance = new AnalysisPanel();
                instance.PopulateFileNames();

            }

        }

        public void SetAlert(int Uniquekey)
        {

            DataTable results;

            object permAlert = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm");


            //if ((CaseViewModel)dgRecords.SelectedItem != null)
            //{

            //Uniquekey = ((CaseViewModel)dgRecords.SelectedItem).Case.UniqueKey;

            string appDir = Environment.CurrentDirectory;

            string connString =
                "Provider=Microsoft.Jet.OLEDB.4.0" +
                ";data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.mdb";

            results = new DataTable();


            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM Projects INNER JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId WHERE (((Projects.UniqueKey)=" + Uniquekey + "));", conn);

                conn.Open();

                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

                //if (adapter == null)

                adapter.Fill(results);

                int i = results.Rows.Count;

                if (results.Rows.Count > 0)
                {

                    OleDbConnection connect = new OleDbConnection();
                    connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB"; //MLHIDE
                    connect.Open();
                    OleDbCommand ChkCommand = new OleDbCommand();
                    ChkCommand.Connection = connect;
                    //ChkCommand.CommandText = "UPDATE StatusTab41 SET [REVIEWED]=" + Convert.ToInt32(permAlert.ToString()) + " WHERE (((StatusTab41.UniqueID)=" + Uniquekey + "));";
                    ChkCommand.CommandText = "UPDATE Projects INNER JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId SET [Projects4].[LastUpdate] ='0' WHERE(((Projects.UniqueKey) = " + Uniquekey + "));"; //MLHIDE
                    ChkCommand.ExecuteNonQuery();
                    connect.Close();
                }
            }

            //DataHelper.RepopulateCollections();

            //}
        }


        public void SetAlert2(int Uniquekey, bool chk_unchk)
        {

            DataTable results;

            if ((CaseViewModel)dgRecords.SelectedItem != null)
            {

                Uniquekey = ((CaseViewModel)dgRecords.SelectedItem).Case.UniqueKey;

                string appDir = Environment.CurrentDirectory;

                string connString =
                    "Provider=Microsoft.Jet.OLEDB.4.0" +
                    ";data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.mdb";

                results = new DataTable();


                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM StatusTab41 WHERE UniqueID=" + Uniquekey + ";", conn);

                    conn.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

                    //if (adapter == null)

                    adapter.Fill(results);

                    int i = results.Rows.Count;

                    if (results.Rows.Count > 0)
                    {
                        var SetRow = results.Rows[0];
                        OleDbConnection connect = new OleDbConnection();
                        connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB"; //MLHIDE
                        connect.Open();
                        OleDbCommand ChkCommand = new OleDbCommand();
                        ChkCommand.Connection = connect;
                        ChkCommand.CommandText = "UPDATE StatusTab41 SET [REVIEWED]=" + chk_unchk + " WHERE (((StatusTab41.UniqueID)=" + Uniquekey + "));"; //MLHIDE
                        ChkCommand.ExecuteNonQuery();
                        connect.Close();
                    }
                    else
                    {
                        //var SetRow = results.Rows[0];
                        OleDbConnection connect = new OleDbConnection();
                        connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB"; //MLHIDE
                        connect.Open();
                        OleDbCommand ChkCommand = new OleDbCommand();
                        ChkCommand.Connection = connect;
                        ChkCommand.CommandText = "INSERT INTO StatusTab41 ( Reviewed, UniqueID )  VALUES (" + chk_unchk + ", " + Uniquekey + "); "; //MLHIDE
                        ChkCommand.ExecuteNonQuery();
                        connect.Close();
                    }

                }

                //DataHelper.RepopulateCollections();

            }
        }

        private void DelRecDataEntry_Checked(object sender, RoutedEventArgs e)
        {
            DataHelper.DelRecStatus = true;
            DataHelper.RepopulateCollections();
        }

        private void DelRecDataEntry_Unchecked(object sender, RoutedEventArgs e)
        {
            DataHelper.DelRecStatus = false;
            DataHelper.RepopulateCollections();
        }

        private void CurrentCohort_Checked(object sender, RoutedEventArgs e)
        {
            DataHelper.CurCohortStatus = true;
            EpiInfoMenuManager.CreatePermanentVariable("CohortYNPerm", 2); //MLHIDE
            EpiInfoMenuManager.SetPermanentVariable("CohortYNPerm", true); //MLHIDE
            DataHelper.RepopulateCollections();
        }

        private void CurrentCohort_Unchecked(object sender, RoutedEventArgs e)
        {
            DataHelper.CurCohortStatus = false;
            EpiInfoMenuManager.CreatePermanentVariable("CohortYNPerm", 2); //MLHIDE
            Epi.Menu.EpiInfoMenuManager.SetPermanentVariable("CohortYNPerm", false); //MLHIDE
            DataHelper.RepopulateCollections();
        }


        private void ResidentNames(int UniqueID)
        {

            string appDir = Environment.CurrentDirectory;
            int counter = 0;
            int cnt = 0;
            string GlobalID = null;

            OleDbConnection connect = new OleDbConnection();
            //OleDbCommand DelCommand2 = new OleDbCommand("select * from [ReportTable]", connect); //MLHIDE
            //DelCommand2.CommandText = "Delete * from ResidentNamesTab;"; //MLHIDE
            //DelCommand2.ExecuteNonQuery();

            OleDbCommand command2 = new OleDbCommand();
            command2.Connection = connect;

            Project project4 = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //MLHIDE
            IDbDriver db = project4.CollectedData.GetDatabase(); //MLHIDE
            System.Data.DataTable result = db.GetTableData("Query23"); //MLHIDE
            DataRow[] dt = result.Select("UniqueKey=" + UniqueID); //MLHIDE

            foreach (DataRow dRow in dt)
            {
                string delStatus = dt[cnt]["RECSTATUS"].ToString(); //MLHIDE
                string prjStatus = dt[cnt]["GlobalRecordId"].ToString(); //MLHIDE
                string ResidentNames = null;

                if (delStatus != "0")
                {

                    int cnt2 = 0;
                    Project project2 = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //MLHIDE
                    IDbDriver db2 = project2.CollectedData.GetDatabase();
                    System.Data.DataTable dt2 = db.GetTableData("Trainees"); //MLHIDE


                    foreach (DataRow dRow2 in dt2.Rows)
                    {
                        string GlobalCompare = dt[cnt]["GlobalRecordId"].ToString(); //MLHIDE
                        string FKEYCompare = dt2.Rows[cnt2]["FKEY"].ToString(); //MLHIDE
                        if (GlobalCompare == FKEYCompare)
                        {
                            int cnt3 = 0;
                            Project project3 = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //MLHIDE
                            IDbDriver db3 = project3.CollectedData.GetDatabase(); //MLHIDE
                            System.Data.DataTable dt3 = db.GetTableData("Query5"); //MLHIDE


                            foreach (DataRow dRow3 in dt3.Rows)
                            {
                                string GlobalCompare2 = dt2.Rows[cnt2]["GlobalRecordId"].ToString(); //MLHIDE
                                string FKEYCompare2 = dt3.Rows[cnt3]["GlobalRecordId"].ToString(); //MLHIDE

                                int RecStat = Convert.ToInt32(dt3.Rows[cnt3]["Recstatus"].ToString()); //MLHIDE

                                //if (GlobalCompare2 == FKEYCompare2 && RecStat != 0)
                                if (GlobalCompare2 == FKEYCompare2 && RecStat != 0)
                                {
                                    counter = counter + 1;
                                    GlobalID = GlobalCompare;
                                    string ResName = dt3.Rows[cnt3]["ResidentName2"].ToString(); //MLHIDE
                                    ResidentNames = ResidentNames + ResName + "; ";
                                    //MessageBox.Show(GlobalCompare + " - " + FKEYCompare + " - " + ResidentNames);
                                    break;
                                }

                                cnt3 = cnt3 + 1;

                            }

                        }

                        cnt2 = cnt2 + 1;
                        resText = ResidentNames;
                    }

                }

            }

            connect.Close();
        }

        public void HasAttachment()
        {
            var permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm"); //MLHIDE

            string UserSettingPERM = null;
            string DelLevel = null;

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

                if (permvar2.ToString() == "1")
                {
                    DelLevel = "National"; //MLHIDE
                }
                if (permvar2.ToString() == "2")
                {
                    DelLevel = "PC"; //MLHIDE
                }
                if (permvar2.ToString() == "3")
                {
                    DelLevel = "Resident"; //MLHIDE
                }
            }

            OleDbConnection connect = new OleDbConnection();

            connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB";   //MLHIDE

            connect.Open(); // ATTACHMENT 

            OleDbCommand command = new OleDbCommand();

            command.Connection = connect;

            //command.CommandText = "UPDATE Projects4 INNER JOIN(Projects INNER JOIN ((Requiredwrittenmaterials INNER JOIN Requiredwrittenmaterials13 ON Requiredwrittenmaterials.GlobalRecordId = Requiredwrittenmaterials13.GlobalRecordId) INNER JOIN ActivityLog ON Requiredwrittenmaterials13.AttachGUID = ActivityLog.GUID) ON Projects.GlobalRecordId = Requiredwrittenmaterials.FKEY) ON Projects4.GlobalRecordId = Projects.GlobalRecordId SET Projects4.HasAttachment = 'Yes'";   //MLHIDE

            command.CommandText = "UPDATE Projects4 INNER JOIN(Projects INNER JOIN ((Requiredwrittenmaterials INNER JOIN Requiredwrittenmaterials13 ON Requiredwrittenmaterials.GlobalRecordId = Requiredwrittenmaterials13.GlobalRecordId) INNER JOIN ActivityLog ON Requiredwrittenmaterials13.AttachGUID = ActivityLog.GUID) ON Projects.GlobalRecordId = Requiredwrittenmaterials.FKEY) ON Projects4.GlobalRecordId = Projects.GlobalRecordId SET Projects4.HasAttachment = iif(ActivityLog.DeletedBy" + DelLevel + " = '" + UserSettingPERM + "', '', 'Yes');";   //MLHIDE

            command.ExecuteNonQuery();

            connect.Close();
        }

        private void DataEntry_Loaded(object sender, RoutedEventArgs e)
        {

            var CohortYNPerm = EpiInfoMenuManager.GetPermanentVariableValue("CohortYNPerm"); //MLHIDE

            if (CohortYNPerm.ToString() == "True") //MLHIDE
            {
                CurrentCohort.IsChecked = true;
            }
            else
            {
                CurrentCohort.IsChecked = false;
            }
        }
    }
}
