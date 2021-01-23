using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Epi;
using Menafrinet.ViewModel;
using Menafrinet.View.Controls;
using Epi.Data;
using Epi.Enter;
using Epi.Windows.Enter;
using Epi.Windows;
using Epi.Fields;
using System.Data;
using FluidKit;
using Menafrinet.View.NavigationTransition;
using Epi.Core;
using Epi.Menu;
using System.IO;
using System.Xml;
using System.Data.OleDb;
using System.Threading;
//using Menu.View;
//using Menu.ViewModel;
//using Menu;


namespace Menafrinet.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
        enum Merged { Imported, Exported, Reset };
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        //Below for settings button to call Enter to load Menu Settings.
        public string currentDirectory = string.Empty;
        string appDir = Environment.CurrentDirectory;
        DataView DV;
        DataTable results;

        private DataHelper DataHelper
        {
            get
            {
                return (this.DataContext as DataHelper);
            }
        }

        public MainWindow()
        {
            try
            {
                InitializeComponent();


                //Uri pageUri = new Uri(appDir + "/Header.mht");
                //browser.Source = pageUri;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void SetTabButtons(TabButton tb)
        {
            // TODO: Move these things to their own tab header custom control
            foreach (UIElement element in grdMenuItems.Children)
            {
                if (element is TabButton && (element as TabButton) != tb)
                {
                    (element as TabButton).IsChecked = false;
                }
            }
        }

        private void btnTab_Checked(object sender, RoutedEventArgs e)
        {
            // TODO: Move these things to their own tab header custom control
            SetTabButtons(sender as TabButton);

        }

        private void btnTab_Unchecked(object sender, RoutedEventArgs e)
        {
            // TODO: Move these things to their own tab header custom control
            int checkedItems = 0;
            foreach (UIElement element in grdMenuItems.Children)
            {
                if (((TabButton)element).IsChecked == true) checkedItems++;
            }

            if (checkedItems == 0)
            {
                // TabButton is derived from ToggleButton, and in this case, we don't want the user to be able to 'uncheck'
                // a tab item... so handle that here
                (sender as TabButton).IsChecked = true;
                e.Handled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
                System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
                //lblVersion.Content = a.GetName().Version;
                if (this.WindowState == System.Windows.WindowState.Minimized) this.WindowState = System.Windows.WindowState.Normal;
                DataHelper.ProjectPath = Properties.Settings.Default.ProjectPath;
                DataHelper.RepopulateCollections();

                object LimitedAccess = EpiInfoMenuManager.GetPermanentVariableValue("PermLimitedAccess");

                if (LimitedAccess != null)
                {
                    if (LimitedAccess.ToString() == "Yes")
                    {
                        btnTabResidents.IsEnabled = false;
                        btnTabConsultants.IsEnabled = false;
                    }
                    else
                    {
                        btnTabConsultants.IsEnabled = true;
                        btnTabResidents.IsEnabled = true;
                    }

                }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // TODO: Must be a better way of getting these things to auto-size to Window height. This shouldn't require code behind.
            //panelDataEntry.ResetHeight();
            //panelMerge.ResetHeight();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }
        private void cmbSystemLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbDistricts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //This is needed for Enter to open form.
            currentDirectory = Directory.GetCurrentDirectory();
            string EI7 = currentDirectory.Substring(currentDirectory.Length - 10, 10);
            if (!(EI7 == "Epi Info 7"))
            {
                if (Directory.Exists(currentDirectory + "\\Epi Info 7"))
                    Directory.SetCurrentDirectory(currentDirectory + "\\Epi Info 7");
                currentDirectory = Directory.GetCurrentDirectory();
            }

        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {


        }


        private void btnEpiInfoLoad(object sender, RoutedEventArgs e)
        {
            //System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            //string commandText = System.IO.Path.GetDirectoryName(a.Location) + "\\EpiInfo.exe";

            //System.Diagnostics.Process.Start(@"C:\TFSCode\EpiInfo.exe");

            System.Diagnostics.Process.Start("EpiInfo.exe");
        }




        private void Home_Click(object sender, RoutedEventArgs e)
        {

            //MainWindow frm = new MainWindow();
            //frm.Show();

            btnTabHome.IsChecked = true;
            btnTabAnalysis.IsChecked = false;
            btnTabEntry.IsChecked = false;
            btnTabMerge.IsChecked = false;
            //btnTabResident.IsChecked = false;
            btnTabMerge.IsChecked = false;
            //btnTabStatus.IsChecked = false;
            btnTabTransmit.IsChecked = false;
        }

        //No longer using this Button for Editing regions, now done through EpiInfo
        //private void btnRegionEditor_Click(object sender, RoutedEventArgs e)
        //{
        //    regionDistrictNameEditor.Init(this.DataHelper);
        //}

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mnuSiteSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mnuRegionDistrictEditor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mnuPopulationEditor_Click(object sender, RoutedEventArgs e)
        {
            //PhotoIntro window = new PhotoIntro();
            //window.Show();
            ////this.Close();
        }

        private void mnuApplicationTools_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start("EpiInfo.exe");
        }

        private void MenuItemFieldSites_Click(object sender, RoutedEventArgs e)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            string appDir = Environment.CurrentDirectory;
            Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
            string commandText = appDir + "\\Enter.exe";

            if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"))
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = commandText;
                proc.StartInfo.Arguments = "/project: Projects\\TrackingMaster\\TrackingMasterNew.prj /view:codeFieldSites";
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
                proc.WaitForExit();

                Thread.Sleep(2500);

                FillResidents_Sub();

                DataHelper.LoadSiteMergeData();

                ((AnalysisPanel)Application.Current.Windows[1].FindName("panelAnalysis")).PopulateFileNames();
            }
        }

        object permvar2;

        private void MenuItemUserSettings_Click(object sender, RoutedEventArgs e)
        {
            //object permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident");
            //ResidentNameTextBox.Text = permvar2.ToString();

            //  LanguageSettings_Perm1
            //try
            //{


            object LimitedAccess = EpiInfoMenuManager.GetPermanentVariableValue("PermLimitedAccess");

            if (LimitedAccess != null)
            {
                if (LimitedAccess.ToString() == "Yes")
                {
                    btnTabResidents.IsEnabled = false;
                    btnTabConsultants.IsEnabled = false;
                }
                else
                {
                    btnTabConsultants.IsEnabled = true;
                    btnTabResidents.IsEnabled = true;
                }

            }


            object langvar1 = EpiInfoMenuManager.GetPermanentVariableValue("LanguageSettings_Perm");
            object permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident");
            string Language_Perm_str1 = "Default";


            if (langvar1 is null || langvar1.ToString() == "")
            {
                Language_Perm_str1 = "English";
                //.Text = Properties.Resources.Language0 + langvar1.ToString();
            }
            else
            {
                Language_Perm_str1 = langvar1.ToString();
            }

            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            string appDir = Environment.CurrentDirectory;
            Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
            string commandText = appDir + "\\Enter.exe";

            if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"))
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = commandText;
                proc.StartInfo.Arguments = "/project: Projects\\TrackingMaster\\TrackingMasterNew.prj /view:UserSettings";
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
                proc.WaitForExit();

                //LoadConfig();
                Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
                permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident");
                //ResidentNameTextBox.Text = Properties.Resources.Resident_Name0 + permvar2.ToString();

                object permvar3 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm");

                Properties.Settings.Default.SiteCode = permvar3.ToString();

                if (permvar3.ToString() == "3")
                {
                    permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident");
                    Menafrinet.View.Properties.Settings.Default.SiteName = permvar2.ToString();
                }

                if (permvar3.ToString() == "2")
                {
                    permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC");
                    Menafrinet.View.Properties.Settings.Default.SiteName = permvar2.ToString();
                }

                if (permvar3.ToString() == "1")
                {
                    permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC");
                    Menafrinet.View.Properties.Settings.Default.SiteName = permvar2.ToString();
                }

                if (permvar3.ToString() != "1" && permvar3.ToString() != "2" && permvar3.ToString() != "3")
                {
                    Menafrinet.View.Properties.Settings.Default.SiteName = "FETP Office";
                }


                //  LanguageSettings_Perm
                //object langvar2 = EpiInfoMenuManager.GetPermanentVariableValue("LanguageSettings_Perm");

                ////LanguageSettingsTextBox.Text = "Language: " + langvar2.ToString();

                //string Language_Perm_str2 = "default";

                //Language_Perm_str2 = langvar2.ToString();

                //if (Language_Perm_str2 != "")
                //{
                //    Language_Perm_str2 = langvar2.ToString();
                //}
                //else
                //{
                //    Language_Perm_str2 = "English";
                //}

                //OleDbConnection connect = new OleDbConnection();
                //connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Projects\TrackingMaster\TrackingMasterNew.MDB"; // @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Projects\Menafrinet\Menafrinet_NE.MDB";

                //connect.Open();

                //OleDbCommand command = new OleDbCommand();
                //command.Connection = connect;


                // gfj for reading all of the code tables 
                //if (Language_Perm_str == "English")
                //{
                //    command.CommandText = "UPDATE UserSettings29LanguageTranslation INNER JOIN metaFields ON UserSettings29LanguageTranslation.FieldName = metaFields.Name SET metaFields.PromptText = [UserSettings29LanguageTranslation].[TypeLabelEnglish];";
                //    command.ExecuteNonQuery();

                //    Project project = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);
                //    IDbDriver db = project.CollectedData.GetDatabase();

                //    var codeTableNames = db.GetTableNames().Where(t => t.StartsWith("code"));

                //    foreach (var tableName in codeTableNames)
                //    {
                //        var values = db.GetTableData(tableName);
                //        var fieldnames = db.GetTableColumnNames(tableName);
                //        //string codefieldname = values.TableName.ToString();
                //        //string FieldNameValue values
                //        String codefieldname = tableName + "." +(fieldnames[0].ToString());
                //        String tablename = tableName;
                //        // iterate over data rows
                //        //command.CommandText = "UPDATE UserSettingscodeTableNames RIGHT JOIN " + tablename + " ON (UserSettingscodeTableNames.codeTableValueSpanish = " + codefieldname + ") AND (UserSettingscodeTableNames.codeTableValueFrench = " + codefieldname + ") SET " + codefieldname + " = UserSettingscodeTableNames].[codeTableValueEnglish];";
                //        command.CommandText = "UPDATE " + tablename + " INNER JOIN UserSettingscodeTableNames ON " + tablename + "." + codefieldname + " = UserSettingscodeTableNames.codeTableValueSpanish SET codetype2.Type = [UserSettingscodeTableNames].[codeTableValueEnglish];";
                //        command.CommandText = "UPDATE " + tablename + " INNER JOIN UserSettingscodeTableNames ON " + tablename + "." + codefieldname + " = UserSettingscodeTableNames.codeTableValueFrench SET codetype2.Type = [UserSettingscodeTableNames].[codeTableValueEnglish];";
                //        //command.CommandText = "UPDATE codetype2 INNER JOIN UserSettingscodeTableNames ON codetype2.Type = UserSettingscodeTableNames.codeTableValueSpanish SET codetype2.Type = [UserSettingscodeTableNames].[codeTableValueFrench];";

                //        command.ExecuteNonQuery();

                //    }
                //}

                //if (Language_Perm_str1 != Language_Perm_str2)
                //{


                //command.CommandText = "UPDATE code_diseaseenglish INNER JOIN UserSettings36DiseaseTranslation ON code_diseaseenglish.Disease = UserSettings36DiseaseTranslation.Disease" + Language_Perm_str1 + " SET code_diseaseenglish.Disease = [UserSettings36DiseaseTranslation].[Disease" + Language_Perm_str2 + "];";
                //command.ExecuteNonQuery();

                //command.CommandText = "UPDATE UserSettings29LanguageTranslation INNER JOIN metaFields ON UserSettings29LanguageTranslation.FieldName = metaFields.Name SET metaFields.PromptText = [UserSettings29LanguageTranslation].[TypeLabel" + Language_Perm_str2 + "];";
                //command.ExecuteNonQuery();

                //Project project = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);
                //IDbDriver db = project.CollectedData.GetDatabase();
                //System.Data.DataTable dt = db.GetTableData("UserSettings35CommentLegalFields");

                ////var codeTableNames = db.GetTableNames().Where(t => t.StartsWith("code"));

                //// permanent variables error !! 5/23/2018
                //foreach (DataRow row in dt.Rows)
                //{
                //    var tablename = row[5].ToString();
                //    var codefieldname = row[6].ToString();
                //    command.CommandText = "UPDATE " + tablename + " INNER JOIN UserSettings35CommentLegalDropdownLists ON " + tablename + "." + codefieldname + " = UserSettings35CommentLegalDropdownLists.codeTableValue" + Language_Perm_str1 + " SET " + tablename + "." + codefieldname + " = [UserSettings35CommentLegalDropdownLists].[codeTableValue" + Language_Perm_str2 + "];";
                //    command.ExecuteNonQuery();
                //}

                //DataTable dt2 = db.GetTableData("MetaViews");
                //DataTable dt3 = db.GetTableData("UserSettings36DialogTranslation");

                //foreach (DataRow row2 in dt2.Rows)
                //{
                //    string MyViewID = row2[0].ToString();
                //    string MyViewName = row2[1].ToString();
                //    string CheckCode = row2[3].ToString();

                //    foreach (DataRow dr in dt3.Rows)
                //    {

                //        //    foreach (DataColumn dc in dt3.Columns)
                //        //{
                //        string LanguageCurrent = "Dialog" + Language_Perm_str1;
                //        string LanguageSelected = "Dialog" + Language_Perm_str2;
                //        string OldTranslation = dr[LanguageCurrent].ToString();
                //        string NewTranslation = dr[LanguageSelected].ToString();

                //        if (CheckCode.Contains(OldTranslation))
                //        {
                //            //MessageBox.Show("Found IT1: " + OldTranslation + " -- " + NewTranslation);
                //            string NewCheckCode = CheckCode.Replace(OldTranslation, NewTranslation);
                //            OleDbCommand command2 = new OleDbCommand(@"UPDATE metaviews
                //                SET CheckCode = @NewCheckCode
                //                WHERE ([ViewID] = MyViewID AND [Name] = MyViewName)", connect);

                //            command2.Parameters.AddWithValue("@NewCheckCode", NewCheckCode);
                //            command2.Parameters.AddWithValue("@MyViewID", MyViewID);
                //            command2.Parameters.AddWithValue("@MyViewName", MyViewName);
                //            command2.ExecuteNonQuery();

                //        }
                //    }
                //}

                //connect.Close();

                // Configuration

                //string EILangSetting = "English";
                //string EIRepresentationOfYes = "Yes";
                //string EIRepresentationOfNo = "No";
                //string EIRepresentationOfMissing = "Missing";

                //switch (langvar2)
                //{
                //    case "Ukrainian":
                //        EILangSetting = "uk";
                //        EIRepresentationOfYes = "Так";
                //        EIRepresentationOfNo = "ні";
                //        EIRepresentationOfMissing = "Відсутні";
                //        MessageBox.Show("Налаштування додатка були змінені. Зміни мови та налаштувань будуть повністю набрані після наступного запуску програми. Закрийте програму та перезапустіть. ", " Сповіщення про відстеження проекту: ");
                //        break;
                //    case "French":
                //        EILangSetting = "fr";
                //        EIRepresentationOfYes = "Oui";
                //        EIRepresentationOfNo = "Non";
                //        EIRepresentationOfMissing = "Manquant";
                //        MessageBox.Show("Les paramètres de l'application ont été modifiés. Les changements de langue et de paramètres prendront plein effet la prochaine fois que vous démarrerez le programme. Veuillez fermer l'application et redémarrer.", "Alerte de Suivi de Projet:");
                //        break;
                //    case "Spanish":
                //        EILangSetting = "es-ES";
                //        EIRepresentationOfYes = "Si";
                //        EIRepresentationOfNo = "No";
                //        EIRepresentationOfMissing = "Falta";
                //        MessageBox.Show("La configuración de la aplicación ha sido modificada. Los cambios de idioma y configuración tendrán pleno efecto la próxima vez que inicie el programa. Por favor cierre la aplicación y reinicie. ", " Alerta de Seguimiento del Proyecto: ");
                //        break;
                //    case "Portuguese":
                //        EILangSetting = "pt";
                //        EIRepresentationOfYes = "Sim";
                //        EIRepresentationOfNo = "Não";
                //        EIRepresentationOfMissing = "Ausente";
                //        MessageBox.Show("As configurações do aplicativo foram alteradas. As alterações de idioma e configurações entrarão em vigor na próxima vez que você iniciar o programa. Por favor, feche o aplicativo e reinicie.", " Alerta de Monitoramento do Projeto: ");
                //        break;
                //    default:
                //        EILangSetting = "en-US";
                //        EIRepresentationOfYes = "Yes";
                //        EIRepresentationOfNo = "No";
                //        EIRepresentationOfMissing = "Missing";
                        MessageBox.Show("The app settings have been changed. The user setting changes will take full effect the next time you start the program. Please close the application and restart.", "ReDPeTT Alert: ");
                //        break;
                //}



                //XmlTextReader settings_xml = new XmlTextReader("Configuration\\EpiInfo.Config.xml");
                //XmlDocument settings_doc = new XmlDocument();
                //settings_doc.Load(settings_xml);
                //settings_xml.Close();
                //XmlNode criterionNode;
                //XmlElement root = settings_doc.DocumentElement;

                //criterionNode = root.SelectSingleNode("Settings/Language");
                ////criterionNode.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.InnerXml = "<Language>" + "en-US" + "</Language>";
                //criterionNode.InnerXml = EILangSetting;
                //criterionNode = root.SelectSingleNode("Settings/RepresentationOfYes");
                //criterionNode.InnerXml = EIRepresentationOfYes;
                //criterionNode = root.SelectSingleNode("Settings/RepresentationOfNo");
                //criterionNode.InnerXml = EIRepresentationOfNo;
                //criterionNode = root.SelectSingleNode("Settings/RepresentationOfMissing");
                //criterionNode.InnerXml = EIRepresentationOfMissing;
                //settings_doc.Save("Configuration\\EpiInfo.Config.xml");


                string connString =
                    "Provider=Microsoft.Jet.OLEDB.4.0" +
                    ";data Source=" + "Projects\\TrackingMaster\\TrackingMasterNew.mdb";


                results = new DataTable();


                using (OleDbConnection conn = new OleDbConnection(connString))
                {

                    OleDbCommand cmd = new OleDbCommand("SELECT UserSettings111FieldstoRemove.FieldName, UserSettings111FieldstoRemove.NameofTable FROM UserSettings111FieldstoRemove; ", conn);

                    conn.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    adapter.Fill(results);
                    //dgRecordsResidents.ItemsSource = results.DefaultView;
                    //adapter.Update(results);

                    DV = new DataView(results);
                    //DV.RowFilter = string.Format("FullName LIKE '%{0}%' or CohortNumber LIKE '%{0}%'  or completeFETP LIKE '%{0}%' or UniversityTrainingInstitution LIKE '%{0}%'or FETPTrackLabel LIKE '%{0}%'", txtSearchResidents.Text);
                    //string DeIdentiyFields;
                    string DeIdentiyFields = "ProjectsDeIdentified";

                    XmlTextReader settings_xml = new XmlTextReader("Projects\\TrackingMaster\\ImportExport\\" + DeIdentiyFields + ".pks7");
                    XmlDocument settings_doc = new XmlDocument();
                    settings_doc.Load(settings_xml);
                    XmlNodeList nodeList = settings_doc.DocumentElement.SelectNodes("/ProjectPackagingScript/fieldsToNull");
                    settings_xml.Close();
                    XmlNode criterionNode;
                    XmlElement root = settings_doc.DocumentElement;
                    criterionNode = root.SelectSingleNode("fieldsToNull");

                    foreach (DataRowView drv in DV)
                    {
                        bool AddField = true;


                        XmlReader xmlFile;
                        xmlFile = XmlReader.Create("Projects\\TrackingMaster\\ImportExport\\" + DeIdentiyFields + ".pks7", new XmlReaderSettings());
                        DataSet ds = new DataSet();
                        DataView dv;
                        ds.ReadXml(xmlFile);

                        dv = new DataView(ds.Tables[0]);
                        dv.Sort = "Product_Name";
                        int index = dv.Find(drv[0].ToString());


                        listBox1.Items.Add("<" + xmlReader.Name + ">");


                        //MessageBox.Show(node.SelectSingleNode("leadcohort").InnerText.Count().ToString());
                        //MessageBox.Show(node.InnerText.ToString());
                        //District d = new District(siteVM.SiteName, siteVM.SiteCode, region);
                        //districts.Sites.Add(d);



                        //MessageBox.Show(node.InnerText.ToString());

                        //node.SelectSingleNode("Product_id").InnerText = d

                        //criterionNode.InnerXml ="< fieldToNull formName = Projects >" + drv[0].ToString() + "</ fieldToNull >";
                        //criterionNode.InnerXml = drv[0].ToString();

                    }

                    settings_doc.Save("Projects\\TrackingMaster\\ImportExport\\" + DeIdentiyFields + ".pks7");

                }
            }
            else
            {
                MessageBox.Show("This project was not installed in the correctly. Please ensure the application is installed correctly");
            }
            
            SetTabs_Sub();

        }

        private void LanguageSettings_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            object langvar1 = EpiInfoMenuManager.GetPermanentVariableValue("LanguageSettings_Perm");
            object permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident");
            string Language_Perm_str1 = "Default";


            if (langvar1 is null || langvar1.ToString() == "")
            {
                Language_Perm_str1 = "English";
                //.Text = Properties.Resources.Language0 + langvar1.ToString();
            }
            else
            {
                Language_Perm_str1 = langvar1.ToString();
            }

            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            string appDir = Environment.CurrentDirectory;
            Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
            string commandText = appDir + "\\Enter.exe";

            if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"))
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = commandText;
                proc.StartInfo.Arguments = "/project: Projects\\TrackingMaster\\TrackingMasterNew.prj /view:LanguageSettings";
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
                proc.WaitForExit();

                //LoadConfig();
                Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
                permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident");
                //ResidentNameTextBox.Text = Properties.Resources.Resident_Name0 + permvar2.ToString();

                object permvar3 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm");

                //Properties.Settings.Default.SiteCode = permvar3.ToString();

                //if (permvar3.ToString() == "3")
                //{
                //    permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident");
                //    Menafrinet.View.Properties.Settings.Default.SiteName = permvar2.ToString();
                //}

                //if (permvar3.ToString() == "2")
                //{
                //    permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC");
                //    Menafrinet.View.Properties.Settings.Default.SiteName = permvar2.ToString();
                //}

                //if (permvar3.ToString() == "1")
                //{
                //    permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC");
                //    Menafrinet.View.Properties.Settings.Default.SiteName = permvar2.ToString();
                //}

                //if (permvar3.ToString() != "1" && permvar3.ToString() != "2" && permvar3.ToString() != "3")
                //{
                //    Menafrinet.View.Properties.Settings.Default.SiteName = "FETP Office";
                //}


                //  LanguageSettings_Perm
                object langvar2 = EpiInfoMenuManager.GetPermanentVariableValue("LanguageSettings_Perm");

                //LanguageSettingsTextBox.Text = "Language: " + langvar2.ToString();

                string Language_Perm_str2 = "default";

                Language_Perm_str2 = langvar2.ToString();

                if (Language_Perm_str2 != "")
                {
                    Language_Perm_str2 = langvar2.ToString();
                }
                else
                {
                    Language_Perm_str2 = "English";
                }

                OleDbConnection connect = new OleDbConnection();
                connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Projects\TrackingMaster\TrackingMasterNew.MDB"; // @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Projects\Menafrinet\Menafrinet_NE.MDB";

                connect.Open();

                OleDbCommand command = new OleDbCommand();
                command.Connection = connect;


                // gfj for reading all of the code tables 
                //if (Language_Perm_str == "English")
                //{
                //    command.CommandText = "UPDATE UserSettings29LanguageTranslation INNER JOIN metaFields ON UserSettings29LanguageTranslation.FieldName = metaFields.Name SET metaFields.PromptText = [UserSettings29LanguageTranslation].[TypeLabelEnglish];";
                //    command.ExecuteNonQuery();

                //    Project project = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);
                //    IDbDriver db = project.CollectedData.GetDatabase();

                //    var codeTableNames = db.GetTableNames().Where(t => t.StartsWith("code"));

                //    foreach (var tableName in codeTableNames)
                //    {
                //        var values = db.GetTableData(tableName);
                //        var fieldnames = db.GetTableColumnNames(tableName);
                //        //string codefieldname = values.TableName.ToString();
                //        //string FieldNameValue values
                //        String codefieldname = tableName + "." +(fieldnames[0].ToString());
                //        String tablename = tableName;
                //        // iterate over data rows
                //        //command.CommandText = "UPDATE UserSettingscodeTableNames RIGHT JOIN " + tablename + " ON (UserSettingscodeTableNames.codeTableValueSpanish = " + codefieldname + ") AND (UserSettingscodeTableNames.codeTableValueFrench = " + codefieldname + ") SET " + codefieldname + " = UserSettingscodeTableNames].[codeTableValueEnglish];";
                //        command.CommandText = "UPDATE " + tablename + " INNER JOIN UserSettingscodeTableNames ON " + tablename + "." + codefieldname + " = UserSettingscodeTableNames.codeTableValueSpanish SET codetype2.Type = [UserSettingscodeTableNames].[codeTableValueEnglish];";
                //        command.CommandText = "UPDATE " + tablename + " INNER JOIN UserSettingscodeTableNames ON " + tablename + "." + codefieldname + " = UserSettingscodeTableNames.codeTableValueFrench SET codetype2.Type = [UserSettingscodeTableNames].[codeTableValueEnglish];";
                //        //command.CommandText = "UPDATE codetype2 INNER JOIN UserSettingscodeTableNames ON codetype2.Type = UserSettingscodeTableNames.codeTableValueSpanish SET codetype2.Type = [UserSettingscodeTableNames].[codeTableValueFrench];";

                //        command.ExecuteNonQuery();

                //    }
                //}

                //if (Language_Perm_str1 != Language_Perm_str2)
                //{
                command.CommandText = "UPDATE code_diseaseenglish INNER JOIN UserSettings70DiseaseTranslation ON code_diseaseenglish.Disease = UserSettings70DiseaseTranslation.Disease" + Language_Perm_str1 + " SET code_diseaseenglish.Disease = [UserSettings70DiseaseTranslation].[Disease" + Language_Perm_str2 + "];";
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE UserSettings66LanguageTranslation INNER JOIN metaFields ON UserSettings66LanguageTranslation.FieldName = metaFields.Name SET metaFields.PromptText = [UserSettings66LanguageTranslation].[TypeLabel" + Language_Perm_str2 + "];";
                command.ExecuteNonQuery();

                //command.CommandText = "UPDATE LanguageSettings96LanguageTranslation INNER JOIN metaFields ON LanguageSettings96LanguageTranslation.FieldName = metaFields.Name SET metaFields.PromptText = IIf([LanguageSettings96LanguageTranslation].[TypeLabel" + Language_Perm_str2 + "]is null, [LanguageSettings96LanguageTranslation].[TypeLabelEnglish], [LanguageSettings96LanguageTranslation].[TypeLabel" + Language_Perm_str2 + "]);";
                command.ExecuteNonQuery();

                Project project = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);
                IDbDriver db = project.CollectedData.GetDatabase();
                System.Data.DataTable dt = db.GetTableData("LanguageSettings97CommentLegalFields");

                //var codeTableNames = db.GetTableNames().Where(t => t.StartsWith("code"));

                // permanent variables error !! 5/23/2018
                //try
                //{
                //    foreach (DataRow row in dt.Rows)
                //    {
                //        var tablename = row[5].ToString();
                //        var codefieldname = row[6].ToString();
                //        command.CommandText = "UPDATE " + tablename + " INNER JOIN LanguageSettings98CommentLegalDropdownLists ON " + tablename + "." + codefieldname + " = LanguageSettings98CommentLegalDropdownLists.codeTableValue" + Language_Perm_str1 + " SET " + tablename + "." + codefieldname + " = [LanguageSettings98CommentLegalDropdownLists].[codeTableValue" + Language_Perm_str2 + "];";
                //        command.ExecuteNonQuery();
                //    }

                //DataTable dt2 = db.GetTableData("MetaViews");
                //DataTable dt3 = db.GetTableData("LanguageSettings99DialogTranslation");

                //foreach (DataRow row2 in dt2.Rows)
                //{
                //    string MyViewID = row2[0].ToString();
                //    string MyViewName = row2[1].ToString();
                //    string CheckCode = row2[3].ToString();

                //    foreach (DataRow dr in dt3.Rows)
                //    {

                //        //    foreach (DataColumn dc in dt3.Columns)
                //        //{
                //        string LanguageCurrent = "Dialog" + Language_Perm_str1;
                //        string LanguageSelected = "Dialog" + Language_Perm_str2;
                //        string OldTranslation = dr[LanguageCurrent].ToString();
                //        string NewTranslation = dr[LanguageSelected].ToString();

                //            if (CheckCode.Contains(OldTranslation))
                //            {
                //                //MessageBox.Show("Found IT1: " + OldTranslation + " -- " + NewTranslation);

                //                 // gfj

                //                string NewCheckCode = CheckCode.Replace(OldTranslation, NewTranslation);
                //                OleDbCommand command2 = new OleDbCommand(@"UPDATE metaviews
                //                    SET CheckCode = @NewCheckCode
                //                    WHERE ([ViewID] = MyViewID AND [Name] = MyViewName)", connect);

                //                command2.Parameters.AddWithValue("@NewCheckCode", NewCheckCode);
                //                command2.Parameters.AddWithValue("@MyViewID", MyViewID);
                //                command2.Parameters.AddWithValue("@MyViewName", MyViewName);
                //                command2.ExecuteNonQuery();

                //            }
                //        }

                //    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //}

                connect.Close();

                // Configuration

                string EILangSetting = "English";
                string EIRepresentationOfYes = "Yes";
                string EIRepresentationOfNo = "No";
                string EIRepresentationOfMissing = "Missing";

                switch (langvar2)
                {
                    case "Ukrainian":
                        EILangSetting = "uk";
                        EIRepresentationOfYes = "Так";
                        EIRepresentationOfNo = "ні";
                        EIRepresentationOfMissing = "Відсутні";
                        MessageBox.Show("Налаштування додатка були змінені. Зміни мови та налаштувань будуть повністю набрані після наступного запуску програми. Закрийте програму та перезапустіть. ", " Сповіщення про відстеження проекту: ");
                        break;
                    case "French":
                        EILangSetting = "fr";
                        EIRepresentationOfYes = "Oui";
                        EIRepresentationOfNo = "Non";
                        EIRepresentationOfMissing = "Manquant";
                        MessageBox.Show("Les paramètres de l'application ont été modifiés. Les changements de langue et de paramètres prendront plein effet la prochaine fois que vous démarrerez le programme. Veuillez fermer l'application et redémarrer.", "Alerte de Suivi de Projet:");
                        break;
                    case "Spanish":
                        EILangSetting = "es-ES";
                        EIRepresentationOfYes = "Si";
                        EIRepresentationOfNo = "No";
                        EIRepresentationOfMissing = "Falta";
                        MessageBox.Show("La configuración de la aplicación ha sido modificada. Los cambios de idioma y configuración tendrán pleno efecto la próxima vez que inicie el programa. Por favor cierre la aplicación y reinicie. ", " Alerta de Seguimiento del Proyecto: ");
                        break;
                    case "Portuguese":
                        EILangSetting = "pt";
                        EIRepresentationOfYes = "Sim";
                        EIRepresentationOfNo = "Não";
                        EIRepresentationOfMissing = "Ausente";
                        MessageBox.Show("As configurações do aplicativo foram alteradas. As alterações de idioma e configurações entrarão em vigor na próxima vez que você iniciar o programa. Por favor, feche o aplicativo e reinicie.", " Alerta de Monitoramento do Projeto: ");
                        break;
                    default:
                        EILangSetting = "en-US";
                        EIRepresentationOfYes = "Yes";
                        EIRepresentationOfNo = "No";
                        EIRepresentationOfMissing = "Missing";
                        MessageBox.Show("The app settings have been changed. The language and settings changes will take full effect the next time you start the program. Please close the application and restart.", "ReDPeTT Alert: ");
                        break;
                }



                XmlTextReader settings_xml = new XmlTextReader("Configuration\\EpiInfo.Config.xml");
                XmlDocument settings_doc = new XmlDocument();
                settings_doc.Load(settings_xml);
                settings_xml.Close();
                XmlNode criterionNode;
                XmlElement root = settings_doc.DocumentElement;

                criterionNode = root.SelectSingleNode("Settings/Language");
                //criterionNode.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.InnerXml = "<Language>" + "en-US" + "</Language>";
                criterionNode.InnerXml = EILangSetting;
                criterionNode = root.SelectSingleNode("Settings/RepresentationOfYes");
                criterionNode.InnerXml = EIRepresentationOfYes;
                criterionNode = root.SelectSingleNode("Settings/RepresentationOfNo");
                criterionNode.InnerXml = EIRepresentationOfNo;
                criterionNode = root.SelectSingleNode("Settings/RepresentationOfMissing");
                criterionNode.InnerXml = EIRepresentationOfMissing;
                settings_doc.Save("Configuration\\EpiInfo.Config.xml");
            }
            else
            {
                MessageBox.Show("This project was not installed in the correctly. Please ensure the application is installed correctly");
            }

            //FillResidents_Sub();
        }



        private void FillResidents_Sub()
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
            DataTable dtDistricts = dbDistricts.GetTableData("codeFieldSites27RAStatus");
         
            int cnt = 0;

            using (StreamWriter outputFile = new StreamWriter(appDir + "\\Projects\\TrackingMaster\\SiteMergeStatus.xml"))
            {
                string quotes = "\"";
                //string countryname = country;

                //outputFile.WriteLine("<?xml version=" + quotes + "1.0" + quotes + "?>");
                //outputFile.WriteLine("<Districts Country=" + quotes + "Mali" + quotes + ">");
                outputFile.WriteLine("<Districts Country=" + quotes + "ResidentsList" + quotes + ">");


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

        private void SetTabs_Sub()
        {
            object LimitedAccess = EpiInfoMenuManager.GetPermanentVariableValue("PermLimitedAccess");

            if (LimitedAccess.ToString() == "Yes")
            {
                btnTabResidents.IsEnabled = false;
                btnTabConsultants.IsEnabled = false;
            }
            else
            {
                btnTabConsultants.IsEnabled = true;
                btnTabResidents.IsEnabled = true;
            }
        }

    }
}
