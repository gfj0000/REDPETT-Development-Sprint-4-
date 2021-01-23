using MultiLang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Epi;
using Epi.Core;
using Epi.Data;
using Epi.Menu;
using Menafrinet.ViewModel;
using Menafrinet.View.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Windows.Xps.Packaging;
using Spire.Doc;
using Spire.Doc.Documents;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GemBox.Document;
using System.Data.OleDb;
using System.Threading;
using System.Data.Common;
using Run = GemBox.Document.Run;
using Spire.Xls;
using Spire.Pdf;
//using Spire.Xls.Converter;
using Workbook = Spire.Xls.Workbook;
using Spire.Pdf.AutomaticFields;

namespace Menafrinet.View.Controls
{
    /// <summary>
    /// Interaction logic for AnalysisPanel.xaml
    /// </summary>
    public partial class AnalysisPanel : UserControl
    {
        int addNew = 0;
        string Reports_ReportProcessing;
        string appDir = Environment.CurrentDirectory;
        string ErrMsg;
        DateTime datePicker1_str;
        DateTime datePicker2_str;
        int CN;
        string RN;
        string UV;
        bool chkFlag;
        public IDbDriver Database { get; protected set; }



        //private System.Windows.Media.Brush BackGroundColor;

        private DataHelper DataHelper
        {
            get
            {
                return (this.DataContext as DataHelper);
            }
        }

        public object ReportButton { get; private set; }

        public AnalysisPanel()
        {
            InitializeComponent();
            //CanvasSelection.Margin = new Thickness(-150, 6, 0, 0);
            PopulateFileNames();
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            //DistrictComboBox.SelectedIndex = 0;
            //EpiStartYearComboBox.SelectedIndex = 0;
            //EpiEndYearComboBox.SelectedIndex = 0;
            //EpiWeekComboBox.SelectedIndex = 0;
            //documentViewer1.Document = null;

            datePicker1.SelectedDate = null;
            datePicker2.SelectedDate = null;
            Cohort.SelectedValue = ml.ml_string(242,"Select Item");
            Cohort.Text = ml.ml_string(242,"Select Item");
            Resident.SelectedValue = ml.ml_string(242,"Select Item");
            Resident.Text = ml.ml_string(242,"Select Item");
            University.SelectedValue = ml.ml_string(242,"Select Item");
            University.Text = ml.ml_string(242,"Select Item");

            PopulateFileNames();

        }

        private void Button_Click_Selected(object sender, RoutedEventArgs e)
        {

            Cursor = Cursors.Wait;

            //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");

            var button = (System.Windows.Controls.Button)sender;
            Reports_ReportProcessing = button.Content.ToString();

            if (File.Exists(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + addNew + ".xps"))
            {
                addNew = addNew + 1;
            }


            if (File.Exists(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".cvs7"))
            {
                Update_All_Click(Reports_ReportProcessing);

                EpiInfoReports();

                if (rptChoice.IsChecked == true)
                {
                    try
                    {

                        // If using Professional version, put your serial key below.

                        ComponentInfo.SetLicense("DN-2019Jul17-9vDKuk7e2Wx9EEzVGRElHpPOwRbArEIC1ifuToPErI9RnpBf4aET0YWqMU8rn3auEr3VWWYlDJz8eD0GGnyhAR4+/Eg==A");


                        DocumentModel document = DocumentModel.Load(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + ".html");

                        //GemBox.Document.PageSetup pageSetup = section.PageSetup;


                        var pageSetup1 = document.Sections[0].PageSetup;

                        //var pageSetup2 = document.Sections[1].PageSetup;

                        // Set page orientation.
                        pageSetup1.Orientation = GemBox.Document.Orientation.Landscape;

                        // Convert HTML to XPS.
                        //DocumentModel.Load(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + ".html").Save(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + addNew + ".xps");

                        //GemBox.Document.Size size = size: landscape;


                        foreach (GemBox.Document.Tables.Table table in document.GetChildElements(true, ElementType.Table))
                            foreach (GemBox.Document.Tables.TableCell cell in table.Rows[0].Cells)
                            {
                                bool first = true;
                                foreach (Run run in cell.GetChildElements(true, ElementType.Run))
                                {
                                    run.Text = run.Text.ToLower();

                                    if (first)
                                    {
                                        char[] chars = run.Text.ToCharArray();
                                        chars[0] = char.ToUpper(chars[0]);
                                        run.Text = new string(chars);
                                    }

                                    first = false;
                                }
                            }




                        document.Save(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + addNew + ".xps");

                        // Set page margins.
                        pageSetup1.PageMargins.Top = 10;
                        //pageSetup1.PageMargins.Bottom = 10;

                        //// Set paper type.
                        //pageSetup2.PaperType = PaperType.Letter;

                        //// Set line numbering.
                        //pageSetup2.LineNumberRestartSetting = LineNumberRestartSetting.NewPage;


                    //size: landscape

                        //new XpsSaveOptions()
                        //{
                        //    SelectionType = SelectionType.EntireFile
                        //});

                        //DocumentModel document = DocumentModel.Load(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + ".html");

                        //Document document = new Document();
                        //document.LoadFromFile(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + ".html", FileFormat.Html, XHTMLValidationType.None);

                        ////Save html to PDF. 
                        //documerent.SaveToFile(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + addNew + ".pdf", FileFormat.PDF);

                        ////Save html to XPS. 
                        //document.SaveToFile(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + addNew + ".xps", FileFormat.XPS);





                        browser2.Visibility = Visibility.Hidden;
                        documentViewer.Visibility = Visibility.Visible;
                        System.Windows.Xps.Packaging.XpsDocument d1 = new System.Windows.Xps.Packaging.XpsDocument(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + addNew + ".xps", System.IO.FileAccess.Read);
                        documentViewer.Document = d1.GetFixedDocumentSequence();
                        //d1.Close();
                    }
                    catch
                    {
                        //documentViewer.v
                        documentViewer.Visibility = Visibility.Hidden;
                        browser2.Visibility = Visibility.Visible;
                        documentViewer.Visibility = Visibility.Hidden;
                        string appDir = Environment.CurrentDirectory;
                        Uri pageUri = new Uri(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + ".html");
                        browser2.Source = pageUri;
                    }
                }
                else
                {
                    documentViewer.Visibility = Visibility.Hidden;
                    browser2.Visibility = Visibility.Visible;
                    documentViewer.Visibility = Visibility.Hidden;
                    string appDir = Environment.CurrentDirectory;
                    Uri pageUri = new Uri(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + ".html");
                    browser2.Source = pageUri;
                }

                this.Cursor = Cursors.Arrow;

            }

            if (File.Exists(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".xls"))
            {

                Update_All_Click(Reports_ReportProcessing);

                ExcelProcessing(Reports_ReportProcessing);

                //gfj TODO: fix the document pathing
                browser2.Visibility = Visibility.Hidden;
                documentViewer.Visibility = Visibility.Visible;
                System.Windows.Xps.Packaging.XpsDocument d1 = new System.Windows.Xps.Packaging.XpsDocument(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + addNew + ".xps", System.IO.FileAccess.Read);
                documentViewer.Document = d1.GetFixedDocumentSequence();

                //Workbook workbook = new Workbook();
                //workbook.LoadFromFile("sample.xlsx", ExcelVersion.Version2010);
                //workbook.SaveToFile("result.xps", Spire.Xls.FileFormat.XPS);


                // If using Professional version, put your serial key below.
                //SpreadsheetInfo.SetLicense("E0MV-XK9C-WFG7-59GD");


                //// Load Excel file.
                //var ef = ExcelFile.Load(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".xls");

                ////ExcelWorksheet ws = ef.Worksheets.Add("DataTable to Sheet");
                //System.Data.DataTable dt = new System.Data.DataTable();


                //string connString =
                //    "Provider=Microsoft.ACE.OLEDB.12.0" +
                //    ";data Source=" + "Projects\\TrackingMaster\\TrackingMasterNew.mdb";

                ////var dt = new System.Data.DataTable();
                //var ws = ef.Worksheets.ActiveWorksheet;


                //using (OleDbConnection conn = new OleDbConnection(connString))
                //{
                //    OleDbCommand cmd = new OleDbCommand("" +
                //        "SELECT ReportTable.projecttitle AS [Project Title], ReportTable.DiseaseSuspected AS [Disease Suspected], ReportTable.DateAssigned AS [Date Assigned], Trainees8.ResidentName2 AS [Lead Resident Name], Trainees8.CohortNumber AS [Lead Cohort Number], ProjectsByCohort_NumberofProjects.CountOfFETP AS [Number of Residents on Project], ReportTable.ResidentNames AS [Resident Names] FROM(ReportTable INNER JOIN(Trainees8 INNER JOIN Trainees ON Trainees8.GlobalRecordId = Trainees.GlobalRecordId) ON ReportTable.GlobalRecordId = Trainees.FKEY) INNER JOIN ProjectsByCohort_NumberofProjects ON ReportTable.GlobalRecordId = ProjectsByCohort_NumberofProjects.GlobalRecordId WHERE(((Trainees8.ResidentPosition2) = 'Lead'));", conn);

                //    conn.Open();

                //    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

                //    adapter.Fill(dt);
                //    adapter.Update(dt);
                //}

                //// Insert DataTable into an Excel worksheet.
                //ws.InsertDataTable(dt,
                //new InsertDataTableOptions()
                //{
                //    ColumnHeaders = true,
                //    StartRow = 17
                //});

                ////ws.Cells["B14"].Value = "ExcelFont.BoldWeight";
                //ws.Cells["A18"].Style.Font.Weight = ExcelFont.BoldWeight;
                //ws.Columns["A"].Width = 30 * 256;
                //ws.Cells.GetSubrange("A18", "I30").Style.Borders.SetBorders(GemBox.Spreadsheet.MultipleBorders.Outside, System.Drawing.Color.Black, LineStyle.Thin);


                ////ExcelFile ef = ExcelFile.Load(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".xlsx");
                //ef.Save(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + addNew + ".xps");



                this.Cursor = Cursors.Arrow;

            }

            // gfj0 - map 


            if (Reports_ReportProcessing.Contains("Map")) //MLHIDE
            {

                Update_All_Click(Reports_ReportProcessing);

                string text = File.ReadAllText(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".map7");  //MLHIDE
                if (!text.Contains("View")) //MLHIDE
                {

                    //File.WriteAllText(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".map7", text);

                    XmlTextReader canvas_xml = new XmlTextReader(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".map7"); //MLHIDE
                    XmlDocument canvas_doc = new XmlDocument();
                    canvas_doc.Load(canvas_xml);
                    canvas_xml.Close();
                    XmlNode criterionNode;
                    XmlElement root = canvas_doc.DocumentElement;
                    criterionNode = root.SelectSingleNode("dataLayer/dashboardHelper/projectPath"); //MLHIDE
                    criterionNode.InnerText = appDir + "\\Projects\\TrackingMaster\\"; //MLHIDE
                    canvas_doc.Save(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".map7"); //MLHIDE

                    Epi.Menu.EpiInfoMenuManager.OpenMapsWithMap(appDir + "\\Projects\\TrackingMaster\\map.map7"); //MLHIDE

                }
                else
                {
                    string connectionString = null;

                    // gfj -- This is the string to decypt string  ConnectionString = Configuration.Decrypt("test");
                    XmlTextReader canvas_xml = new XmlTextReader(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".map7"); //MLHIDE
                    XmlDocument canvas_doc = new XmlDocument();
                    canvas_doc.Load(canvas_xml);
                    canvas_xml.Close();
                    XmlNode criterionNode;
                    XmlElement root = canvas_doc.DocumentElement;
                    criterionNode = root.SelectSingleNode("dataLayer/dashboardHelper/connectionString"); //MLHIDE
                    string CS = criterionNode.InnerText.ToString().Replace("connectionString=\"", ""); //MLHIDE

                    if (!string.IsNullOrEmpty(CS) && !string.IsNullOrEmpty(CS))
                    {
                        connectionString = Configuration.Decrypt(CS);
                    }

                    const string DataSourceMarker = "Data Source="; //MLHIDE

                    //if (!connectionString.Contains(appDir))
                    //{

                    string filepath = null;  //MLHIDE
                    var filepathjoin = new List<string>();
                    int cntpart = 0;

                    var parts = connectionString.Split(';');
                    foreach (var part in parts)
                    {

                        //if (part.StartsWith(DataSourceMarker, StringComparison.OrdinalIgnoreCase))
                        //{
                        //    ////filepath = part.Substring(DataSourceMarker.Length);

                        //    //break;

                        //}

                        filepathjoin.Add(part);
                        cntpart++;
                    }

                    //MessageBox.Show(parts[1] + " - " + parts[2] + " - " + parts[3]);
                    //connectionString.Replace(filepath, appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB");
                    //connectionString.Replace("Bob", "GFJ");
                    //MessageBox.Show(filepath + " - " + connectionString);
                    // appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".map7"
                    //connectionString = filepathjoin[0] + appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".map7" + filepathjoin[2];
                    connectionString = filepathjoin[0] + "Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".map7" + filepathjoin[2];

                    CS = Configuration.Encrypt(connectionString);

                    criterionNode.InnerText = CS;
                    canvas_doc.Save(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".map7");

                    //}

                    Epi.Menu.EpiInfoMenuManager.OpenMapsWithMap(appDir + "\\Projects\\TrackingMaster\\map.map7");

                }


                this.Cursor = Cursors.Arrow;

            }
        }


        private void Update_All_Click(string Reports_ReportProcessing)
        {
            // =========== Start config reporting ======== //

            string appDir = Environment.CurrentDirectory;
            //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");


            if (datePicker1.SelectedDate.HasValue)
            {
                datePicker1_str = (DateTime)datePicker1.SelectedDate;
            }
            else
            {
                datePicker1_str = Convert.ToDateTime("01/01/1977");
            }

            if (datePicker2.SelectedDate.HasValue)
            {
                datePicker2_str = (DateTime)datePicker2.SelectedDate;
            }
            else
            {
                datePicker2_str = Convert.ToDateTime("01/01/2050");
            }

            if (!string.IsNullOrEmpty(Cohort.Text) && !(Cohort.Text == ml.ml_string(242, "Select Item")))
            {
                CN = Int32.Parse(Cohort.Text);
            }
            else
            {
                CN = 99;
            }

            if (!string.IsNullOrEmpty(Resident.Text) && !(Resident.Text == ml.ml_string(242, "Select Item")))
            {
                RN = Resident.Text;
            }
            else
            {
                RN = "ALL";
            }

            if (!string.IsNullOrEmpty(University.Text) && !(University.Text == ml.ml_string(242, "Select Item")))
            {
                UV = University.Text;
            }
            else
            {
                UV = "ALL";
            }


            Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable("Permanent_Date_Beg", 3);
            Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable("Permanent_Date_End", 3);
            Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable("Permanent_Cohort", 1);
            Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable("Permanent_Resident", 2);
            Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable("Permanent_University", 2);
            Epi.Menu.EpiInfoMenuManager.SetPermanentVariable("Permanent_Date_Beg", datePicker1_str);
            Epi.Menu.EpiInfoMenuManager.SetPermanentVariable("Permanent_Date_End", datePicker2_str);
            Epi.Menu.EpiInfoMenuManager.SetPermanentVariable("Permanent_Cohort", CN);
            Epi.Menu.EpiInfoMenuManager.SetPermanentVariable("Permanent_Resident", RN);
            Epi.Menu.EpiInfoMenuManager.SetPermanentVariable("Permanent_University", UV);

            string Todays_Date_Str = DateTime.Now.ToShortDateString();

            OleDbConnection connect = new OleDbConnection();

            string connstring = appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB";

            connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + connstring;
            connect.Open();

            //MessageBox.Show("Connection Opened");

            OleDbCommand command = new OleDbCommand();
            command.Connection = connect;


            if (System.IO.File.Exists(Environment.CurrentDirectory + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"))
            {
                if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".cvs7"))
                {

                    string textCHK = File.ReadAllText(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".cvs7");

                    if (textCHK.Contains("ReportTable_Trainee"))
                    {

                        //    if (Reports_ReportProcessing.Substring(0, 15).Contains("Enrolled") || (Reports_ReportProcessing.Substring(0, 15).Contains("Grads")))
                        //{
                        var schema = connect.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                        foreach (var row in schema.Rows.OfType<DataRow>())
                        {
                            string tableName = row.ItemArray[2].ToString();
                            if (tableName == "ReportTable_Trainee")
                            {
                                // table exists
                                command.CommandText = "Drop Table ReportTable_Trainee";
                                command.ExecuteNonQuery();

                                //MessageBox.Show("Yeah!!!");
                            }
                        }



                        command.Parameters.AddWithValue("Permanent_Date_Beg", datePicker1_str);
                        command.Parameters.AddWithValue("Permanent_Date_End", datePicker2_str);
                        command.Parameters.AddWithValue("Cohort", CN);
                        command.Parameters.AddWithValue("RES", RN);
                        command.Parameters.AddWithValue("Univ", UV);


                        command.CommandText = "SELECT TraineeInformation21.*, TraineeInformation.RECSTATUS INTO ReportTable_Trainee FROM TraineeInformation INNER JOIN TraineeInformation21 ON TraineeInformation.GlobalRecordId = TraineeInformation21.GlobalRecordId WHERE ((TraineeInformation.RECSTATUS=1) AND((Monthbeginning >=[Permanent_Date_Beg] And Monthbeginning <= [Permanent_Date_End])) AND (([cohort] <> '99' And [CohortNumber]=[Cohort]) Or ([cohort] = '99' And [CohortNumber] <>[Cohort])) AND (([RN] <> '99' And [FullName]=[RN]) Or ([RN] = 'ALL' And [FullName] <>[RN])) AND (([UV] <> '99' And [UniversityTrainingInstitution]=[UV]) Or ([UV] = 'ALL' And [UniversityTrainingInstitution] <>[UV])));"; //MLHIDE


                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception f)
                        {

                            MessageBox.Show("No records have been entered." + f.ToString(), "Warning:");

                            //    Report1.IsEnabled = true;
                            //    Report2.IsEnabled = true;
                            //    Report3.IsEnabled = true;
                            //    Report4.IsEnabled = true;
                            //    Report5.IsEnabled = true;

                            this.Cursor = Cursors.Arrow;

                            return;
                        }
                    }


                    if (textCHK.Contains("ReportTable")) //MLHIDE
                    {


                        //if (new[] { "Cohort", "Resident", "Dup" }.Any(c => Reports_ReportProcessing.Contains(c)))
                        //{
                        int counter = 0;
                        int cnt = 0;
                        string GlobalID = null;


                        OleDbCommand DelCommand2 = new OleDbCommand("select * from [ReportTable]", connect); //MLHIDE
                        DelCommand2.CommandText = "Delete * from ResidentNamesTab;"; //MLHIDE
                        DelCommand2.ExecuteNonQuery();

                        OleDbCommand command2 = new OleDbCommand();
                        command2.Connection = connect;

                        Project project4 = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //MLHIDE
                        IDbDriver db = project4.CollectedData.GetDatabase();
                        System.Data.DataTable dt = db.GetTableData("Query16"); //MLHIDE


                        if (Reports_ReportProcessing.Contains("Project by Cohort")) //MLHIDE
                        {
                            foreach (DataRow dRow in dt.Rows)
                            {
                                string delStatus = dt.Rows[cnt]["RECSTATUS"].ToString();
                                string prjStatus = dt.Rows[cnt]["Projects.GlobalRecordId"].ToString();
                                string ResidentNames = null;

                                if (delStatus != "0")
                                {

                                    int cnt2 = 0;
                                    Project project2 = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj");
                                    IDbDriver db2 = project2.CollectedData.GetDatabase();
                                    System.Data.DataTable dt2 = db.GetTableData("Trainees");


                                    foreach (DataRow dRow2 in dt2.Rows)
                                    {
                                        string GlobalCompare = dt.Rows[cnt]["Projects.GlobalRecordId"].ToString();
                                        string FKEYCompare = dt2.Rows[cnt2]["FKEY"].ToString();
                                        if (GlobalCompare == FKEYCompare)
                                        {
                                            int cnt3 = 0;
                                            Project project3 = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj");
                                            IDbDriver db3 = project3.CollectedData.GetDatabase();
                                            System.Data.DataTable dt3 = db.GetTableData("Query5");


                                            foreach (DataRow dRow3 in dt3.Rows)
                                            {
                                                string GlobalCompare2 = dt2.Rows[cnt2]["GlobalRecordId"].ToString();
                                                string FKEYCompare2 = dt3.Rows[cnt3]["GlobalRecordId"].ToString();

                                                int RecStat = Convert.ToInt32(dt3.Rows[cnt3]["Recstatus"].ToString());

                                                //if (GlobalCompare2 == FKEYCompare2 && RecStat != 0)
                                                if (GlobalCompare2 == FKEYCompare2 && RecStat != 0)
                                                {
                                                    counter = counter + 1;
                                                    GlobalID = GlobalCompare;
                                                    string ResName = dt3.Rows[cnt3]["ResidentName2"].ToString();
                                                    ResidentNames = ResidentNames + ResName + "; ";
                                                    //MessageBox.Show(GlobalCompare + " - " + FKEYCompare + " - " + ResidentNames);
                                                    break;
                                                }

                                                cnt3 = cnt3 + 1;

                                            }

                                        }

                                        cnt2 = cnt2 + 1;

                                    }


                                    int cnt4 = 0;

                                    string Dup = "0";

                                    foreach (DataRow dRowDups in dt.Rows)
                                    {

                                        if (dt.Rows[cnt]["DateAssigned"].ToString() != null)
                                        {

                                            var chkvar = dt.Rows[cnt]["DateAssigned"].ToString();
                                            var chkvar2 = dt.Rows[cnt4]["DateAssigned"].ToString();
                                            var chkvar3 = dt.Rows[cnt4]["type"].ToString();

                                            if (cnt != cnt4 && dt.Rows[cnt4]["DateAssigned"].ToString() != "")
                                            {
                                                string TypeRead = dt.Rows[cnt]["Type"].ToString();
                                                string TypeCompare = dt.Rows[cnt4]["Type"].ToString();
                                                DateTime DateIssueRead = DateTime.Parse(dt.Rows[cnt]["DateAssigned"].ToString());
                                                DateTime DateIssueCompare = DateTime.Parse(dt.Rows[cnt4]["DateAssigned"].ToString());
                                                DateTime threeDaysEarlier = DateIssueCompare.AddDays(-3);
                                                DateTime threeDaysLater = DateIssueCompare.AddDays(+3);

                                                if ((DateIssueRead >= threeDaysEarlier && DateIssueRead <= threeDaysLater) && TypeRead == TypeCompare)
                                                {
                                                    //MessageBox.Show("This is a dup: " + Environment.NewLine + "DateIssueRead: " + DateIssueRead + Environment.NewLine + " threeDaysEarlier: " + threeDaysEarlier + Environment.NewLine + " threeDaysLater: " + threeDaysLater + Environment.NewLine + " TypeRead: " + TypeRead + " TypeCompare: " + TypeCompare);
                                                    Dup = "1";
                                                }
                                            }

                                            cnt4 = cnt4 + 1;


                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                    try
                                    {


                                        if (GlobalID != null)
                                        {
                                            if (ResidentNames == null || ResidentNames.Length < 1)
                                            {
                                                ResidentNames = "...";
                                            }
                                            command2.Parameters.Clear();
                                            command2.Parameters.AddWithValue("GlobalID", GlobalID);
                                            command2.Parameters.AddWithValue("ResidentNames", ResidentNames);
                                            command2.Parameters.AddWithValue("PossibleDuplicateProject", Dup);
                                            command2.CommandText = "INSERT INTO ResidentNamesTab(GlobalID, ResidentNames, PossibleDuplicateProject) VALUES (@GlobalID, @ResidentNames, PossibleDuplicateProject)";
                                            command2.ExecuteNonQuery();
                                        }
                                    }
                                    catch
                                    {

                                        //    MessageBox.Show("No records have been entered." + g.ToString(), "Warning:");

                                        //    Report1.IsEnabled = true;
                                        //    Report2.IsEnabled = true;
                                        //    Report3.IsEnabled = true;
                                        //    Report4.IsEnabled = true;
                                        //    Report5.IsEnabled = true;

                                        //    this.Cursor = Cursors.Arrow;

                                        //return;
                                    }

                                }
                                cnt = cnt + 1;
                                ResidentNames = null;

                            }

                            //} // check this

                        }

                        var schema = connect.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" }); //MLHIDE

                        foreach (var row in schema.Rows.OfType<DataRow>())
                        {
                            string tableName = row.ItemArray[2].ToString();
                            if (tableName == "ReportTable") //MLHIDE
                            {
                                // table exists
                                command.CommandText = "Drop Table ReportTable"; //MLHIDE
                                command.ExecuteNonQuery();

                                //MessageBox.Show("Yeah!!!");
                            }
                        }



                        command.Parameters.AddWithValue("Permanent_Date_Beg", datePicker1_str); //MLHIDE
                        command.Parameters.AddWithValue("Permanent_Date_End", datePicker2_str); //MLHIDE
                        command.Parameters.AddWithValue("Cohort", CN); //MLHIDE
                        command.Parameters.AddWithValue("RES", RN); //MLHIDE
                        command.Parameters.AddWithValue("Univ", UV); //MLHIDE

                        command.CommandText = "SELECT Projects4.*, Projects5.*, ResidentNamesTab.*, Projects.RECSTATUS, Projects4.GlobalRecordId AS GlobalRecordId INTO ReportTable FROM ResidentNamesTab RIGHT JOIN((Projects INNER JOIN Projects4 ON Projects.GlobalRecordId = Projects4.GlobalRecordId) INNER JOIN Projects5 ON Projects.GlobalRecordId = Projects5.GlobalRecordId) ON ResidentNamesTab.GlobalID = Projects.GlobalRecordId WHERE((Projects.RECSTATUS = 1) AND((Projects4.DateAssigned >=[Permanent_Date_Beg] And Projects4.DateAssigned <=[Permanent_Date_End])) AND(([cohort] <> '99' And[Projects4].[leadcohort] =[Cohort]) Or([cohort] = '99' And[Projects4].[leadcohort] <>[Cohort]) ) AND (([RES] <> 'ALL' And (((InStr([ResidentNames],[RES]))>0))) Or ([RES] = 'ALL' And [ResidentNames] <> [RES])) AND (([Univ] <> 'ALL' And[UniversityTrainingInstitution] =[Univ]) Or([Univ] = 'ALL' And[UniversityTrainingInstitution] <>[Univ])));"; //MLHIDE


                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception f)
                        {

                            MessageBox.Show("No records have been entered." + f.ToString(), "Warning:");

                            //    Report1.IsEnabled = true;
                            //    Report2.IsEnabled = true;
                            //    Report3.IsEnabled = true;
                            //    Report4.IsEnabled = true;
                            //    Report5.IsEnabled = true;

                            this.Cursor = Cursors.Arrow;

                            return;
                        }

                    }
                    //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvasAndCreateOutput("Projects\\Menafrinet\\ClassificationByAgegroup.cvs7", "Projects\\Menafrinet\\Output\\Classification.html");
                }
            }

            connect.Close();
        }


        private void documentViewer_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //try
            //{
            //    Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
            //    string appDir = Environment.CurrentDirectory;
            //    //string Reports_ReportFullScreen = null;
            //    string strCmdText = $"Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + addNew + ".xps";
            //    Process.Start(strCmdText);
            //}
            //catch
            //{
            //    MessageBox.Show("Please by patient");
            //}
        }


        private void tblockCategory_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock)
            {
                TextBlock txt = sender as TextBlock;
                txt.Foreground = this.Resources["categoryHover"] as SolidColorBrush;
                this.Cursor = Cursors.Hand;
            }
        }



        private void tblockCategory_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock)
            {
                TextBlock txt = sender as TextBlock;
                txt.Foreground = this.Resources["categoryNormal"] as SolidColorBrush;
                this.Cursor = Cursors.Arrow;
            }
        }

        private void tblockPlace_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //panelPlaces.Visibility = Visibility.Visible;
            //panelTime.Visibility = Visibility.Collapsed;
            //panelPeople.Visibility = Visibility.Collapsed;

            //panelHeaderPlaces.Visibility = Visibility.Visible;
            //panelHeaderTime.Visibility = Visibility.Collapsed;
            //panelHeaderPeople.Visibility = Visibility.Collapsed;
        }

        private void tblockTime_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //panelPlaces.Visibility = Visibility.Collapsed;
            //panelTime.Visibility = Visibility.Visible;
            //panelPeople.Visibility = Visibility.Collapsed;

            ////panelHeaderPlaces.Visibility = Visibility.Collapsed;
            //panelHeaderTime.Visibility = Visibility.Visible;
            //panelHeaderPeople.Visibility = Visibility.Collapsed;
        }

        private void tblockPerson_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //panelPlaces.Visibility = Visibility.Collapsed;
            //panelTime.Visibility = Visibility.Collapsed;
            //panelPeople.Visibility = Visibility.Visible;

            ////panelHeaderPlaces.Visibility = Visibility.Collapsed;
            //panelHeaderTime.Visibility = Visibility.Collapsed;
            //panelHeaderPeople.Visibility = Visibility.Visible;
        }

        private void BtnCasesByEpiweek_Click(object sender, RoutedEventArgs e)
        {
            //SetSearchValues();
            //CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
            //string appDir = Environment.CurrentDirectory;
            //documentViewer1.Visibility = Visibility.Hidden;
            //browser2.Visibility = Visibility.Visible;
            //ReportsUpdateTextbox.Visibility = Visibility.Hidden;
            ////if (File.Exists(appDir + "\\Projects\\Menafrinet\\Reports\\AgeGroup.html"))
            ////{
            //Uri pageUri = new Uri(appDir + "\\projects\\Menafrinet\\Reports\\BarChartSuspectCases.html");
            //browser2.Source = pageUri;
            ////}
        }
        private void btnCases_Click(object sender, RoutedEventArgs e)
        {
            //SetSearchValues();
            //CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
            //documentViewer1.Visibility = Visibility.Hidden;
            //browser2.Visibility = Visibility.Visible;
            //ReportsUpdateTextbox.Visibility = Visibility.Hidden;
            //string appDir = Environment.CurrentDirectory;
            //Uri pageUri = new Uri(appDir + "\\projects\\Menafrinet\\Reports\\DistrictByCaseStatus.html");
            //browser2.Source = pageUri;
        }


        private void NotificationMap_Click(object sender, RoutedEventArgs e)
        {
            //string appDir = Environment.CurrentDirectory;
            //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
            //Epi.Menu.EpiInfoMenuManager.OpenAnalysisWithPGM(appDir + "\\Projects\\Menafrinet\\DistrictNotification.pgm7");
            //Epi.Menu.EpiInfoMenuManager.OpenMapsWithMap(appDir + "\\Projects\\Menafrinet\\Reports\\Map1.map7");
        }

        private void BtnClassifyByAgeGroup_Click(object sender, RoutedEventArgs e)
        {
            //browser2.Visibility = Visibility.Hidden;
            //documentViewer1.Visibility = Visibility.Visible;
            //System.Windows.Xps.Packaging.XpsDocument d1 = new System.Windows.Xps.Packaging.XpsDocument("projects\\Menafrinet\\reports\\AgeGroupByConfirmation.xps", System.IO.FileAccess.Read);
            //documentViewer1.Document = d1.GetFixedDocumentSequence();
            //d1.Close();

            //    SetSearchValues();
            //    CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
            //    documentViewer1.Visibility = Visibility.Hidden;
            //    browser2.Visibility = Visibility.Visible;
            //    ReportsUpdateTextbox.Visibility = Visibility.Hidden;
            //    string appDir = Environment.CurrentDirectory;
            //    Uri pageUri = new Uri(appDir + "\\projects\\Menafrinet\\Reports\\AgeGroup.html");
            //    browser2.Source = pageUri;
            //}

            //private void Btn_NmA_Click(object sender, RoutedEventArgs e)
            //{
            //    //CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
            //    //documentViewer1.Visibility = Visibility.Hidden;
            //    //browser2.Visibility = Visibility.Visible;
            //    //SetSearchValues();
            //    //ReportsUpdateTextbox.Visibility = Visibility.Hidden;
            //    //string appDir = Environment.CurrentDirectory;
            //    //Uri pageUri = new Uri(appDir + "\\Projects\\Menafrinet\\Reports\\Region-District.html");
            //    //browser2.Source = pageUri;
            //    //documentViewer1 = pageUri;
            //    //documentViewer1 = pageUri;
            //}


            //private void DataCleaning_Button_Click(object sender, RoutedEventArgs e)
            //{
            //    //CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
            //    //documentViewer1.Visibility = Visibility.Hidden;
            //    //browser2.Visibility = Visibility.Visible;
            //    //ReportsUpdateTextbox.Visibility = Visibility.Hidden;
            //    //string appDir = Environment.CurrentDirectory;
            //    //Uri pageUri = new Uri(appDir + "\\Projects\\Menafrinet\\Reports\\DataCleaning.html");
            //    //browser2.Source = pageUri;
            //    //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "\\Projects\\Reports\\DataCleaning.cvs7");
            //    //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "/DataCleaning.cvs7");
            //    //System.Diagnostics.Process.Start("Analysis.exe");
            //}

            //private void LinelistbyLabTest_Click(object sender, RoutedEventArgs e)
            //{
            //    //CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
            //    //documentViewer1.Visibility = Visibility.Hidden;
            //    //browser2.Visibility = Visibility.Visible;
            //    //ReportsUpdateTextbox.Visibility = Visibility.Hidden;
            //    //string appDir = Environment.CurrentDirectory;
            //    //Uri pageUri = new Uri(appDir + "\\Projects\\Menafrinet\\Reports\\LinelistbyLabTest.html");
            //    //browser2.Source = pageUri;
            //    //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "\\Projects\\Reports\\DataCleaning.cvs7");
            //    //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "/DataCleaning.cvs7");
            //    //System.Diagnostics.Process.Start("Analysis.exe");
            //}


            //private bool documentViewer1_DoubleClick(mshtml.IHTMLEventObj e)
            //{
            //    //MessageBox.Show("Switching to Full Screen");
            //    label1.Visibility = Visibility.Hidden;
            //    label2.Visibility = Visibility.Hidden;
            //    label3.Visibility = Visibility.Hidden;
            //    DistrictComboBox.Visibility = Visibility.Hidden;
            //    EpiStartYearComboBox.Visibility = Visibility.Hidden;
            //    EpiEndYearComboBox.Visibility = Visibility.Hidden;
            //    EpiWeekComboBox.Visibility = Visibility.Hidden;
            //    ClearButton.Visibility = Visibility.Hidden;
            //    documentViewer1.Visibility = Visibility.Hidden;
            //    ReportsText.Visibility = Visibility.Hidden;
            //    BtnCases.Visibility = Visibility.Hidden;
            //    BtnCasesByEpiweek.Visibility = Visibility.Hidden;
            //    BtnClassifyByAgeGroup.Visibility = Visibility.Hidden;
            //    NmALineList.Visibility = Visibility.Hidden;
            //    PerformanceIndicators.Visibility = Visibility.Hidden;
            //    SelectedEventButton.Visibility = Visibility.Hidden;

            //    documentViewer1.Visibility = Visibility.Visible;

            //    return true;
            //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "/Region-District.cvs7");
            ////Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "\\Projects\\Ghana\\Region-District.cvs7");
            //}

            //private bool documentViewer1_DoubleClick(mshtml.IHTMLEventObj e)
            //{
            //    //MessageBox.Show("Leaving Full Screen");
            //    label1.Visibility = Visibility.Visible;
            //    label2.Visibility = Visibility.Visible;
            //    label3.Visibility = Visibility.Visible;
            //    DistrictComboBox.Visibility = Visibility.Visible;
            //    EpiStartYearComboBox.Visibility = Visibility.Visible;
            //    EpiEndYearComboBox.Visibility = Visibility.Visible;
            //    EpiWeekComboBox.Visibility = Visibility.Visible;
            //    ClearButton.Visibility = Visibility.Visible;
            //    documentViewer1.Visibility = Visibility.Visible;
            //    ReportsText.Visibility = Visibility.Visible;
            //    BtnCases.Visibility = Visibility.Visible;
            //    BtnCasesByEpiweek.Visibility = Visibility.Visible;
            //    BtnClassifyByAgeGroup.Visibility = Visibility.Visible;
            //    NmALineList.Visibility = Visibility.Visible;
            //    PerformanceIndicators.Visibility = Visibility.Visible;
            //    SelectedEventButton.Visibility = Visibility.Visible;

            //    documentViewer1.Visibility = Visibility.Hidden;
            //    //documentViewer1.Focus();

            //    return true;


            //    //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "/Region-District.cvs7");
            //    //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "\\Projects\\Ghana\\Region-District.cvs7");
            //}    

            //private void FlatColorButton_Loaded(object sender, RoutedEventArgs e)
            //{

            //}

            //private void documentViewer1_LoadCompleted(object sender, NavigationEventArgs e)
            //{
            //    //mshtml.HTMLDocument doc = (mshtml.HTMLDocument)documentViewer1.Document;
            //    //mshtml.HTMLDocumentEvents2_Event iEvent = (mshtml.HTMLDocumentEvents2_Event)doc;
            //    //iEvent.ondblclick += new mshtml.HTMLDocumentEvents2_ondblclickEventHandler(documentViewer1_DoubleClick);
            //}
            ////private void documentViewer1_LoadCompleted(object sender, NavigationEventArgs e)
            ////{
            ////    mshtml.HTMLDocument doc = (mshtml.HTMLDocument)documentViewer1.Document;
            ////    mshtml.HTMLDocumentEvents2_Event iEvent = (mshtml.HTMLDocumentEvents2_Event)doc;
            ////    //iEvent.ondblclick += new mshtml.HTMLDocumentEvents2_ondblclickEventHandler(documentViewer1_DoubleClick);
            ////}
            //private void FlatColorButton_Loaded_1(object sender, RoutedEventArgs e)
            //{

            //}

            //private void btnCases_Loaded(object sender, RoutedEventArgs e)
            //{

            //}
        }

        //private void PopulateDistrictComboBox()
        //{
        //    DistrictComboBox.ItemsSource = null;
        //    DistrictComboBox.Items.Clear();

        //if (System.IO.File.Exists(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath))
        //{
        //    Project project = new Project(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

        //    IDbDriver db = project.CollectedData.GetDatabase();
        //    System.Data.DataTable dt = db.GetTableData("codedistrictsettings23codedistrictsettings", "District", "District");

        //    DistrictComboBox.Items.Add(string.Empty);

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        DistrictComboBox.Items.Add(row[0].ToString());
        //    }

        //}
        //}// PopulateDistrictComboBox -- end
        //=========================================================================================
        //private void PopulateEpiYearComboBoxs()
        //{
        //    EpiStartYearComboBox.ItemsSource = null;
        //    EpiStartYearComboBox.Items.Clear();
        //    EpiEndYearComboBox.ItemsSource = null;
        //    EpiEndYearComboBox.Items.Clear();

        //    EpiStartYearComboBox.Items.Add(string.Empty);
        //    EpiEndYearComboBox.Items.Add(string.Empty);

        //    string[] years = { "2030", "2029", "2028", "2027", "2026", "2025", "2024", "2023", "2022", "2021", "2020", "2019", "2018", "2017", "2016", "2015", "2014", "2013", "2012", "2011", "2010", "2009", "2008", "2007", "2006", "2005", "2004", "2003", "2002", "2001", "2000", "1999", "1998", "1997" };
        //    for (int n_i = 0; n_i < years.Length; n_i++)
        //    {
        //        EpiStartYearComboBox.Items.Add(years[n_i]);
        //        EpiEndYearComboBox.Items.Add(years[n_i]);
        //    }

        //if (System.IO.Directory.Exists(Environment.CurrentDirectory + PROJECT))
        //{
        //    Project project = new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

        //    IDbDriver db = project.CollectedData.GetDatabase();
        //    DataTable dt = db.GetTableData("GhanaCBS1", "EpiYear", "EpiYear");

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        if (!row[0].ToString().Equals(string.Empty))
        //        {
        //            if (IsDuplicateYear(EpiStartYearComboBox, row[0].ToString()) == false)
        //            {
        //                EpiStartYearComboBox.Items.Add(row[0].ToString());
        //                EpiEndYearComboBox.Items.Add(row[0].ToString());
        //            }
        //        }
        //    }
        //}
        //}// PopulateEpiYearComboBoxs -- end
        //=========================================================================================
        //private void PopulateEpiWeekComboBox()
        //{
        //    EpiWeekComboBox.Items.Add(string.Empty);
        //    for (int n_i = 1; n_i <= 53; n_i++)
        //        EpiWeekComboBox.Items.Add(n_i.ToString());

        //}// PopulateEpiWeekComboBox -- end
        ////=========================================================================================
        //private bool IsDuplicateYear(ComboBox EpiYearComboBox, string CurrentData)
        //{
        //    int n_size = EpiYearComboBox.Items.Count;
        //    for (int n_i = 0; n_i < n_size; n_i++)
        //    {
        //        if (EpiYearComboBox.Items.GetItemAt(n_i).Equals(CurrentData))
        //            return true;
        //    }
        //    return false;
        //}// IsDuplicateYear -- end
        //=========================================================================================


        //private void MouseEnteredBtnCases(object sender, MouseEventArgs e)
        //{
        //    if (BtnCases.IsMouseOver == true)
        //    {
        //        EpiWeekComboBox.IsEnabled = false;
        //    }

        //}// MouseEnteredBtnCases -- end
        ////=========================================================================================
        //private void MouseLeftBtnCases(object sender, MouseEventArgs e)
        //{
        //    if (BtnCases.IsMouseOver == false)
        //    {
        //        EpiWeekComboBox.IsEnabled = true;
        //    }
        //}
        //private void LoadDropDownLists(object sender, RoutedEventArgs e)
        //{
        //    PopulateDistrictComboBox();
        //    PopulateEpiYearComboBoxs();
        //    PopulateEpiWeekComboBox();
        //    BackGroundColor = BtnCases.Background;
        //}

        private void BtnCasesByEpiweek_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void CanvasSelection_MouseLeave(object sender, MouseEventArgs e)
        {
            //Canvas c = (Canvas)sender;
            //DoubleAnimation Animation = new DoubleAnimation(0, TimeSpan.FromSeconds(.5));
            //c.BeginAnimation(Canvas.OpacityProperty, Animation);
            this.Width = 200;
        }

        private void CanvasSelection_MouseEnter(object sender, MouseEventArgs e)
        {
            //Canvas c = (Canvas)sender;
            //DoubleAnimation Animation = new DoubleAnimation(1, TimeSpan.FromSeconds(1));
            //c.BeginAnimation(Canvas.OpacityProperty, Animation);
            this.Width = 50;
        }


        private void ChangeCriteria_flatColorButton_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            //if (CanvasSelection.Margin.Left == 0)
            //{
            //    ChangeCriteria_flatColorButton.Content = "Modifier Les Critères";
            //    CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
            //    ChangeCriteria_flatColorButton.ToolTip = "Cliquez ici pour mettre à jour ou de modifier les rapports.";
            //    documentViewer1.Visibility = Visibility.Hidden;
            //    browser2.Visibility = Visibility.Hidden;
            //    //ArrowIN.Visibility = System.Windows.Visibility.Hidden;
            //    //ArrowOUT.Visibility = System.Windows.Visibility.Visible;
            //    //DropShadowEffect Drop = null;
            //    //CanvasSelection.Effect = null;
            //    //Drop.Direction = 0;
            //    //Drop.BlurRadius = 0;
            //    //Drop.ShadowDepth = 0;
            //    //Drop.Color = Colors.Black;
            //    ////documentViewer1.Margin = new Thickness(246, 2, 7, 73);
            //    //documentViewer1.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    ChangeCriteria_flatColorButton.Content = "Sous-Menu Collapse";
            //    CanvasSelection.Margin = new Thickness(0, 32, 0, 0);
            //    ChangeCriteria_flatColorButton.ToolTip = "Cliquez ici pour fermer ce menu";
            //    documentViewer1.Visibility = Visibility.Hidden;
            //    browser2.Visibility = Visibility.Hidden;
            //    //ArrowIN.Visibility = System.Windows.Visibility.Visible;
            //    //ArrowOUT.Visibility = System.Windows.Visibility.Hidden;
            //    //DropShadowEffect Drop = new DropShadowEffect();
            //    //CanvasSelection.Effect = new DropShadowEffect();
            //    //Drop.Direction = 305;
            //    //Drop.BlurRadius = 15;
            //    //Drop.ShadowDepth = 6;
            //    //Drop.Color = Colors.Black;
            //    //documentViewer1.Margin = new Thickness(396, 2, 7, 73);
            //    //documentViewer1.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("test444");

            //ReportFileNames.Clear();

            FileInfo projectFileInfo = new FileInfo("Projects\\TrackingMaster");
            DirectoryInfo projectDirectory = projectFileInfo.Directory;

            var files = projectDirectory.GetFiles("*.cvs7", SearchOption.TopDirectoryOnly);  //MLHIDE

            //foreach (FileInfo fi in files)
            {
                //ReportFileNames.Add(fi);
            }

            //ReportFileNamesView = new ListCollectionView(ReportFileNames);
            {

            }

        }


        public void PopulateFileNames()
        {
            try
            {

                canvasList.Items.Clear();

                FileInfo projectFileInfo = new FileInfo("Projects\\TrackingMaster\\");
                DirectoryInfo projectDirectory = projectFileInfo.Directory;
                object FilterReportForPWD = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm");


                var files = projectDirectory.GetFiles("Reports_*.*", SearchOption.TopDirectoryOnly);

                foreach (FileInfo fi in files)
                {


                    //if (!fi.ToString().Contains("Conn"))
                    //{

                    if ((FilterReportForPWD.ToString() == "3" && !fi.ToString().Contains("Trainee Code")) || FilterReportForPWD.ToString() != "3")
                    {
                        string file = fi.ToString().Replace(@"Reports_", @"");  //MLHIDE "xls",@""
                                                                                //string file2 = file.ToString().Replace(@"Trainee_", @"");  // "xls",@""
                        string file3 = file.ToString().Replace(@".xls", @""); //MLHIDE
                        file3 = file3.ToString().Replace(@".cvs7", @""); //MLHIDE
                        file3 = file3.ToString().Replace(@".map7", @""); //MLHIDE
                        DataContext = file3;
                        canvasList.Items.Add(file3);
                    }
                    //}
                }



                University.ItemsSource = null;
            University.Items.Clear();

            if (System.IO.File.Exists(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath))
            {
                Project project = new Project(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

                IDbDriver db = project.CollectedData.GetDatabase();
                System.Data.DataTable dt = db.GetTableData("Query15");

                IDbDriver db2 = project.CollectedData.GetDatabase();
                System.Data.DataTable dt2 = db.GetTableData("codeFieldSites27University_Training", "University_Training", "University_Training");

                University.Items.Clear();
                University.Items.Add(ml.ml_string(242,"Select Item"));
                University.SelectedIndex = 0;
                //University.Items.Add("Select Item");

                foreach (DataRow row in dt2.Rows)
                {
                    University.Items.Add(row[0].ToString());
                }
            }

            Cohort.ItemsSource = null;
            Cohort.Items.Clear();

            if (System.IO.File.Exists(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath))
            {
                Project project = new Project(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath); 

                //MessageBox.Show("Check 2: " + University.SelectedValue.ToString());

                IDbDriver db = project.CollectedData.GetDatabase();
                System.Data.DataTable dt = db.GetTableData("Query21");

                Cohort.Items.Clear();
                Cohort.Items.Add(ml.ml_string(242,"Select Item"));
                Cohort.SelectedIndex = 0;

                foreach (DataRow row in dt.Rows)
                {
                    Cohort.Items.Add(row[2].ToString());
                }
            }

            if (System.IO.File.Exists(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath))
            {
                Project project = new Project(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath);
                IDbDriver db = project.CollectedData.GetDatabase();
                System.Data.DataTable dt = db.GetTableData("Query15");
                RefreshResidents(dt);
            }
            }
            catch
            {
                Cohort.Items.Clear();
                Cohort.Items.Add(ml.ml_string(242, "Select Item"));
                Cohort.SelectedIndex = 0;
                //University.Items.Add("Select Item");

                for (int i = 1; i <= 30; i++)
                { // print numbers from 1 to 5
                    {
                         Cohort.Items.Add(i.ToString());
                    }
                }
            }
        }

        public void RefreshResidents(System.Data.DataTable dt)
        {
            Resident.Items.Clear();
            Resident.Items.Add(ml.ml_string(242, "Select Item"));
            Resident.SelectedIndex = 0;

            foreach (DataRow row in dt.Rows)
            {
                if ((row["RECSTATUS"].ToString() == "1"))
                {
                    Resident.Items.Add(row["FullName"].ToString());
                }
            }
        }
        private void canvasList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ReportsViewModel vm = (panelAnalysis.DataContext as ReportsViewModel);

            //if (vm.ReportFileNamesView.CurrentItem != null)
            //{
            //FileInfo fi = canvasList.ToString();

            //string Reports_ReportProcessing = fi.FullName.Replace(".cvs7", ".html");

            //if (File.Exists(Reports_ReportProcessing))
            //{
            //    Uri uri = new Uri(Reports_ReportProcessing);
            //    browser.Source = uri; // new System.Uri((panelAnalysis.DataContext as ReportsViewModel).HtmlSource);
            //}
            //}
        }

        private void EpiInfoReports()
        {

            string Todays_Date_Str = DateTime.Now.ToShortDateString();

            if (File.Exists(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + ".html"))
            {
                File.Delete(appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + ".html");
            }

            string Label_str = " ";
            //string space = "     \u0020    ";
            Label_str = ml.ml_string(177, "Created: <b>") + Todays_Date_Str + "</b>";

            if (datePicker1_str.ToString("MM/dd/yyyy") != "01/01/1977") { Label_str += ml.ml_string(219,"<br />   Beginning Reporting Date:  <b>") + datePicker1_str.ToShortDateString() + "</b>"; }
            if (datePicker2_str.ToString("MM/dd/yyyy") != "01/01/2050") { Label_str += ml.ml_string(218,"<br />  Ending Reporting Date:  <b>") + datePicker2_str.ToShortDateString() + "</b>"; }
            if (CN != 99) { Label_str += ml.ml_string(216,"<br />  Cohort number: <b>") + CN + "</b>"; }
            if (RN != "ALL") { Label_str += ml.ml_string(217,"<br />  Resident: <b>") + RN + "</b>"; }
            if (UV != "ALL") { Label_str += ml.ml_string(212,"<br />  Institution: <b>") + UV + "</b>"; }

            XmlTextReader canvas_xml = new XmlTextReader("Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".cvs7");
            XmlDocument canvas_doc = new XmlDocument();
            canvas_doc.Load(canvas_xml);
            canvas_xml.Close();
            XmlNode criterionNode;
            XmlElement root = canvas_doc.DocumentElement;
            criterionNode = root.SelectSingleNode("Gadgets/standardTextReportGadget/text");

            if (criterionNode != null)
            {
                criterionNode.InnerText = Label_str;
            }

            canvas_doc.Save("Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".cvs7");

            // Create HTML Output
            Core.Common.OpenDashboardWithCanvasAndCreateOutput(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".cvs7", appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + ".html", true);

            //browser2.Source = null;
            //System.Threading.Thread.Sleep(1500);

            //Epi.Menu.EpiInfoMenuManager.OpenAnalysisWithPGM("Projects\\Menafrinet\\Reports\\DistrictMap.pgm7");

            //Epi.Menu.EpiInfoMenuManager.OpenAnalysisWithPGM(appDir + "\\DistrictTab.pgm7");
            //Epi.Menu.EpiInfoMenuManager.OpenAnalysisWithPGM(appDir + "\\DistrictNotification.pgm7");

            //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvasAndCreateOutput("Projects\\niger\\NmA.cvs7", "Projects\\niger\\Output\\NmA.html");

            //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvasAndCreateOutput("Projects\\niger\\Epicurve.cvs7", appDir + "\\Projects\\niger\\Output\\Epicurve.html");
            //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvasAndCreateOutput("Epicurve.cvs7", "Epicurve.html");

            //Epi.Menu.EpiInfoMenuManager.OpenAnalysisWithPGM("Projects\\Ghana\\LaunchEpicurve.pgm7");

            //ReportsUpdateTextbox.Visibility = Visibility.Visible;
            //CanvasSelection.Margin = new Thickness(-170, 29, 0, 0);
            //ReportProgressBar.Visibility = Visibility.Hidden;

        }


        public static DataRowCollection residentList;
        public void RefreshResidentList()
        {
            Resident.ItemsSource = residentList;
        }

        //private void SetSearchValues()
        //{
        //    //           if (DateTime.(datePicker1_str))
        //    //datePicker1.SelectedDate = Convert.ToDateTime("01/01/1977");
        //    //           if (DateTime.IsNullOrEmpty(datePicker2_str))
        //    //datePicker2.SelectedDate = Convert.ToDateTime("01/01/2050");

        //    if (datePicker1.SelectedDate.HasValue)
        //    {
        //        start_year_str = (DateTime)datePicker1.SelectedDate;
        //    }
        //    else
        //    {
        //        datePicker1_str = Convert.ToDateTime("01/01/1977");
        //    }
        //    if (datePicker2.SelectedDate.HasValue)
        //    {
        //        datePicker2_str = (DateTime)datePicker2.SelectedDate;
        //    }
        //    else
        //    {
        //        datePicker2_str = Convert.ToDateTime("01/01/2050");
        //    }


        //    //To be removed 11 / 1 / 15 DDP
        //    datePicker1.SelectedDate = Convert.ToDateTime("01/01/1977");
        //    datePicker2.SelectedDate = Convert.ToDateTime("01/01/2050");

        //    datePicker1_str = (DateTime)datePicker1.SelectedDate;
        //    datePicker2_str = (DateTime)datePicker2.SelectedDate;

        //}


        public void TestExcel(string Reports_ReportProcessing)
        {
            // load Excel file  
            Workbook workbook = new Workbook();
            workbook.LoadFromFile("D:\\MyExcel.xlsx");
            // Set PDF template  
            PdfDocument pdfDocument = new PdfDocument();
            pdfDocument.PageSettings.Orientation = PdfPageOrientation.Landscape;
            pdfDocument.PageSettings.Width = 970;
            pdfDocument.PageSettings.Height = 850;
            //Convert Excel to PDF using the template above  
            //PdfConverter pdfConverter = new PdfConverter(workbook);
            //PdfConverterSettings settings = new PdfConverterSettings();
            //settings.TemplateDocument = pdfDocument;
            //pdfDocument = pdfConverter.Convert(settings);
            // Save and preview PDF  
            pdfDocument.SaveToFile("MyPDF.pdf");
            System.Diagnostics.Process.Start("MyPDF.pdf");
        }


        public void ExcelProcessing(string Reports_ReportProcessing)
        {

            if (File.Exists(appDir + "\\Projects\\TrackingMaster\\" + Reports_ReportProcessing + addNew + ".xps"))
            {
                addNew = addNew + 1;
            }

            int ErrorCnt = 1;

            bool success = false;
            //while (!success)
            //{
            //try
            //{
            Microsoft.Office.Interop.Excel.Application app1 = null;
            Excel.Workbook wb1 = null;

            //Microsoft.Office.Interop.Excel.Application app1 = new Excel.Application();

            app1 = new Microsoft.Office.Interop.Excel.Application();

            if (app1 == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Thread.Sleep(2500);

            app1.Visible = false;
            app1.DisplayAlerts = false;
            app1.ScreenUpdating = false;

            //ReportsViewModel vm = (panelAnalysis.DataContext as ReportsViewModel);
            //FileInfo fi = vm.ReportFileNamesView.CurrentItem as FileInfo;

            //OleDbConnection thisConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Projects\TrackingMaster\TrackingMasterNew.MDB");

            //string strPath = appDir + "\\Projects\\TrackingMaster\\" + Reports_ReportProcessing + ".xls";
            //string strPath = appDir + "\\Projects\\TrackingMaster\\" + Reports_ReportProcessing + ".xls";


            //wb1 = app1.Workbooks.Open(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".xls", true);
            if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + "Conn.xls"))
            {
                wb1 = app1.Workbooks.Open(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + "Conn.xls", true);
            }
            else
            {

                wb1 = app1.Workbooks.Open(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + ".xls", true);

                // Set connection string
                setupConnections(appDir, wb1, app1);

            }



            //wb1.SaveAs(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + "Conn.xls");
            //var wb1 = app1.Workbooks.Open(strPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, 0);


            foreach (Excel.Worksheet ws1 in wb1.Worksheets)
            {

                string Label_str1 = " ";
                string Label_str2 = " ";

                if (datePicker1_str.ToString("MM/dd/yyyy") != "01/01/1977") { Label_str1 += "Beginning Report Date: " + datePicker1_str.ToString("MM/dd/yyyy"); }
                if (datePicker2_str.ToString("MM/dd/yyyy") != "01/01/2050") { Label_str2 += "Ending Report Date: " + datePicker2_str.ToString("MM/dd/yyyy"); }

                object misValue = System.Reflection.Missing.Value;
                ws1.Cells[12, 1] = Label_str1;
                ws1.Cells[13, 1] = Label_str2;


                //wb1.SaveAs(appDir + "\\Projects\\TrackingMaster\\Reports\\" + Reports_ReportProcessing + ".xls", XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                Thread.Sleep(2000);


                try
                {
                    while (!success)
                    {
                        try
                        {
                            //object misValue = System.Reflection.Missing.Value;
                            string paramExportFilePath = appDir + "\\Projects\\TrackingMaster\\Output_" + Reports_ReportProcessing + addNew + ".xps";
                            Excel.XlFixedFormatType paramExportFormat = Excel.XlFixedFormatType.xlTypeXPS;
                            Excel.XlFixedFormatQuality paramExportQuality = Excel.XlFixedFormatQuality.xlQualityStandard;
                            bool paramOpenAfterPublish = false;
                            bool paramIncludeDocProps = true;
                            bool paramIgnorePrintAreas = true;



                            //wb1.SaveAs(appDir + "\\Projects\\TrackingMaster\\Reports\\" + Reports_ReportProcessing + addNew + ".xps");


                            //}
                            //catch (Exception e)
                            //{
                            //    MessageBox.Show("Report Execution Error: " + e);
                            //}

                            //ws1.ExportAsFixedFormat(XlFixedFormatType.xlTypeXPS, (appDir + "\\Projects\\TrackingMaster\\Reports\\" + Reports_ReportProcessing + addNew + ".xps"), Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                            if (wb1 != null)//save as xps
                                wb1.ExportAsFixedFormat(paramExportFormat, paramExportFilePath, paramExportQuality, paramIncludeDocProps, paramIgnorePrintAreas, 1, 1, paramOpenAfterPublish, misValue);
                            success = true;
                            //MessageBox.Show("Success");
                        }
                        catch (System.Runtime.InteropServices.COMException e)
                        {
                            MessageBox.Show("Error LOOP");
                            if ((e.ErrorCode & 0xFFFFFFFF) == 0x80071779)
                            {   // Excel is busy
                                Thread.Sleep(2000); // Wait, and...
                                success = false;  // ...try again
                                                  //MessageBox.Show("W  A  I  T");
                                MessageBox.Show("Report Excution Error: " + e.ToString(), "Error");
                            }
                            else
                            {   // Re-throw!
                                throw e;
                            }
                        }
                    }

                }
                catch (Exception ex1)
                {
                    //obj = null;
                    MessageBox.Show("Report Excution Error: " + ex1.ToString(), "Error");


                    if (ErrorCnt == 2)
                    {
                        return;
                    }
                    else
                    {
                        ++ErrorCnt;
                    }
                }

            }

            wb1.Close(false, Type.Missing, Type.Missing);
            app1.Application.Quit();
            app1.Quit();


            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            wb1 = null;
            app1 = null;


            //this.xpsDocument = workbook.ConvertToXpsDocument(SaveOptions.XpsDefault);

            Process[] Processes;
            Processes = System.Diagnostics.Process.GetProcessesByName("EXCEL");
            foreach (System.Diagnostics.Process p in Processes)
            {
                if (p.MainWindowTitle.Trim() == "")
                    p.Kill();
            }

            //Report1.IsEnabled = true;
            //Report2.IsEnabled = true;
            //Report3.IsEnabled = true;
            //Report4.IsEnabled = true;
            //Report5.IsEnabled = true;

            //this.Cursor = Cursors.Arrow;

            //}
            //catch (System.Runtime.InteropServices.COMException e)
            //{

            //    MessageBox.Show("Error LOOP");

            //    if (ErrorCnt == 2)
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        ++ErrorCnt;
            //    }

            //    if ((e.ErrorCode & 0xFFFFFFFF) == 0x80071779)
            //    {   // Excel is busy
            //        Thread.Sleep(2500); // Wait, and...
            //        success = false;  // ...try again
            //        MessageBox.Show("W  A  I  T");

            //    }
            //    else
            //    {   // Re-throw!
            //        //throw e;
            //        MessageBox.Show("Exception Occured while Exporting: " + e.ToString());
            //    }

            //    // Dx8D07D6BA


            //    //catch (Exception ce)
            //    //{

            //    //{   // Excel is busy
            //    //MessageBox.Show("Excel is busy" + ce);
            //    Thread.Sleep(4000); // Wait, and...
            //    success = false;  // ...try again

            //    ErrMsg = e.Message;

            //    Process[] Processes;
            //    Processes = System.Diagnostics.Process.GetProcessesByName("EXCEL");
            //    foreach (System.Diagnostics.Process p in Processes)
            //    {
            //        if (p.MainWindowTitle.Trim() == "")
            //            p.Kill();
            //    }


            //}

            //Report1.IsEnabled = true;
            //Report2.IsEnabled = true;
            //Report3.IsEnabled = true;
            //Report4.IsEnabled = true;
            //Report5.IsEnabled = true;

            //this.Cursor = Cursors.Arrow;

            //}
        }


        private void setupConnections(string appDir, Excel.Workbook wb1, Excel.Application app1)
        {

            foreach (WorkbookConnection connection in wb1.Connections)
            {

                const string DataSourceMarker = "Data Source="; //MLHIDE

                var conString = connection.OLEDBConnection.Connection.ToString();
                if (!conString.Contains(appDir))
                {

                    string filepath = null;

                    var parts = conString.Split(';');
                    foreach (var part in parts)
                    {
                        if (part.StartsWith(DataSourceMarker, StringComparison.OrdinalIgnoreCase))
                        {
                            filepath = part.Substring(DataSourceMarker.Length);
                            break;
                        }
                    }

                    conString = conString.Replace(filepath, appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB");

                    connection.OLEDBConnection.Connection = conString;

                    wb1.SaveAs(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + "Conn.xls");

                    CloseEverything(app1, wb1);

                    ExcelProcessing(Reports_ReportProcessing);

                }
                else
                {
                    wb1.RefreshAll();
                    wb1.SaveAs(appDir + "\\Projects\\TrackingMaster\\Reports_" + Reports_ReportProcessing + "Conn.xls");
                    //Thread.Sleep(1500);
                }
            }
        }


        public void CloseEverything(Excel.Application app1, Excel.Workbook wb1)
        {

            wb1.Close(false, Type.Missing, Type.Missing);
            app1.Application.Quit();
            app1.Quit();


            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            wb1 = null;
            app1 = null;

            //this.xpsDocument = workbook.ConvertToXpsDocument(SaveOptions.XpsDefault);

            Process[] Processes;
            Processes = System.Diagnostics.Process.GetProcessesByName("EXCEL");
            foreach (System.Diagnostics.Process p in Processes)
            {
                if (p.MainWindowTitle.Trim() == "")
                    p.Kill();
            }

            //Report1.IsEnabled = true;
            //Report2.IsEnabled = true;
            //Report3.IsEnabled = true;
            //Report4.IsEnabled = true;
            //Report5.IsEnabled = true;

            //this.Cursor = Cursors.Arrow;

            //}
            //catch (System.Runtime.InteropServices.COMException e)
            //{

            //    MessageBox.Show("Error LOOP");

            //    if (ErrorCnt == 2)
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        ++ErrorCnt;
            //    }

            //    if ((e.ErrorCode & 0xFFFFFFFF) == 0x80071779)
            //    {   // Excel is busy
            //        Thread.Sleep(2500); // Wait, and...
            //        success = false;  // ...try again
            //        MessageBox.Show("W  A  I  T");

            //    }
            //    else
            //    {   // Re-throw!
            //        //throw e;
            //        MessageBox.Show("Exception Occured while Exporting: " + e.ToString());
            //    }

            //    // Dx8D07D6BA


            //    //catch (Exception ce)
            //    //{

            //    //{   // Excel is busy
            //    //MessageBox.Show("Excel is busy" + ce);
            //    Thread.Sleep(4000); // Wait, and...
            //    success = false;  // ...try again

            //    ErrMsg = e.Message;

            //    Process[] Processes;
            //    Processes = System.Diagnostics.Process.GetProcessesByName("EXCEL");
            //    foreach (System.Diagnostics.Process p in Processes)
            //    {
            //        if (p.MainWindowTitle.Trim() == "")
            //            p.Kill();
            //    }


            //}

            //Report1.IsEnabled = true;
            //Report2.IsEnabled = true;
            //Report3.IsEnabled = true;
            //Report4.IsEnabled = true;
            //Report5.IsEnabled = true;

            //this.Cursor = Cursors.Arrow;
        }

        private void CleanUpOldFiles(object sender, RoutedEventArgs e)
        {

            string appDir = Environment.CurrentDirectory;

            string[] filePaths = System.IO.Directory.GetFiles(@appDir + "\\Projects\\TrackingMaster\\", "*.xps");
            foreach (string filePath in filePaths)
            {
                //var result = MessageBox.Show("Delete All Text Files?", "Delete Verification", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (filePath.EndsWith(".xps"))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            filePaths = System.IO.Directory.GetFiles(@appDir + "\\Projects\\TrackingMaster\\", "Output_*.html");
            foreach (string filePath in filePaths)
            {
                //var result = MessageBox.Show("Delete All Text Files?", "Delete Verification", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (filePath.EndsWith(".html"))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        private void University_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if (System.IO.File.Exists(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath))
                {
                    if (University.SelectedIndex > 0)
                    {

                        var chkstr = University.SelectedValue.ToString();

                        if (!string.IsNullOrEmpty(University.SelectedValue.ToString()) && University.SelectedValue.ToString() != ml.ml_string(242, "Select Item"))
                        {

                            Project project = new Project(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

                            //MessageBox.Show("Check 2: " + University.SelectedValue.ToString());

                            IDbDriver db = project.CollectedData.GetDatabase();
                            System.Data.DataTable dt = db.GetTableData("Query19");

                            //IDbDriver db2 = project.CollectedData.GetDatabase();
                            //System.Data.DataTable dt2 = db.GetTableData("codeFieldSites27University_Training", "University_Training", "University_Training");

                            Cohort.Items.Clear();
                            Cohort.Items.Add(ml.ml_string(242,"Select Item"));
                            Cohort.SelectedIndex = 0;
                            //University.Items.Add("Select Item");

                            foreach (DataRow row in dt.Rows)
                            {
                                var chkvar = (row[2].ToString());
                                var chkvar2 = University.SelectedValue.ToString().Trim();

                                if (row[2].ToString().Trim() == University.SelectedValue.ToString().Trim())
                                {
                                    Cohort.Items.Add(row[1].ToString());
                                }
                            }


                            IDbDriver db2 = project.CollectedData.GetDatabase();
                            System.Data.DataTable dt2 = db.GetTableData("Query20");

                            Resident.Items.Clear();
                            Resident.Items.Add(ml.ml_string(242,"Select Item"));
                            Resident.SelectedIndex = 0;
                            //University.Items.Add("Select Item");

                            foreach (DataRow row2 in dt2.Rows)
                            {
                                var chkvarres = Int32.Parse(row2[1].ToString());
                                var chkvarres1 = row2[2].ToString().Trim();
                                //var chkvarres2 = Int32.Parse(Cohort.SelectedValue.ToString());
                                var chkvarres3 = University.SelectedValue.ToString().Trim();

                                if (chkvarres1 == chkvarres3)
                                {
                                    Resident.Items.Add(row2[3].ToString());
                                }
                            }

                            //if (Cohort.SelectedIndex > 0)
                            ////&& (Resident.SelectedIndex == 0))
                            //{
                            //    //MessageBox.Show("cohort test");
                            //    Cohort.SelectedValue = "Select Item";
                            //    Cohort.Text = "Select Item";
                            //}

                            //if (Resident.SelectedIndex > 0)
                            ////&& (Resident.SelectedIndex == 0))
                            //{
                            //    //MessageBox.Show("cohort test");
                            //    Resident.SelectedValue = "Select Item";
                            //    Resident.Text = "Select Item";
                            //}
                        }

                    if (University.SelectedValue.ToString() == ml.ml_string(242,"Select Item"))
                    {


                        if (System.IO.File.Exists(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath))
                        {
                            Project project = new Project(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath);

                            //MessageBox.Show("Check 2: " + University.SelectedValue.ToString());

                            IDbDriver db = project.CollectedData.GetDatabase();
                            System.Data.DataTable dt = db.GetTableData("Query21");

                            Cohort.Items.Clear();
                            Cohort.Items.Add(ml.ml_string(242,"Select Item"));
                            Cohort.SelectedIndex = 0;

                            foreach (DataRow row in dt.Rows)
                            {
                                Cohort.Items.Add(row[2].ToString());
                            }
                        }


                    }

                }
            }
            }
            catch
            {

            }
        }

        private void Resident_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("test1");
            //if (Cohort.SelectedIndex > 0) 
            //    //&& (Resident.SelectedIndex == 0))
            //{
            //    //MessageBox.Show("cohort test");
            //    Cohort.SelectedValue = "Select Item";
            //    Cohort.Text = "Select Item";
            //}

            //if (University.SelectedIndex > 0)
            ////&& (Resident.SelectedIndex == 0))
            //{
            //    //MessageBox.Show("cohort test");
            //    University.SelectedValue = "Select Item";
            //    University.Text = "Select Item";
            //}
        }

        private void Cohort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if (System.IO.File.Exists(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath))
                {
                    if (Cohort.SelectedIndex > 0)
                    {
                        if (!string.IsNullOrEmpty(Cohort.SelectedValue.ToString()) && Cohort.SelectedValue.ToString() != ml.ml_string(242, "Select Item"))
                        {

                            Project project = new Project(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);

                            var chkvar0 = Cohort.SelectedItem.ToString();

                            //MessageBox.Show("Check 2: " + chkvar3);

                            IDbDriver db = project.CollectedData.GetDatabase();
                            System.Data.DataTable dt = db.GetTableData("Query20");

                            //IDbDriver db2 = project.CollectedData.GetDatabase();
                            //System.Data.DataTable dt2 = db.GetTableData("codeFieldSites27University_Training", "University_Training", "University_Training");

                            Resident.Items.Clear();
                            Resident.Items.Add(ml.ml_string(242, "Select Item"));
                            Resident.SelectedIndex = 0;
                            //University.Items.Add("Select Item");

                            foreach (DataRow row in dt.Rows)
                            {
                                var chkvar = Int32.Parse(row[1].ToString());
                                var chkvar1 = row[2].ToString().Trim();
                                var chkvar2 = Int32.Parse(Cohort.SelectedValue.ToString());
                                var chkvar3 = University.SelectedValue.ToString().Trim();

                                if (chkvar == chkvar2 && ((chkvar3 != ml.ml_string(242, "Select Item") && chkvar1 == chkvar3) || (chkvar1 != chkvar3 && chkvar3 == ml.ml_string(242, "Select Item"))))
                                {
                                    var chkvar4 = row[3].ToString();
                                    Resident.Items.Add(row[3].ToString());
                                }
                            }

                            //MessageBox.Show("test2");
                            //if (Resident.SelectedIndex > 0) 
                            //    //&& (Cohort.SelectedIndex == 0))
                            //{
                            //    //MessageBox.Show("resident test");
                            //    Resident.SelectedValue = "Select Item";
                            //    Resident.Text = "Select Item";
                            //}

                            //if (University.SelectedIndex > 0)
                            ////&& (Cohort.SelectedIndex == 0))
                            //{
                            //    //MessageBox.Show("resident test");
                            //    University.SelectedValue = "Select Item";
                            //    University.Text = "Select Item";
                            //}
                        }
                        //}

                        if (Cohort.SelectedValue.ToString() == ml.ml_string(242, "Select Item"))
                        {

                            Project project = new Project(Environment.CurrentDirectory + "\\" + Properties.Settings.Default.ProjectPath);
                            IDbDriver db = project.CollectedData.GetDatabase();
                            System.Data.DataTable dt = db.GetTableData("Query20");

                            Resident.Items.Clear();
                            Resident.Items.Add(ml.ml_string(242, "Select Item"));
                            Resident.SelectedIndex = 0;
                            //University.Items.Add("Select Item");

                            foreach (DataRow row in dt.Rows)
                            {
                                var chkvar = Int32.Parse(row[1].ToString());
                                var chkvar1 = row[2].ToString().Trim();
                                //var chkvar2 = Int32.Parse(Cohort.SelectedValue.ToString());
                                var chkvar3 = University.SelectedValue.ToString().Trim();

                                if ((chkvar3 != ml.ml_string(242, "Select Item") && chkvar1 == chkvar3) || (chkvar1 != chkvar3 && chkvar3 == ml.ml_string(242, "Select Item")))
                                {
                                    var chkvar4 = row[3].ToString();
                                    Resident.Items.Add(row[3].ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }

        }

        private void RptChoice_Click(object sender, RoutedEventArgs e)
        {

            if (rptChoice.IsChecked == true)
            {

                if (chkFlag == true)
                {
                    rptChoice.IsChecked = false;
                }

                chkFlag = true;

            }
            //else

            if (rptChoice.IsChecked == false)
            {

                if (chkFlag == false)
                {
                    rptChoice.IsChecked = true;
                }

                chkFlag = false;

            }

        }

        private void RptChoice_Unchecked(object sender, RoutedEventArgs e)
        {
            if (rptChoice.IsChecked == true)
            {
                rptChoice.IsChecked = false;
            }
        }
    }

    //private void ArrowOUT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    //{
    //    if (CanvasSelection.Margin.Left == 0)
    //    {
    //        ChangeCriteria_flatColorButton.Content = "Modifier Les Critères";
    //        CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
    //        ChangeCriteria_flatColorButton.ToolTip = "Cliquez ici pour mettre à jour ou de modifier les rapports.";
    //        //ArrowIN.Visibility = System.Windows.Visibility.Hidden;
    //        //ArrowOUT.Visibility = System.Windows.Visibility.Visible;
    //    }
    //    else
    //    {
    //        ChangeCriteria_flatColorButton.Content = "Sous-Menu Collapse";
    //        CanvasSelection.Margin = new Thickness(0, 32, 0, 0);
    //        ChangeCriteria_flatColorButton.ToolTip = "Cliquez ici pour fermer ce menu";
    //        //ArrowIN.Visibility = System.Windows.Visibility.Visible;
    //        //ArrowOUT.Visibility = System.Windows.Visibility.Hidden;

    //    }
    //}

    //private void ArrowIN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    //{
    //    if (CanvasSelection.Margin.Left == 0)
    //    {
    //        ChangeCriteria_flatColorButton.Content = "Modifier Les Critères";
    //        CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
    //        ChangeCriteria_flatColorButton.ToolTip = "Cliquez ici pour mettre à jour ou de modifier les rapports.";
    //        //ArrowIN.Visibility = System.Windows.Visibility.Hidden;
    //        //ArrowOUT.Visibility = System.Windows.Visibility.Visible;
    //    }
    //    else
    //    {
    //        ChangeCriteria_flatColorButton.Content = "Sous-Menu Collapse";
    //        CanvasSelection.Margin = new Thickness(0, 32, 0, 0);
    //        ChangeCriteria_flatColorButton.ToolTip = "Cliquez ici pour fermer ce menu";
    //        //ArrowIN.Visibility = System.Windows.Visibility.Visible;
    //        //ArrowOUT.Visibility = System.Windows.Visibility.Hidden;

    //    }
    //}

    //private void OpenDataDictionary_Button_Click(object sender, RoutedEventArgs e)
    //{
    //    //Epi.Menu.EpiInfoMenuManager. dataDictionary(Environment.CurrentDirectory);

    //    // if (dashboardHelper != null)
    //    //{
    //    //    scrollViewerAnalysis.Visibility = Visibility.Collapsed;
    //    //    dataDictionary.Visibility = Visibility.Collapsed;
    //    //    dataDisplay.Visibility = Visibility.Visible;
    //    //}

    //    //dataDictionary.SetDataView(this.dashboardHelper.GenerateDataDictionaryTable().DefaultView);
    //    //dataDictionary.Refresh();
    //}

    //private void OpenDashboard_Button_Click(object sender, RoutedEventArgs e)
    //{
    //    Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory);
    //}

    //private void Maps_Click(object sender, RoutedEventArgs e)
    //{
    //    System.Diagnostics.Process.Start(@"C:\TFSCode\Mapping.exe");
    //}

    //private void MapFinalLabResult_Click(object sender, RoutedEventArgs e)
    //{
    //    //System.Diagnostics.Process.Start(@"C:\TFSCode\Mapping.exe");
    //    //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "\\Projects\\Ghana\\Region-District.cvs7");
    //    //Epi.Menu.EpiInfoMenuManager.OpenMapsWithMap(Projects\\Ghana\\Region-District.cvs)
    //}

    //    private void ClassificationMap_Click(object sender, RoutedEventArgs e)
    //    {
    //        //Epi.Menu.EpiInfoMenuManager.OpenMapsWithMap("Projects\\Menafrinet\\Reports\\DistrictMap.map7");
    //    }


    //    private void SettingsMenu_Click(object sender, RoutedEventArgs e)
    //    {
    //        //  Menu.MainWindow.VisibilityProperty = true(MainWindow.Window);

    //        //NavigationService.Navigate(new Uri("/Menafrinet.View;/Menu/MainWindow.xaml", UriKind.Relative));
    //        //NavigationService.Navigate(new Uri("/Menu/component/MainWindow.xaml", UriKind.Relative));

    //        //this.Frame.Navigate(typeof(MainWindow), null);
    //    }

    //    private void NmALineList_Click(object sender, RoutedEventArgs e)
    //    {

    //    }

    //    private void Maps_Copy_Click(object sender, RoutedEventArgs e)
    //    {

    //    }

    //    private void BtnIndicators_Click(object sender, RoutedEventArgs e)
    //    {
    //        CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
    //        documentViewer1.Visibility = Visibility.Hidden;
    //        browser2.Visibility = Visibility.Visible;
    //        ReportsUpdateTextbox.Visibility = Visibility.Hidden;
    //        string appDir = Environment.CurrentDirectory;
    //        Uri pageUri = new Uri(appDir + "\\Projects\\Menafrinet\\Reports\\PerformanceIndicators.html");
    //        browser2.Source = pageUri;
    //        //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "\\Projects\\Reports\\DataCleaning.cvs7");
    //        //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "/DataCleaning.cvs7");
    //        //System.Diagnostics.Process.Start("Analysis.exe");
    //    }

    //    private void BtnFinalResult_Click(object sender, RoutedEventArgs e)
    //    {
    //        CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
    //        documentViewer1.Visibility = Visibility.Hidden;
    //        browser2.Visibility = Visibility.Visible;
    //        ReportsUpdateTextbox.Visibility = Visibility.Hidden;
    //        string appDir = Environment.CurrentDirectory;
    //        Uri pageUri = new Uri(appDir + "\\Projects\\Menafrinet\\Reports\\FinalResultByDistrict.html");
    //        browser2.Source = pageUri;
    //        //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "\\Projects\\Reports\\DataCleaning.cvs7");
    //        //Epi.Menu.EpiInfoMenuManager.OpenDashboardWithCanvas(Environment.CurrentDirectory + "/DataCleaning.cvs7");
    //        //System.Diagnostics.Process.Start("Analysis.exe");
    //        //CanvasSelection.Margin = new Thickness(-170, 32, 0, 0);
    //        //documentViewer1.Visibility = Visibility.Hidden;
    //        //browser2.Visibility = Visibility.Visible;
    //        //ReportsUpdateTextbox.Visibility = Visibility.Hidden;
    //        //string appDir2 = Environment.CurrentDirectory;
    //        //Uri pageUri2 = new Uri(appDir + "\\Projects\\Menafrinet\\Reports\\FinalResultWeek.html");
    //        //browser2.Source = pageUri;
    //    }


    //    private void MapAttackRates_Click(object sender, RoutedEventArgs e)
    //    {
    //        //System.Diagnostics.Process.Start(@"C:\TFSCode\Mapping.exe");
    //    }

    //    private void FinalLabResultMap_Click(object sender, RoutedEventArgs e)
    //    {
    //        //string appDir = Environment.CurrentDirectory;
    //        //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
    //        //Epi.Menu.EpiInfoMenuManager.OpenAnalysisWithPGM(appDir + "\\Projects\\Menafrinet\\DistrictTab.pgm7");
    //        //Epi.Menu.EpiInfoMenuManager.OpenMapsWithMap(appDir + "\\Projects\\Menafrinet\\Reports\\Map1.map7");
    //    }



    //    //private void browser2_LoadCompleted(object sender, NavigationEventArgs e)
    //    //{
    //    //    mshtml.HTMLDocumentClass doc = (mshtml.HTMLDocumentClass)browser2.Document;
    //    //    mshtml.HTMLDocumentEvents2_Event iEvent = (mshtml.HTMLDocumentEvents2_Event)doc;
    //    //    iEvent.ondblclick += new mshtml.HTMLDocumentEvents2_ondblclickEventHandler(browser2_DoubleClick);
    //    //}

    //    //private void BtnBackupDatabase_Click(object sender, RoutedEventArgs e)
    //    //{
    //    //    File.Copy(Menafrinet_NE, Menafrinet_NEbak, true);
    //    //}



    //private readonly Project _project;
    //private string _htmlSource = String.Empty;


    //public string HtmlSource
    //{
    //    get
    //    {
    //        return _htmlSource;
    //    }
    //    set
    //    {
    //        _htmlSource = value;
    //        //RaisePropertyChanged("HtmlSource");                   //MLHIDE
    //    }
    //}

    //public ReportsViewModel(Project project)
    //{
    //    // pre
    //    Contract.Requires(project != null);

    //    // post
    //    Contract.Ensures(_project != null);

    //    _project = project;
    //    ReportFileNames = new ObservableCollection<FileInfo>();

    //    PopulateFileNames();
    //}


    //    PopulateFileNames();

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
            //Contract.Requires(project != null);

            // post
            //Contract.Ensures(_project != null);

            _project = project;
            ReportFileNames = new ObservableCollection<FileInfo>();

            //PopulateFileNames();
        }



        //public AnalysisPanel PopulateFileNames { get; set; }

    }
}
