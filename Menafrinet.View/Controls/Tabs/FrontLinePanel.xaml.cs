﻿using MultiLang;
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
using Menafrinet.View.DataAccess;

namespace Menafrinet.View.Controls
{
    /// <summary>
    /// Interaction logic for ResidentPanel.xaml
    /// </summary>
    public partial class FrontLinePanel : UserControl
    {
        Epi.View resview { get; set; }
        Project Project { get; set; }

        string appDir = Environment.CurrentDirectory;

        string uniqueID;

        bool eventhandled;

        string operatorSymbol;

        public FrontLinePanel()
        {
            InitializeComponent();

        }

        DataTable results;
        DataView DV;

        private DataHelper DataHelper
        {
            get
            {
                return (this.DataContext as DataHelper);
            }
        }

        private void btnNewRecordFrontLine_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            //Project resdata = new Project(appDir + "\\" + Properties.Settings.Default.ProjectPath); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

            //foreach (Epi.View view in resdata.Views)
            //{
            //    // ugly hack... this is needed to get the view object without needing to specify the view name (which may differ between countries)
            //    // and because we can't just say Project.Views[0]
            //    if (view.Name == "TraineeInformation")
            //    {
            //        resview = view;
            //        break;
            //    }
            //}


            //Epi.Enter.EnterUIConfig resUiConfig = new Epi.Enter.EnterUIConfig();

            //resUiConfig.AllowOneRecordOnly.Add(resview, false);
            //resUiConfig.ShowDashboardButton.Add(resview, true);
            //resUiConfig.ShowDeleteButtons.Add(resview, true);
            //resUiConfig.ShowEditFormButton.Add(resview, false);
            //resUiConfig.ShowFileMenu.Add(resview, false);
            //resUiConfig.ShowFindButton.Add(resview, false);
            //resUiConfig.ShowLineListButton.Add(resview, true);
            //resUiConfig.ShowMapButton.Add(resview, false);
            //resUiConfig.ShowNavButtons.Add(resview, true);
            //resUiConfig.ShowNewRecordButton.Add(resview, true);
            //resUiConfig.ShowOpenFormButton.Add(resview, false);
            //resUiConfig.ShowPrintButton.Add(resview, true);
            //resUiConfig.ShowRecordCounter.Add(resview, true);
            //resUiConfig.ShowSaveRecordButton.Add(resview, true);
            //resUiConfig.ShowToolbar.Add(resview, true);
            //resUiConfig.ShowLinkedRecordsViewer = false;

            //Epi.Windows.Enter.EnterMainForm emf_resident = new Epi.Windows.Enter.EnterMainForm(resdata, resview, resUiConfig);

            //emf_resident.RecordSaved += new SaveRecordEventHandler(emfCases_RecordSaved);
            //emf_resident.ShowDialog();
            //emf_resident.RecordSaved -= new SaveRecordEventHandler(emfCases_RecordSaved);

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
                proc.StartInfo.Arguments = "/project: " + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj /view:Frontline /record:* /title:ReDPPeT FrontLine Entry";
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
                //proc.CloseMainWindow();
                //proc.WaitForExit();
                //proc.Close();
                //proc.Dispose();

            }

            const int SleepAmount = 500;
            while (!eventhandled)
            {
                Thread.Sleep(SleepAmount); // Wait, and...
            }

            dgRecordsFrontLine_Sub();

            //FillFrontLine_Sub();

            DataHelper.LoadSiteMergeData();

            this.Cursor = Cursors.Arrow;

        }

        private IEnumerable<object> GetProcessesByName(string processName)
        {
            throw new NotImplementedException();
        }

        public void emfCases_RecordSaved(object sender, SaveRecordEventArgs e)
        {
            //RefreshCases();
            //string caseGuid = e.RecordGuid;
            //if (e.Form == resview)
            //{
            //    ((this.DataContext) as DataHelper).UpdateOrAddCase.Execute(caseGuid);
            //}

            dgRecordsFrontLine_Sub();

        }

        private void btnPrintRecordsFrontLine_Click(object sender, RoutedEventArgs e)
        {
            Print_Sub();
        }

        private void dgRecords_MouseDoubleClickFrontLine(object sender, RoutedEventArgs e)
        {
            if (dgRecordsFrontLine.SelectedItems.Count == 1)
            {

                Cursor = Cursors.Wait;

                DataRowView drv = dgRecordsFrontLine.SelectedItem as DataRowView;

                uniqueID = drv.Row.ItemArray[0].ToString();

                EditCase();

                dgRecordsFrontLine_Sub();


                //FillFrontLine_Sub();

                DataHelper.LoadSiteMergeData();

                //Thread.Sleep(2500);

                this.Cursor = Cursors.Arrow;

            }
        }

        private void dgRecordsFrontLine_Loaded(object sender, RoutedEventArgs e)
        {
            dgRecordsFrontLine_Sub();
        }


        public void dgRecordsFrontLine_Sub()
        {

            string appDir = Environment.CurrentDirectory;

            string connString = "Provider=Microsoft.Jet.OLEDB.4.0" + ";data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.mdb"; //MLHIDE

            results = new DataTable();

            if ((DelRecFrontLine).IsChecked == true)
            {
                operatorSymbol = "is not null"; 
            }
            else
            {
                operatorSymbol = " = 1";
            }

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                OleDbCommand cmd = new OleDbCommand("SELECT Frontline.*, Frontline118.*, Frontline119.* FROM (Frontline INNER JOIN Frontline118 ON Frontline.GlobalRecordId = Frontline118.GlobalRecordId) INNER JOIN Frontline119 ON Frontline.GlobalRecordId = Frontline119.GlobalRecordId; ", conn);

                conn.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                adapter.Fill(results);
                dgRecordsFrontLine.ItemsSource = results.DefaultView;
                adapter.Update(results);
                RecordCountResident.Text = dgRecordsFrontLine.Items.Count.ToString();

                //((AnalysisPanel)Application.Current.Windows[1].FindName("panelAnalysis")).RefreshFrontLine(results);

            }
        }

        private void txtSearchFrontLine_TextChanged(object sender, TextChangedEventArgs e)
        {
            // "RPFL7_FullName LIKE '%{0}%' or RPFL1_CohortNumber LIKE '%{0}%' or RPFL10_Email LIKE '%{0}%'"
            DV = new DataView(results);
            DV.RowFilter = string.Format("RPFL7_FullName LIKE '%{0}%' or RPFL10_Email LIKE '%{0}%'", txtSearchFrontLine.Text);
            RecordCountResident.Text = DV.Count.ToString();
            dgRecordsFrontLine.ItemsSource = DV;
        }

        private void EditCase()
        {
            Cursor = Cursors.Wait;

            //Project resdata = new Project(appDir + "\\" + Properties.Settings.Default.ProjectPath); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

            //foreach (Epi.View view in resdata.Views)
            //{
            //    // ugly hack... this is needed to get the view object without needing to specify the view name (which may differ between countries)
            //    // and because we can't just say Project.Views[0]
            //    if (view.Name == "TraineeInformation")
            //    {
            //        resview = view;
            //        break;
            //    }
            //}


            //Epi.Enter.EnterUIConfig resUiConfig = new Epi.Enter.EnterUIConfig();

            //resUiConfig.AllowOneRecordOnly.Add(resview, false);
            //resUiConfig.ShowDashboardButton.Add(resview, true);
            //resUiConfig.ShowDeleteButtons.Add(resview, true);
            //resUiConfig.ShowEditFormButton.Add(resview, false);
            //resUiConfig.ShowFileMenu.Add(resview, false);
            //resUiConfig.ShowFindButton.Add(resview, false);
            //resUiConfig.ShowLineListButton.Add(resview, true);
            //resUiConfig.ShowMapButton.Add(resview, false);
            //resUiConfig.ShowNavButtons.Add(resview, true);
            //resUiConfig.ShowNewRecordButton.Add(resview, true);
            //resUiConfig.ShowOpenFormButton.Add(resview, false);
            //resUiConfig.ShowPrintButton.Add(resview, true);
            //resUiConfig.ShowRecordCounter.Add(resview, true);
            //resUiConfig.ShowSaveRecordButton.Add(resview, true);
            //resUiConfig.ShowToolbar.Add(resview, true);
            //resUiConfig.ShowLinkedRecordsViewer = false;

            //Epi.Windows.Enter.EnterMainForm emf_resident = new Epi.Windows.Enter.EnterMainForm(resdata, resview, resUiConfig);

            ////Epi.Enter.EnterUIConfig uiConfig = Core.Common.GetExistingCaseConfig(DataHelper.CaseForm);
            ////Epi.Windows.Enter.EnterMainForm emf = new Epi.Windows.Enter.EnterMainForm(DataHelper.Project, DataHelper.CaseForm, uiConfig);

            //int uniqueKey = ((CaseViewModel)dgRecordsResidents.SelectedItem).Case.UniqueKey;



            //emf_resident.LoadRecord(Convert.ToInt32(uniqueID));
            //emf_resident.RecordSaved += new SaveRecordEventHandler(emfCases_RecordSaved);
            //emf_resident.ShowDialog();
            //emf_resident.RecordSaved -= new SaveRecordEventHandler(emfCases_RecordSaved);

            int uniqueKey = (Convert.ToInt32(uniqueID));

            //emf.LoadRecord(uniqueKey);
            //emf.RecordSaved += new SaveRecordEventHandler(emfCases_RecordSaved);
            //emf.ShowDialog();
            //emf.RecordSaved -= new SaveRecordEventHandler(emfCases_RecordSaved);

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
                proc.StartInfo.Arguments = "/project: " + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj /view:Frontline /record:" + uniqueKey;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
                //proc.WaitForExit();
                //proc.Close();
                //proc.Dispose();

            }

            const int SleepAmount = 500;
            while (!eventhandled)
            {
                Thread.Sleep(SleepAmount); // Wait, and...
            }

        }

        void RefreshDatagrid(object sender, EventArgs e)
        {
            eventhandled = true;
        }

        private void Print_Sub()
        {

            string baseFileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString("N");

            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append(Menafrinet.Core.Common.GetHtmlHeader().ToString());

            htmlBuilder.AppendLine("<table width=\"100%\" style=\"border: 0px; padding: 0px; margin: 0px; clear: left; width:100%; \">");
            htmlBuilder.AppendLine(" <tr style=\"border: 0px;\">");
            htmlBuilder.AppendLine("  <td width=\"50%\" style=\"border: 0px;\">");
            htmlBuilder.AppendLine(ml.ml_string(157, "   <p style=\"font-size: 13pt; font-weight: bold; clear: left; text-decoration: underline;\">Resident Driven Project Tracking Tool (ReDPeTT)</p>"));
            htmlBuilder.AppendLine(ml.ml_string(205,"   <p style=\"font-size: 13pt; font-weight: bold; text-decoration: underline;\">Frontline Line List</p>"));
            htmlBuilder.AppendLine(ml.ml_string(155, "   <p style=\"font-size: 13pt; font-weight: bold;\">Date printed: ") + DateTime.Now.ToShortDateString() + "</p>");

            if (!String.IsNullOrEmpty(txtSearchFrontLine.Text.Trim()))
            {
                htmlBuilder.AppendLine(ml.ml_string(168, "   <p style=\"font-size: 13pt; font-weight: bold;\">Filter: ") + txtSearchFrontLine.Text.Trim() + "</p>");
            }

            htmlBuilder.AppendLine("  </td>");
            htmlBuilder.AppendLine(" </tr>");
            htmlBuilder.AppendLine("</table>");


            htmlBuilder.AppendLine("<p style=\"font-weight: bold; clear: left;\">&nbsp;</p>");

            htmlBuilder.AppendLine("<table style=\"width: 1200px; border: 4px solid black;\" align=\"left\">");
            htmlBuilder.AppendLine("<thead>");
            htmlBuilder.AppendLine("<tr style=\"border-top: 0px solid black;\">");
            htmlBuilder.AppendLine(ml.ml_string(159,"<th style=\"width: 180px;\">Cohort Number</th>"));
            htmlBuilder.AppendLine(ml.ml_string(227,"<th style=\"width: 180px;\">Cohort Start Date</th>"));
            htmlBuilder.AppendLine(ml.ml_string(228,"<th style=\"width: 140px;\">Cohort End Date</th>"));
            htmlBuilder.AppendLine(ml.ml_string(229,"<th style=\"width: 140px;\">Full Name</th>"));
            htmlBuilder.AppendLine(ml.ml_string(230,"<th style=\"width: 140px;\">Email</th>"));
            //htmlBuilder.AppendLine(ml.ml_string(163, "<th style=\"width: 30px;\">Mobile Phone</th>"));
            //htmlBuilder.AppendLine(ml.ml_string(164, "<th style=\"width: 30px;\">Trainee Email</th>"));
            //htmlBuilder.AppendLine("<th style=\"width: 100px;\">Health Facility</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 140px;\">District</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 30px;\">Year</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 30px;\">Epi<br/>Week</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 100px;\">Classification</th>");
            //htmlBuilder.AppendLine("<th style=\"width: 100px;\">Outcome</th>");
            //Added Created
            htmlBuilder.AppendLine(ml.ml_string(166, "<th style=\"width: 100px;\">Created</th>"));
            //Added Updated
            htmlBuilder.AppendLine(ml.ml_string(152, "<th style=\"width: 100px;\">Updated</th>"));
            htmlBuilder.AppendLine("</tr>");
            htmlBuilder.AppendLine("</thead>");

            htmlBuilder.AppendLine("<tbody>");

            DV = new DataView(results);
            DV.RowFilter = string.Format("RPFL7_FullName LIKE '%{0}%' or RPFL1_CohortNumber = '%{0}%' or RPFL10_Email LIKE '%{0}%'", txtSearchFrontLine.Text);

            foreach (DataRowView drv in DV)
            {
                if (drv["RECSTATUS"].ToString() == "1") 
                {
                    htmlBuilder.AppendLine("<tr>");
                }
                else
                {
                    htmlBuilder.AppendLine("<tr bgcolor = Lightgrey>");
                }

                htmlBuilder.AppendLine("<td>" + drv["RPFL1_CohortNumber"] + " </td>");
                htmlBuilder.AppendLine("<td>" + drv["RPFL2_CohortStart"] + " </td>");
                htmlBuilder.AppendLine("<td>" + drv["RPFL3_CohortEnd"] + " </td>");
                htmlBuilder.AppendLine("<td>" + drv["RPFL7_FullName"] + " </td>");
                htmlBuilder.AppendLine("<td>" + drv["RPFL10_Email"] + " </td>");

                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[35].ToString() + "</td>");
                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[11].ToString() + "</td>");
                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[9].ToString() + "</td>");
                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[33].ToString() + "</td>");
                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[38].ToString() + "</td>");
                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[31].ToString() + "</td>");
                //htmlBuilder.AppendLine("<td>" + drv.Row.ItemArray[32].ToString() + "</td>");

                //if (caseVM.AgeYears.HasValue)
                //{
                //    htmlBuilder.AppendLine("<td align=\"right\">" + caseVM.AgeYears.ToString() + "</td>");
                //}
                //else
                //{
                //    htmlBuilder.AppendLine("<td>&nbsp;</td>");
                //}
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

        private void dgRecordsFrontLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void FillFrontLine_Sub()
        {

            XmlDocument doc = new XmlDocument();
            //XmlElement root2 = doc.DocumentElement;
            //XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            //doc.InsertBefore(xmlDeclaration, root2);

            //XmlElement element1 = doc.CreateElement(string.Empty, "<rootElement>", string.Empty);
            XmlText text1 = doc.CreateTextNode("ResidentList");
            //element1.AppendChild(text1);


            Project project = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj");

            IDbDriver dbDistricts = project.CollectedData.GetDatabase();
            DataTable dtDistricts = dbDistricts.GetTableData("codeFieldSites120RAStatus");

            int cnt = 0;

            using (StreamWriter outputFile = new StreamWriter(appDir + "\\Projects\\TrackingMaster\\SiteMergeStatus.xml"))
            {
                string quotes = "\"";
                //string countryname = country;

                //outputFile.WriteLine("<?xml version=" + quotes + "1.0" + quotes + "?>");
                //outputFile.WriteLine("<Districts Country=" + quotes + "Mali" + quotes + ">");
                outputFile.WriteLine("<Districts Country=" + quotes + "FrontLineList" + quotes + ">");


                foreach (DataRow DRow in dtDistricts.Rows)
                {

                    if (cnt < dtDistricts.Rows.Count)
                    {

                        int RecStatusString = Convert.ToInt32(((dtDistricts.Rows[cnt]["Recstatus"].ToString())));

                        if (RecStatusString != 0)
                        {
                            String District2 = (dtDistricts.Rows[cnt]["RAStatus"].ToString());
                            //String DistrictCode = (dtDistricts.Rows[cnt]["CohortNumber"].ToString());
                            //String Region = (dtDistricts.Rows[cnt]["GivenName"].ToString());
                            //String RegionCode = (dtDistricts.Rows[cnt]["Surname"].ToString());
                            //String CountryRegDistCode = DistrictCode;

                            //outputFile.WriteLine("   <District Name=" + quotes + District + quotes + " Code=" + quotes + CountryRegDistCode + quotes + " Region = " + quotes + Region + quotes + " RegionCode=" + quotes + RegionCode + quotes + ">");

                            outputFile.WriteLine("   <District Name=" + quotes + District2 + quotes + " Code=" + quotes + "" + quotes + " Region = " + quotes + "" + quotes + " RegionCode=" + quotes + "" + quotes + ">");


                            //outputFile.WriteLine("     <LastReported>635190000000000000</LastReported>");
                            outputFile.WriteLine("    <LastReported>636780039481982551</LastReported>");
                            outputFile.WriteLine("   </District>");

                            cnt++;
                        }
                    }
                }

                Project project2 = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj");
                IDbDriver dbDistricts2 = project.CollectedData.GetDatabase();
                DataTable dtDistricts2 = dbDistricts2.GetTableData("TraineeInformationDropdown");


                cnt = 0;

                foreach (DataRow DRow in dtDistricts2.Rows)
                {

                    if (cnt < dtDistricts2.Rows.Count)
                    {

                        int RecStatusString = Convert.ToInt32(((dtDistricts2.Rows[cnt]["Recstatus"].ToString())));

                        if (RecStatusString != 0)
                        {
                            String District = (dtDistricts2.Rows[cnt]["FullName"].ToString());
                            String DistrictCode = (dtDistricts2.Rows[cnt]["CohortNumber"].ToString());
                            String Region = (dtDistricts2.Rows[cnt]["GivenName"].ToString());
                            String RegionCode = (dtDistricts2.Rows[cnt]["Surname"].ToString());
                            String CountryRegDistCode = DistrictCode;

                            //outputFile.WriteLine("   <District Name=" + quotes + District + quotes + " Code=" + quotes + CountryRegDistCode + quotes + " Region = " + quotes + Region + quotes + " RegionCode=" + quotes + RegionCode + quotes + ">");

                            outputFile.WriteLine("   <District Name=" + quotes + District + quotes + " Code=" + quotes + CountryRegDistCode + quotes + " Region = " + quotes + Region + quotes + " RegionCode=" + quotes + RegionCode + quotes + ">");

                            //outputFile.WriteLine("     <LastReported>635190000000000000</LastReported>");
                            outputFile.WriteLine("    <LastReported>636780039481982551</LastReported>");
                            outputFile.WriteLine("   </District>");

                            cnt++;

                        }
                    }
                }

                outputFile.WriteLine("</Districts>");
                outputFile.Close();
                //doc.Save(appDir + "\\Projects\\Menafrinet\\SiteMergeStatus.xml");


            }
        }

        private void DelRecFrontLine_Checked(object sender, RoutedEventArgs e)
        {
            DV = new DataView(results);
            DV.RowFilter = string.Format("RPFL7_FullName LIKE '%{0}%' or RPFL1_CohortNumber = '%{0}%' or RPFL10_Email LIKE '%{0}%'", txtSearchFrontLine.Text);
            RecordCountResident.Text = DV.Count.ToString();
            dgRecordsFrontLine.ItemsSource = DV;

            dgRecordsFrontLine_Sub();

        }

        private void DelRecFrontLine_Unchecked(object sender, RoutedEventArgs e)
        {
            DV = new DataView(results);
            DV.RowFilter = string.Format("RPFL7_FullName LIKE '%{0}%' or RPFL1_CohortNumber = '%{0}%' or RPFL10_Email LIKE '%{0}%'", txtSearchFrontLine.Text);
            RecordCountResident.Text = DV.Count.ToString();
            dgRecordsFrontLine.ItemsSource = DV;

            dgRecordsFrontLine_Sub();
        }
    }
}
