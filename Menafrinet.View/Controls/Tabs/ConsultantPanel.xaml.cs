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

namespace Menafrinet.View.Controls
{
    /// <summary>
    /// Interaction logic for ResidentPanel.xaml
    /// </summary>
    public partial class ConsultantPanel : UserControl
    {

        Epi.View conview { get; set; }

        Project Project { get; set; }

        string appDir = Environment.CurrentDirectory;

        string uniqueID;

        string operatorSymbol;

        public ConsultantPanel()
        {
            InitializeComponent();
        }

        DataTable conresults;
        DataView conDV;
        bool eventhandled;

        private void btnNewRecordConsultant_Click(object sender, RoutedEventArgs e)
        {

            Cursor = Cursors.Wait;

            //Project condata = new Project(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

            ////Database = Project.CollectedData.GetDatabase();
            //foreach (Epi.View view in condata.Views)
            //{
            //    // ugly hack... this is needed to get the view object without needing to specify the view name (which may differ between countries)
            //    // and because we can't just say Project.Views[0]
            //    if (view.Name == "ConsultantInfo")
            //    {
            //        conview = view;
            //        break;
            //    }
            //}


            //Epi.Enter.EnterUIConfig coniConfig = new Epi.Enter.EnterUIConfig();

            //coniConfig.AllowOneRecordOnly.Add(conview, false);
            //coniConfig.ShowDashboardButton.Add(conview, true);
            //coniConfig.ShowDeleteButtons.Add(conview, true);
            //coniConfig.ShowEditFormButton.Add(conview, false);
            //coniConfig.ShowFileMenu.Add(conview, false);
            //coniConfig.ShowFindButton.Add(conview, false);
            //coniConfig.ShowLineListButton.Add(conview, true);
            //coniConfig.ShowMapButton.Add(conview, false);
            //coniConfig.ShowNavButtons.Add(conview, true);
            //coniConfig.ShowNewRecordButton.Add(conview, true);
            //coniConfig.ShowOpenFormButton.Add(conview, false);
            //coniConfig.ShowPrintButton.Add(conview, true);
            //coniConfig.ShowRecordCounter.Add(conview, true);
            //coniConfig.ShowSaveRecordButton.Add(conview, true);
            //coniConfig.ShowToolbar.Add(conview, true);
            //coniConfig.ShowLinkedRecordsViewer = false;

            //Epi.Windows.Enter.EnterMainForm emf_consultant = new Epi.Windows.Enter.EnterMainForm(condata, conview, coniConfig);

            ////Epi.Windows.Enter.EnterMainForm emf = new Epi.Windows.Enter.EnterMainForm(DataHelper.Project, DataHelper.CaseForm, uiConfig);

            //emf_consultant.RecordSaved += new SaveRecordEventHandler(emfCases_RecordSaved);
            //emf_consultant.ShowDialog();
            //emf_consultant.RecordSaved -= new SaveRecordEventHandler(emfCases_RecordSaved);

            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            string appDir = Environment.CurrentDirectory;
            //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
            string commandText = appDir + "\\Enter.exe";

            if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"))
            {
                eventhandled = false;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = commandText;
                proc.Exited += new EventHandler(RefreshDatagrid);
                proc.EnableRaisingEvents = true;
                proc.StartInfo.Arguments = "/project: " + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj /view:ConsultantInfo /record:* /title:ReDPPeT Conultant Entry";
                proc.StartInfo.UseShellExecute = true;
                proc.Start();

            }

            const int SleepAmount = 500;
            while (!eventhandled)
            {
                Thread.Sleep(SleepAmount); // Wait, and...
            }

            dgRecordsConsultants_Sub();


            this.Cursor = Cursors.Arrow;

        }

        void RefreshDatagrid(object sender, EventArgs e)
        {
            eventhandled = true;
        }

        public void emfCases_RecordSaved(object sender, SaveRecordEventArgs e)
        {
            //RefreshCases();
            //string caseGuid = e.RecordGuid;
            //if (e.Form == conview)
            //{
            //    ((this.DataContext) as DataHelper).UpdateOrAddCase.Execute(caseGuid);
            //}

            //dgRecordsConsultant_Loaded();

            dgRecordsConsultants_Sub();

        }


        private void btnPrintRecordsConsultant_Click(object sender, RoutedEventArgs e)
        {
            Print_Sub();
        }

        private void dgRecords_MouseDoubleClickConsultant(object sender, RoutedEventArgs e)
        {
            if (dgRecordsConsultant.SelectedItems.Count == 1)
            {

                Cursor = Cursors.Wait;

                DataRowView drvcon = dgRecordsConsultant.SelectedItem as DataRowView;

                uniqueID = drvcon.Row.ItemArray[0].ToString();

                EditCase();

                dgRecordsConsultants_Sub();

                this.Cursor = Cursors.Arrow;

            }
        }

        public void dgRecordsConsultant_Loaded(object sender, RoutedEventArgs e)
        {
            dgRecordsConsultants_Sub();
        }


        public void dgRecordsConsultants_Sub()
        {
            string appDir = Environment.CurrentDirectory;

            string connString =
                "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB"; // 
                //"Provider=Microsoft.Jet.OLEDB.4.0;data Source=Projects\\TrackingMaster\\TrackingMasterNew.mdb";

            conresults = new DataTable();

            if ((DelRecConsultants).IsChecked == true)
            {
                operatorSymbol = "is not null";
            }
            else
            {
                operatorSymbol = " = 1";
            }

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                //OleDbCommand cmd = new OleDbCommand("SELECT ConsultantInfo.*, ConsultantInfo24.* FROM ConsultantInfo24 INNER JOIN ConsultantInfo ON ConsultantInfo24.GlobalRecordId = ConsultantInfo.GlobalRecordId;", conn);
                OleDbCommand cmd = new OleDbCommand("SELECT ConsultantInfo.*, ConsultantInfo24.*, (SELECT  [codementor1].[mentor] FROM codementor1 WHERE [ConsultantInfo24].[Mentor] = Left([codementor1].[mentor],1) AND [codementor1].[mentor] = [ConsultantInfo24].[Mentor]  & Mid([codementor1]![mentor],2)) AS Mentor, (SELECT  [codesupervisor1].[supervisor] FROM codesupervisor1 WHERE [ConsultantInfo24].[Supervisor] = Left([codesupervisor1].[supervisor],1) AND [codesupervisor1].[supervisor]= [ConsultantInfo24].[Supervisor]  & Mid([codesupervisor1].[supervisor],2)) AS Supervisor FROM ConsultantInfo24 INNER JOIN ConsultantInfo ON ConsultantInfo24.GlobalRecordId = ConsultantInfo.GlobalRecordId WHERE (((ConsultantInfo.RECSTATUS)" + operatorSymbol + ")) ORDER BY ConsultantInfo24.FullName;", conn);

                conn.Open();

                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

                adapter.Fill(conresults);

                dgRecordsConsultant.ItemsSource = conresults.DefaultView;
                adapter.Update(conresults);
                RecordCountConsultant.Text = dgRecordsConsultant.Items.Count.ToString();

            }
        }

        private void txtSearchConsultant_TextChanged(object sender, TextChangedEventArgs e)
        {
            conDV = new DataView(conresults);
            //conDV.RowFilter = string.Format("FullName LIKE '%{0}%' or ProfessionalAffiliation LIKE '%{0}%' or Supervisor LIKE '%{0}%' or Mentor LIKE '%{0}%'", txtSearchConsultant.Text);
            conDV.RowFilter = string.Format("FullName LIKE '%{0}%' or ProfessionalAffiliation LIKE '%{0}%'", txtSearchConsultant.Text);
            RecordCountConsultant.Text = conDV.Count.ToString();
            dgRecordsConsultant.ItemsSource = conDV;
        }

        private void EditCase()
        {
            Cursor = Cursors.Wait;

            //Project resdata = new Project(appDir + "\\" + Properties.Settings.Default.ProjectPath); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

            //foreach (Epi.View view in resdata.Views)
            //{
            //    // ugly hack... this is needed to get the view object without needing to specify the view name (which may differ between countries)
            //    // and because we can't just say Project.Views[0]
            //    if (view.Name == "ConsultantInfo")
            //    {
            //        conview = view;
            //        break;
            //    }
            //}


            //Epi.Enter.EnterUIConfig coniConfig = new Epi.Enter.EnterUIConfig();

            //coniConfig.AllowOneRecordOnly.Add(conview, false);
            //coniConfig.ShowDashboardButton.Add(conview, true);
            //coniConfig.ShowDeleteButtons.Add(conview, true);
            //coniConfig.ShowEditFormButton.Add(conview, false);
            //coniConfig.ShowFileMenu.Add(conview, false);
            //coniConfig.ShowFindButton.Add(conview, false);
            //coniConfig.ShowLineListButton.Add(conview, true);
            //coniConfig.ShowMapButton.Add(conview, false);
            //coniConfig.ShowNavButtons.Add(conview, true);
            //coniConfig.ShowNewRecordButton.Add(conview, true);
            //coniConfig.ShowOpenFormButton.Add(conview, false);
            //coniConfig.ShowPrintButton.Add(conview, true);
            //coniConfig.ShowRecordCounter.Add(conview, true);
            //coniConfig.ShowSaveRecordButton.Add(conview, true);
            //coniConfig.ShowToolbar.Add(conview, true);
            //coniConfig.ShowLinkedRecordsViewer = false;

            //Epi.Windows.Enter.EnterMainForm emf_consultant = new Epi.Windows.Enter.EnterMainForm(resdata, conview, coniConfig);

            ////Epi.Enter.EnterUIConfig uiConfig = Core.Common.GetExistingCaseConfig(DataHelper.CaseForm);
            ////Epi.Windows.Enter.EnterMainForm emf = new Epi.Windows.Enter.EnterMainForm(DataHelper.Project, DataHelper.CaseForm, uiConfig);

            //int uniqueKey = ((CaseViewModel)dgRecordsResidents.SelectedItem).Case.UniqueKey;

            int uniqueKey = (Convert.ToInt32(uniqueID));

            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            string appDir = Environment.CurrentDirectory;
            //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
            string commandText = appDir + "\\Enter.exe";

            if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"))
            {
                eventhandled = false;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = commandText;
                proc.Exited += new EventHandler(RefreshDatagrid);
                proc.EnableRaisingEvents = true;
                proc.StartInfo.Arguments = "/project: " + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj /view:ConsultantInfo /record:" + uniqueKey;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }

            const int SleepAmount = 500;
            while (!eventhandled)
            {
                Thread.Sleep(SleepAmount); // Wait, and...
            }

                dgRecordsConsultants_Sub();

        }

        private void Print_Sub()
        {

            string baseFileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString("N");

            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append(Menafrinet.Core.Common.GetHtmlHeader().ToString());

            htmlBuilder.AppendLine("<table width=\"100%\" style=\"border: 0px; padding: 0px; margin: 0px; clear: left; width:100%; \">");
            htmlBuilder.AppendLine(" <tr style=\"border: 0px;\">");
            htmlBuilder.AppendLine("  <td width=\"50%\" style=\"border: 0px;\">");
            htmlBuilder.AppendLine("   <p style=\"font-size: 13pt; font-weight: bold; clear: left; text-decoration: underline;\">Resident Driven Project Tracking Tool (ReDPeTT)</p>");
            htmlBuilder.AppendLine(ml.ml_string(176, "   <p style=\"font-size: 13pt; font-weight: bold; text-decoration: underline;\">Consultant Line List</p>"));
            htmlBuilder.AppendLine(ml.ml_string(155, "   <p style=\"font-size: 13pt; font-weight: bold;\">Date printed: ") + DateTime.Now.ToShortDateString() + "</p>");

            if (!String.IsNullOrEmpty(txtSearchConsultant.Text.Trim()))
            {
                htmlBuilder.AppendLine(ml.ml_string(175, "   <p style=\"font-size: 13pt; font-weight: bold;\">Filter: ") + txtSearchConsultant.Text.Trim() + "</p>");
            }

            htmlBuilder.AppendLine("  </td>");
            htmlBuilder.AppendLine(" </tr>");
            htmlBuilder.AppendLine("</table>");


            htmlBuilder.AppendLine("<p style=\"font-weight: bold; clear: left;\">&nbsp;</p>");

            htmlBuilder.AppendLine("<table style=\"width: 1200px; border: 4px solid black;\" align=\"left\">");
            htmlBuilder.AppendLine("<thead>");
            htmlBuilder.AppendLine("<tr style=\"border-top: 0px solid black;\">");
            htmlBuilder.AppendLine(ml.ml_string(173, "<th style=\"width: 180px;\">Full Name</th>"));
            htmlBuilder.AppendLine(ml.ml_string(172, "<th style=\"width: 140px;\">Professional Affiliation</th>"));
            htmlBuilder.AppendLine(ml.ml_string(171, "<th style=\"width: 140px;\">Mentor</th>"));
            htmlBuilder.AppendLine(ml.ml_string(170, "<th style=\"width: 30px;\">Supervisor</th>"));
            htmlBuilder.AppendLine(ml.ml_string(163, "<th style=\"width: 30px;\">Mobile Phone</th>"));
            htmlBuilder.AppendLine(ml.ml_string(169, "<th style=\"width: 100px;\">Consultant Email</th>"));
            //htmlBuilder.AppendLine("<th style=\"width: 140px;\">District</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 30px;\">Year</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 30px;\">Epi<br/>Week</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 100px;\">Classification</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 100px;\">Outcome</th>");
            //Added Created
            htmlBuilder.AppendLine(ml.ml_string(174, "<th style=\"width: 100px;\">Created</th>"));
            //Added Updated
            htmlBuilder.AppendLine(ml.ml_string(152, "<th style=\"width: 100px;\">Updated</th>"));
            htmlBuilder.AppendLine("</tr>");
            htmlBuilder.AppendLine("</thead>");

            htmlBuilder.AppendLine("<tbody>");

            conDV = new DataView(conresults);
            conDV.RowFilter = string.Format("FullName LIKE '%{0}%' or ProfessionalAffiliation LIKE '%{0}%'", txtSearchConsultant.Text);

            foreach (DataRowView drv in conDV)
            {
                if (drv["RECSTATUS"].ToString() == "1")
                {
                    htmlBuilder.AppendLine("<tr>");
                }
                else
                {
                    htmlBuilder.AppendLine("<tr bgcolor = Lightgrey>");
                }
                htmlBuilder.AppendLine("<td>" + drv["FullName"] + " </td>");
                htmlBuilder.AppendLine("<td>" + drv["ProfessionalAffiliation"] + " </td>");
                htmlBuilder.AppendLine("<td>" + drv["Mentor"] + " </td>");
                htmlBuilder.AppendLine("<td>" + drv["Supervisor"] + " </td>");
                htmlBuilder.AppendLine("<td>" + drv["MobilePhone"] + " </td>");

                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[11].ToString() + "</td>");
                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[16].ToString() + "</td>");
                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[26].ToString() + "</td>");
                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[27].ToString() + "</td>");
                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[24].ToString() + "</td>");

                //if (caseVM.AgeYears.HasValue)
                //{
                //    htmlBuilder.AppendLine("<td align=\"right\">" + caseVM.AgeYears.ToString() + "</td>");
                //}
                //else
                //{
                //    htmlBuilder.AppendLine("<td>&nbsp;</td>");
                //}
                htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[21].ToString() + "</td>");
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

                if (drv.Row.ItemArray[4] != null)
                {
                    //htmlBuilder.AppendLine("<td align=\"right\">" + caseVM.DateRecordCreated.Value.ToShortDateString() + "</td>");
                    htmlBuilder.AppendLine("<td align=\"right\">" + drv.Row.ItemArray[4] + "</td>");

                }
                else
                {
                    htmlBuilder.AppendLine("<td>&nbsp;</td>");
                }

                if (drv.Row.ItemArray[6] != null)
                {
                    htmlBuilder.AppendLine("<td align=\"right\">" + drv.Row.ItemArray[6] + "</td>");
                }
                else
                {
                    htmlBuilder.AppendLine("<td>&nbsp;</td>");
                }

                htmlBuilder.AppendLine("</tr>");
            }

            htmlBuilder.AppendLine("</tbody>");
            htmlBuilder.AppendLine("</table>");

            string fileName = baseFileName + ".html";

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

        private void DelRecConsultants_Unchecked(object sender, RoutedEventArgs e)
        {
            conDV = new DataView(conresults);
            conDV.RowFilter = string.Format("FullName LIKE '%{0}%' or ProfessionalAffiliation LIKE '%{0}%'", txtSearchConsultant.Text);
            RecordCountConsultant.Text = conDV.Count.ToString();
            dgRecordsConsultant.ItemsSource = conDV;

            dgRecordsConsultants_Sub();

        }

        private void DelRecConsultants_Checked(object sender, RoutedEventArgs e)
        {
            conDV = new DataView(conresults);
            conDV.RowFilter = string.Format("FullName LIKE '%{0}%' or ProfessionalAffiliation LIKE '%{0}%'", txtSearchConsultant.Text);
            RecordCountConsultant.Text = conDV.Count.ToString();
            dgRecordsConsultant.ItemsSource = conDV;

            dgRecordsConsultants_Sub();
        }
    }


}

