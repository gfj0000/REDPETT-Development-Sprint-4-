using Epi;
using Epi.Data;
using Epi.Menu;
using Menafrinet.View.Controls;
using Menafrinet.ViewModel;
using MultiLang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml;
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
        object permvar3;
        object permvar2;
        int PW;
        int PW2;
        Double PW3;
        int PWcnt;
        double chkbot;
        int StartCheckboxPerm_num;
        int StartCheckbox2Perm_num;

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
            try
            { // gfj0 fix 12/10/2020
                //string configFilePath = Configuration.DefaultConfigurationPath;
                //bool configurationOk = true;
                string appDir = Environment.CurrentDirectory;
                Configuration.Load(appDir + "\\Configuration\\EpiInfo.config.xml"); //MLHIDE

                System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
                //lblVersion.Content = a.GetName().Version;
                if (this.WindowState == System.Windows.WindowState.Minimized) this.WindowState = System.Windows.WindowState.Normal;
                DataHelper.ProjectPath = Properties.Settings.Default.ProjectPath; //gfj0 12/10/2020 fix entire path for config file
                DataHelper.RepopulateCollections();

                //btnVideos.Visibility = Visibility.Visible;

                object LimitedAccess = EpiInfoMenuManager.GetPermanentVariableValue("PermLimitedAccess"); //MLHIDE
                var StartCheckboxPerm = EpiInfoMenuManager.GetPermanentVariableValue("StartCheckboxPerm"); //MLHIDE
                var StartCheckbox2Perm = EpiInfoMenuManager.GetPermanentVariableValue("StartCheckbox2Perm"); //MLHIDE
                var SiteCodePerm = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm"); //MLHIDE

                if (StartCheckboxPerm.ToString() != "1")
                {
                    StartCheckbox2Perm = 0;
                }

                StartCheckboxPerm_num = Convert.ToInt32(StartCheckboxPerm);

                if (StartCheckboxPerm.ToString() != "1")
                {
                    StartCheckbox2Perm = "0";
                }

                StartCheckbox2Perm_num = Convert.ToInt32(StartCheckbox2Perm);


                if (StartCheckboxPerm_num == 1 && StartCheckbox2Perm_num == 1)
                {
                    StartBoxFirst.Visibility = Visibility.Hidden;
                    StartTextFirst.Visibility = Visibility.Hidden;
                    StartTextFirst2.Visibility = Visibility.Hidden;
                    StartButtonFirst.Visibility = Visibility.Hidden;

                    ShadeBox.Visibility = Visibility.Hidden;
                    StartBox.Visibility = Visibility.Hidden;
                    StartButton.Visibility = Visibility.Hidden;
                    StartText.Visibility = Visibility.Hidden;
                    StartCheckbox.Visibility = Visibility.Hidden;
                    StartArrow.Visibility = Visibility.Hidden;

                    StartBox2.Visibility = Visibility.Hidden;
                    StartButton2.Visibility = Visibility.Hidden;
                    StartText2.Visibility = Visibility.Hidden;
                    StartCheckbox2.Visibility = Visibility.Hidden;
                    StartArrow2.Visibility = Visibility.Hidden;
                }

                //if (StartCheckboxPerm_num == 1)
                //{
                //    StartBox.Visibility = Visibility.Hidden;
                //    StartButton.Visibility = Visibility.Hidden;
                //    StartText.Visibility = Visibility.Hidden;
                //    StartCheckbox.Visibility = Visibility.Hidden;
                //    StartArrow.Visibility = Visibility.Hidden;
                //}

                //if (StartCheckboxPerm_num == 1)
                //{
                //    StartBox2.Visibility = Visibility.Hidden;
                //    StartButton2.Visibility = Visibility.Hidden;
                //    StartText2.Visibility = Visibility.Hidden;
                //    StartCheckbox2.Visibility = Visibility.Hidden;
                //    StartArrow2.Visibility = Visibility.Hidden;
                //}

                if (LimitedAccess != null)
                {
                    if (LimitedAccess.ToString() == "1")
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

                if (SiteCodePerm != null)
                {
                    if (SiteCodePerm.ToString() == "3")
                    {
                        btnSites.IsEnabled = false;
                        btnLanguage.IsEnabled = false;
                    }
                    else
                    {
                        btnSites.IsEnabled = true;
                        btnLanguage.IsEnabled = true;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

            //popup.PlacementTarget = this;
            //popupC.Placement = PlacementMode.MousePoint;
            //popupC.StaysOpen = false;
            //popupC.IsOpen = true;

            Spanel.Visibility = Visibility.Visible;

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
            Spanel.Visibility = Visibility.Collapsed;

            FrameworkElement fe = sender as FrameworkElement;

            while (fe != null)

            {

                if (fe is ContextMenu)

                {

                    (fe as ContextMenu).IsOpen = false;

                    break;

                }

                fe = fe.Parent as FrameworkElement;

            }

            //popupC.Visibility = Visibility.Collapsed;
            //popupC.IsOpen = false;

            // gfj
            string PermSitePWD1 = Properties.Settings.Default.Setting_PWD;
            EpiInfoMenuManager.SetPermanentVariable("Permanent_SitePWD", PermSitePWD1); //MLHIDE

            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            string appDir = Environment.CurrentDirectory;
            //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml"); //MLHIDE
            string commandText = appDir + "\\Enter.exe"; //MLHIDE

            if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj")) //MLHIDE
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = commandText;
                proc.StartInfo.Arguments = "/project: Projects\\TrackingMaster\\TrackingMasterNew.prj /view:codeFieldSites"; //MLHIDE
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
                proc.WaitForExit();

                Thread.Sleep(2500);

                // GFJ0
                DataHelper.FillResidents_Sub();

                DataHelper.LoadSiteMergeData();

                //FieldSites27RAPasswords();
                //object Setting_PWD2 = EpiInfoMenuManager.GetPermanentVariableValue("Permanent_SitePWD");

                //FieldSites27RAPasswords();

                EpiInfoMenuManager.SetPermanentVariable("Permanent_SitePWD", "XXX"); //MLHIDE

            }
            else
            {
                MessageBox.Show(ml.ml_string(231, "This project was not installed in the correctly. Please ensure the application is installed correctly"));
            }

        }


        private void MenuItemUserSettings_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //FieldSites27RAPasswords();

                object LimitedAccess = EpiInfoMenuManager.GetPermanentVariableValue("PermLimitedAccess"); //MLHIDE

                if (LimitedAccess != null)
                {
                    if (LimitedAccess.ToString() == "1")
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


                object langvar1 = EpiInfoMenuManager.GetPermanentVariableValue("LanguageSettings_Perm"); //MLHIDE
                object permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident"); //MLHIDE
                string Language_Perm_str1 = "Default"; //MLHIDE




                if (langvar1 is null || langvar1.ToString() == "")
                {
                    Language_Perm_str1 = "English"; //MLHIDE
                    //.Text = Properties.Resources.Language0 + langvar1.ToString();
                }
                else
                {
                    Language_Perm_str1 = langvar1.ToString();
                }

                System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
                string appDir = Environment.CurrentDirectory;
                //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml"); //MLHIDE
                string commandText = appDir + "\\Enter.exe"; //MLHIDE

                if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj")) //MLHIDE
                {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = commandText;
                    proc.StartInfo.Arguments = "/project: Projects\\TrackingMaster\\TrackingMasterNew.prj /view:UserSettings"; //MLHIDE
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                    proc.WaitForExit();

                    SetUser();

                    //LoadConfig();
                    //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");  //MLHIDE

                    permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident");  //MLHIDE
                    //ResidentNameTextBox.Text = Properties.Resources.Resident_Name0 + permvar2.ToString();

                    permvar3 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm"); //MLHIDE

                    Properties.Settings.Default.SiteCode = permvar3.ToString();

                    if (permvar3.ToString() == "3")
                    {
                        permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident"); //MLHIDE
                        Menafrinet.View.Properties.Settings.Default.SiteName = permvar2.ToString();
                    }

                    if (permvar3.ToString() == "2")
                    {
                        permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC"); //MLHIDE
                        Menafrinet.View.Properties.Settings.Default.SiteName = permvar2.ToString();
                    }

                    if (permvar3.ToString() == "1")
                    {
                        permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC"); //MLHIDE
                        Menafrinet.View.Properties.Settings.Default.SiteName = permvar2.ToString();
                    }

                    if (permvar3.ToString() != "1" && permvar3.ToString() != "2" && permvar3.ToString() != "3")
                    {
                        Menafrinet.View.Properties.Settings.Default.SiteName = "FETP Office";
                    }


                    //object Setting_PWD2 = EpiInfoMenuManager.GetPermanentVariableValue("Permanent_SitePWD");
                    //Properties.Settings.Default.Setting_PWD = Setting_PWD2.ToString();
                    //EpiInfoMenuManager.SetPermanentVariable("Permanent_SitePWD", "XXX");

                    SetTabs_Sub();

                    Deidentify();

                    MessageBox.Show(ml.ml_string(209, "The app settings have been changed. The user setting changes will take full effect the next time you start the program. Please close the application and restart."), "ReDPeTT Alert: "); //MLHIDE

                }
                else
                {
                    MessageBox.Show(ml.ml_string(231, "This project was not installed in the correctly. Please ensure the application is installed correctly"));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); //MLHIDE
            }
        }

        private void LanguageSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
                //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
                string commandText = appDir + "\\Enter.exe";

                if (System.IO.File.Exists(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"))
                {
                    string PermSitePWD1 = Properties.Settings.Default.Setting_PWD;
                    EpiInfoMenuManager.SetPermanentVariable("Permanent_SitePWD", PermSitePWD1);

                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = commandText;
                    proc.StartInfo.Arguments = "/project: Projects\\TrackingMaster\\TrackingMasterNew.prj /view:LanguageSettings";
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                    proc.WaitForExit();

                    EpiInfoMenuManager.SetPermanentVariable("Permanent_SitePWD", "XXX");

                    //LoadConfig();
                    //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");
                    permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident");
                    //ResidentNameTextBox.Text = Properties.Resources.Resident_Name0 + permvar2.ToString();

                    object permvar3 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm");

                    //  LanguageSettings_Perm
                    object langvar2 = EpiInfoMenuManager.GetPermanentVariableValue("LanguageSettings_Perm"); //MLHIDE

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
                    connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB"; // @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Projects\Menafrinet\Menafrinet_NE.MDB";

                    connect.Open();

                    OleDbCommand command = new OleDbCommand();
                    command.Connection = connect;

                    // Sets the field label
                    //command.CommandText = "UPDATE UserSettings66LanguageTranslation INNER JOIN metaFields ON UserSettings66LanguageTranslation.FieldName = metaFields.Name SET metaFields.PromptText = [UserSettings66LanguageTranslation].[TypeLabel" + Language_Perm_str2 + "];";

                    command.CommandText = "UPDATE LanguageSettings96LanguageTranslation INNER JOIN metaFields ON LanguageSettings96LanguageTranslation.FieldName = metaFields.Name SET metaFields.PromptText = [LanguageSettings96LanguageTranslation].[TypeLabel" + Language_Perm_str2 + "];"; //MLHIDE
                    command.ExecuteNonQuery();

                    //command.CommandText = "UPDATE LanguageSettings96LanguageTranslation INNER JOIN metaFields ON LanguageSettings96LanguageTranslation.FieldName = metaFields.Name SET metaFields.PromptText = IIf([LanguageSettings96LanguageTranslation].[TypeLabel" + Language_Perm_str2 + "]is null, [LanguageSettings96LanguageTranslation].[TypeLabelEnglish], [LanguageSettings96LanguageTranslation].[TypeLabel" + Language_Perm_str2 + "]);";
                    //command.ExecuteNonQuery();

                    // Sets the comment legal drop down lists
                    Project project = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //MLHIDE new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);
                    IDbDriver db = project.CollectedData.GetDatabase();
                    System.Data.DataTable dt = db.GetTableData("LanguageSettings97CommentLegalFields"); //MLHIDE

                    //var codeTableNames = db.GetTableNames().Where(t => t.StartsWith("code"));

                    // permanent variables error !! 5/23/2018
                    //try
                    //{
                    foreach (DataRow row in dt.Rows)
                    {
                        var tablename = row[5].ToString();
                        var codefieldname = row[6].ToString();

                        //command.CommandText = "UPDATE " + tablename + " LEFT JOIN LanguageSettings98CommentLegalDropdownLists ON " + tablename + "." + codefieldname + " = LanguageSettings98CommentLegalDropdownLists.codeTableValue" + Language_Perm_str1 + " SET " + tablename + "." + codefieldname + " = [LanguageSettings98CommentLegalDropdownLists].[codeTableValue" + Language_Perm_str2 + "];";
                        try
                        {
                            command.CommandText = "UPDATE " + tablename + " INNER JOIN LanguageSettings98CommentLegalDropdownLists ON " + tablename + "." + codefieldname + " = LanguageSettings98CommentLegalDropdownLists.codeTableValue" + Language_Perm_str1 + " SET " + tablename + "." + codefieldname + " = [LanguageSettings98CommentLegalDropdownLists].[codeTableValue" + Language_Perm_str2 + "];"; //MLHIDE
                            command.ExecuteNonQuery();
                        }
                        catch
                        {
                            MessageBox.Show("Please ensure that table \"" + tablename + " - " + codefieldname + "\" exist in the source table"); //MLHIDE
                        }

                        try
                        {
                            command.CommandText = "UPDATE " + tablename + " INNER JOIN LanguageSettings98CommentLegalDropdownLists ON " + tablename + "." + codefieldname + " = LanguageSettings98CommentLegalDropdownLists.codeTableValue" + "English" + " SET " + tablename + "." + codefieldname + " = [LanguageSettings98CommentLegalDropdownLists].[codeTableValue" + Language_Perm_str2 + "];"; //MLHIDE
                            command.ExecuteNonQuery();
                        }
                        catch
                        {
                            MessageBox.Show("Please ensure that table \"" + tablename + " - " + codefieldname + "\" exist in the source table"); //MLHIDE
                        }

                    }

                    DataTable dt2 = db.GetTableData("MetaViews"); //MLHIDE
                    DataTable dt3 = db.GetTableData("LanguageSettings99DialogTranslation"); //MLHIDE

                    // Check code dialogs
                    foreach (DataRow row2 in dt2.Rows)
                    {
                        string MyViewID = row2[0].ToString();
                        string MyViewName = row2[1].ToString();
                        string CheckCode = row2[3].ToString();

                        foreach (DataRow dr in dt3.Rows)
                        {

                            //    foreach (DataColumn dc in dt3.Columns)
                            //{
                            string LanguageCurrent = "Dialog" + Language_Perm_str1; //MLHIDE
                            string LanguageSelected = "Dialog" + Language_Perm_str2; //MLHIDE
                            string OldTranslation = dr[LanguageCurrent].ToString();
                            string NewTranslation = dr[LanguageSelected].ToString();

                            if (CheckCode.Contains(OldTranslation))
                            {
                                //MessageBox.Show("Found IT1: " + OldTranslation + " -- " + NewTranslation);

                                // gfj

                                string NewCheckCode = CheckCode.Replace(OldTranslation, NewTranslation);
                                OleDbCommand command2 = new OleDbCommand(@"UPDATE metaviews
                                    SET CheckCode = @NewCheckCode
                                    WHERE ([ViewID] = MyViewID AND [Name] = MyViewName)", connect); //MLHIDE

                                command2.Parameters.AddWithValue("@NewCheckCode", NewCheckCode); //MLHIDE
                                command2.Parameters.AddWithValue("@MyViewID", MyViewID); //MLHIDE
                                command2.Parameters.AddWithValue("@MyViewName", MyViewName); //MLHIDE
                                command2.ExecuteNonQuery();

                            }
                        }

                    }

                    command.CommandText = "UPDATE LanguageSettings100DiseaseTranslation INNER JOIN code_diseaseenglish ON LanguageSettings100DiseaseTranslation.DiseaseEnglish = code_diseaseenglish.DiseaseEnglish SET code_diseaseenglish.Disease = [LanguageSettings100DiseaseTranslation].[Disease" + Language_Perm_str2 + "];"; //MLHIDE
                    command.ExecuteNonQuery();

                    connect.Close();

                    // Configuration

                    string EILangSetting = "English"; //MLHIDE
                    string EIRepresentationOfYes = "Yes"; //MLHIDE
                    string EIRepresentationOfNo = "No"; //MLHIDE
                    string EIRepresentationOfMissing = "Missing"; //MLHIDE

                    switch (langvar2)
                    {
                        case "Ukrainian": //MLHIDE
                            EILangSetting = "uk"; //MLHIDE
                            EIRepresentationOfYes = "Так"; //MLHIDE
                            EIRepresentationOfNo = "ні"; //MLHIDE
                            EIRepresentationOfMissing = "Відсутні"; //MLHIDE
                            MessageBox.Show("Налаштування додатка були змінені. Зміни мови та налаштувань будуть повністю набрані після наступного запуску програми. Закрийте програму та перезапустіть. ", " Сповіщення про відстеження проекту: "); //MLHIDE
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
                            EILangSetting = "en-US"; //MLHIDE
                            EIRepresentationOfYes = "Yes"; //MLHIDE
                            EIRepresentationOfNo = "No"; //MLHIDE
                            EIRepresentationOfMissing = "Missing"; //MLHIDE
                            MessageBox.Show("The app settings have been changed. The language and settings changes will take full effect the next time you start the program. Please close the application and restart.", "ReDPeTT Alert: ");
                            break;
                    }



                    XmlTextReader settings_xml = new XmlTextReader("Configuration\\EpiInfo.Config.xml"); //MLHIDE
                    XmlDocument settings_doc = new XmlDocument();
                    settings_doc.Load(settings_xml);
                    settings_xml.Close();
                    XmlNode criterionNode;
                    XmlElement root = settings_doc.DocumentElement;

                    criterionNode = root.SelectSingleNode("Settings/Language"); //MLHIDE
                    //criterionNode.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.InnerXml = "<Language>" + "en-US" + "</Language>";
                    criterionNode.InnerXml = EILangSetting;
                    criterionNode = root.SelectSingleNode("Settings/RepresentationOfYes"); //MLHIDE
                    criterionNode.InnerXml = EIRepresentationOfYes;
                    criterionNode = root.SelectSingleNode("Settings/RepresentationOfNo"); //MLHIDE
                    criterionNode.InnerXml = EIRepresentationOfNo;
                    criterionNode = root.SelectSingleNode("Settings/RepresentationOfMissing"); //MLHIDE
                    criterionNode.InnerXml = EIRepresentationOfMissing;
                    settings_doc.Save("Configuration\\EpiInfo.Config.xml"); //MLHIDE
                }
                else
                {
                    MessageBox.Show("This project was not installed in the correctly. Please ensure the application is installed correctly");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void SetTabs_Sub()
        {
            object LimitedAccess = EpiInfoMenuManager.GetPermanentVariableValue("PermLimitedAccess"); //MLHIDE

            if (LimitedAccess.ToString() == "1")
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

        private void Deidentify()
        {
            try
            {
                //Project project = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //new Project(Environment.CurrentDirectory + PROJECT + PROJECTPATH);
                //IDbDriver db = project.CollectedData.GetDatabase();
                //System.Data.DataTable dt = db.GetTableData("LanguageSettings97CommentLegalFields");
                //string appDir = Environment.CurrentDirectory;

                Project project = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj"); //MLHIDE
                IDbDriver dbDeID = project.CollectedData.GetDatabase();

                DataTable dtDeIDProj = dbDeID.GetTableData("UserSettings111ProjectFieldstoRemove"); //MLHIDE
                DataTable dtDeIDRes = dbDeID.GetTableData("UserSettings111ResidentFieldstoRemove"); //MLHIDE
                DataTable dtDeIDComm = dbDeID.GetTableData("UserSettings111CommunicationFieldstoRemove"); //MLHIDE
                DataTable dtDeIDFront = dbDeID.GetTableData("UserSettings111FrontlineFieldstoRemove"); //MLHIDE
                DataTable dtReqFields = dbDeID.GetTableData("UserSettings111RequireFieldstoRemove"); //MLHIDE
                DataTable dtDeIDCon = dbDeID.GetTableData("UserSettings111ConsultantFieldstoRemove"); //MLHIDE


                XmlDocument docProject = new XmlDocument();
                docProject.Load(appDir + "\\Projects\\TrackingMaster\\ImportExport\\ProjectsDeIdentified.pks7"); //MLHIDE

                XmlDocument docTrainee = new XmlDocument();
                docTrainee.Load(appDir + "\\Projects\\TrackingMaster\\ImportExport\\TraineeInfoDeIdentified.pks7"); //MLHIDE

                XmlDocument docFL = new XmlDocument();                       //FrontlineDeIdentified
                docFL.Load(appDir + "\\Projects\\TrackingMaster\\ImportExport\\FrontlineDeIdentified.pks7"); //MLHIDE

                XmlDocument docCON = new XmlDocument();
                docCON.Load(appDir + "\\Projects\\TrackingMaster\\ImportExport\\ConsultantDeidentified.pks7"); //MLHIDE

                //string quotes = "\"";
                int cnt = 0;

                string textProject = File.ReadAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\ProjectsDeIdentified.pks7"); //MLHIDE
                string textTrainee = File.ReadAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\TraineeInfoDeIdentified.pks7"); //MLHIDE
                string textFL = File.ReadAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\FrontlineDeIdentified.pks7"); //MLHIDE

                List<string>[] listDeIDPRJ = new List<String>[2];
                listDeIDPRJ[0] = new List<string>();
                listDeIDPRJ[1] = new List<string>();

                List<string>[] listDeIDTR = new List<String>[2];
                listDeIDTR[0] = new List<string>();
                listDeIDTR[1] = new List<string>();

                List<string>[] listDeIDComm = new List<String>[2];
                listDeIDComm[0] = new List<string>();
                listDeIDComm[1] = new List<string>();

                List<string>[] listDeIDFL = new List<String>[2];
                listDeIDFL[0] = new List<string>();
                listDeIDFL[1] = new List<string>();

                List<string>[] listDeIDCON = new List<String>[2];
                listDeIDCON[0] = new List<string>();
                listDeIDCON[1] = new List<string>();

                List<string>[] listDeIDReq = new List<String>[2];
                listDeIDReq[0] = new List<string>();
                listDeIDReq[1] = new List<string>();

                var ListfieldsToNullPRJ = new List<string>();
                var ListfieldsToNullTR = new List<string>();
                var ListfieldsToNullFL = new List<string>();
                var ListfieldsToNullCON = new List<string>();
                var ListfieldsToRequiredProj = new List<string>();
                var ListfieldsToRequiredTrInfo = new List<string>();
                var ListfieldsToRequiredFL = new List<string>();
                var ListfieldsToRequiredCon = new List<string>();
                var ListfieldsToRequiredComm = new List<string>();

                cnt = 0;

                //foreach (string s in differenceQueryADD)

                foreach (DataRow DRow in dtDeIDProj.Rows)
                {
                    //listDeIDPRJ[0].Add("Projects");
                    listDeIDPRJ[1].Add(dtDeIDProj.Rows[cnt]["FieldName"].ToString()); //MLHIDE
                    //listDeIDPRJ[0].Add(dtDeIDProj.Rows[cnt]["ProjectFieldRequired"].ToString());

                    cnt++;
                }

                cnt = 0;

                foreach (DataRow DRow in dtDeIDComm.Rows)
                {
                    //listDeIDPRJ[0].Add("Requiredwrittenmaterials");
                    listDeIDComm[1].Add(dtDeIDComm.Rows[cnt]["FieldName"].ToString()); //MLHIDE
                    listDeIDComm[0].Add(dtDeIDComm.Rows[cnt]["CommunicationFieldRequired"].ToString()); //MLHIDE

                    cnt++;
                }

                cnt = 0;

                foreach (DataRow DRow in dtDeIDRes.Rows)
                {
                    //string testvarx = dtDeIDRes.Rows[cnt]["FieldName"].ToString();
                    listDeIDTR[1].Add(dtDeIDRes.Rows[cnt]["FieldName"].ToString()); //MLHIDE
                    listDeIDTR[0].Add(dtDeIDRes.Rows[cnt]["ResidentFieldRequired"].ToString()); //MLHIDE

                    cnt++;
                }

                cnt = 0;

                foreach (DataRow DRow in dtDeIDFront.Rows)
                {
                    listDeIDFL[1].Add(dtDeIDFront.Rows[cnt]["FieldName"].ToString()); //MLHIDE
                    listDeIDFL[0].Add(dtDeIDFront.Rows[cnt]["FrontlineFieldRequired"].ToString()); //MLHIDE

                    cnt++;
                }

                cnt = 0;

                foreach (DataRow DRow in dtDeIDCon.Rows)
                {
                    var chkvar = dtDeIDCon.Rows[cnt]["FieldName"].ToString(); //MLHIDE
                    listDeIDCON[1].Add(dtDeIDCon.Rows[cnt]["FieldName"].ToString()); //MLHIDE
                    //listDeIDFL[0].Add(dtDeIDCon.Rows[cnt]["ConsultantFieldRequired"].ToString());

                    cnt++;
                }

                cnt = 0;

                foreach (DataRow DRow in dtReqFields.Rows)
                {
                    var chkvar = dtReqFields.Rows[cnt]["RequireFieldsName"].ToString(); //MLHIDE
                    var chkvar2 = dtReqFields.Rows[cnt]["TableName"].ToString(); //MLHIDE
                    listDeIDReq[0].Add(dtReqFields.Rows[cnt]["RequireFieldsName"].ToString() + ";" + dtReqFields.Rows[cnt]["TableName"].ToString()); //MLHIDE

                    if (chkvar2 == "Projects" || chkvar2 == "Trainees") //MLHIDE
                    {
                        ListfieldsToRequiredProj.Add(dtReqFields.Rows[cnt]["RequireFieldsName"].ToString()); //MLHIDE
                    }
                    if (chkvar2 == "TraineeInformation") //MLHIDE
                    {
                        ListfieldsToRequiredTrInfo.Add(dtReqFields.Rows[cnt]["RequireFieldsName"].ToString()); //MLHIDE
                    }
                    if (chkvar2 == "ConsultantInfo") //MLHIDE
                    {
                        ListfieldsToRequiredCon.Add(dtReqFields.Rows[cnt]["RequireFieldsName"].ToString()); //MLHIDE
                    }
                    if (chkvar2 == "Frontline") //MLHIDE
                    {
                        ListfieldsToRequiredFL.Add(dtReqFields.Rows[cnt]["RequireFieldsName"].ToString()); //MLHIDE
                    }
                    if (chkvar2 == "Requiredwrittenmaterials") //MLHIDE
                    {
                        ListfieldsToRequiredComm.Add(dtReqFields.Rows[cnt]["RequireFieldsName"].ToString()); //MLHIDE
                    }

                    cnt++;
                }

                foreach (XmlNode xnode in docProject.SelectSingleNode("ProjectPackagingScript/fieldsToNull")) //MLHIDE
                {
                    ListfieldsToNullPRJ.Add(xnode.InnerText);
                }
                foreach (XmlNode xnode in docTrainee.SelectSingleNode("ProjectPackagingScript/fieldsToNull")) //MLHIDE
                {
                    ListfieldsToNullTR.Add(xnode.InnerText);
                }
                foreach (XmlNode xnode in docProject.SelectSingleNode("ProjectPackagingScript/fieldsToNull")) //MLHIDE
                {
                    ListfieldsToNullFL.Add(xnode.InnerText);
                }
                foreach (XmlNode xnode in docCON.SelectSingleNode("ProjectPackagingScript/fieldsToNull")) //MLHIDE
                {
                    ListfieldsToNullCON.Add(xnode.InnerText);
                }

                IEnumerable<string> differenceQueryPRJ1 = listDeIDPRJ[1].Except(ListfieldsToNullPRJ);
                IEnumerable<string> differenceQueryPRJ2 = ListfieldsToRequiredProj.Except(ListfieldsToNullPRJ);
                IEnumerable<string> differenceQueryPRJ = differenceQueryPRJ1.Union(differenceQueryPRJ2);

                IEnumerable<string> differenceQueryCOM1 = listDeIDComm[1].Except(ListfieldsToNullPRJ);
                IEnumerable<string> differenceQueryCOM2 = ListfieldsToRequiredComm.Except(ListfieldsToNullPRJ);
                IEnumerable<string> differenceQueryCOM = differenceQueryCOM1.Union(differenceQueryCOM2);

                IEnumerable<string> differenceQueryTR1 = listDeIDTR[1].Except(ListfieldsToNullTR);
                IEnumerable<string> differenceQueryTR2 = ListfieldsToRequiredTrInfo.Except(ListfieldsToNullTR);
                IEnumerable<string> differenceQueryTR = differenceQueryTR1.Union(differenceQueryTR2);



                IEnumerable<string> differenceQueryFL1 = listDeIDFL[1].Except(ListfieldsToNullFL);
                IEnumerable<string> differenceQueryFL2 = ListfieldsToRequiredFL.Except(ListfieldsToNullFL);
                IEnumerable<string> differenceQueryFL = differenceQueryTR1.Union(differenceQueryTR2);

                IEnumerable<string> differenceQueryCON1 = listDeIDCON[1].Except(ListfieldsToNullCON);
                IEnumerable<string> differenceQueryCON2 = ListfieldsToRequiredCon.Except(ListfieldsToNullCON);
                IEnumerable<string> differenceQueryCON = differenceQueryCON1.Union(differenceQueryCON2);

                IEnumerable<string> differenceQueryRemovePRJ = ListfieldsToNullPRJ.Except(listDeIDPRJ[1]);
                IEnumerable<string> differenceQueryRemoveTR = ListfieldsToNullTR.Except(listDeIDTR[1]);
                IEnumerable<string> differenceQueryRemoveFL = ListfieldsToNullFL.Except(listDeIDFL[1]);
                IEnumerable<string> differenceQueryRemoveCON = ListfieldsToNullCON.Except(listDeIDCON[1]);


                string TableToRemove;
                string FieldToRemove;

                foreach (string Field in differenceQueryPRJ)
                {
                    FieldToRemove = Field.ToString();
                    TableToRemove = "Projects"; //MLHIDE

                    if (FieldToRemove == "ResidentName2") //MLHIDE
                    {
                        TableToRemove = "Trainees"; //MLHIDE
                    }

                    var fieldToNull = docProject.SelectSingleNode("ProjectPackagingScript/fieldsToNull"); //MLHIDE
                    XmlNode ele = docProject.CreateElement("fieldToNull"); //MLHIDE
                    string test = "formName=\"" + TableToRemove + "\">" + FieldToRemove; //MLHIDE
                    ele.InnerText = test;
                    fieldToNull.AppendChild(ele);
                }

                foreach (string Field in differenceQueryCOM)
                {
                    FieldToRemove = Field.ToString();
                    TableToRemove = "ReqiredWrittenMaterials"; //MLHIDE

                    //var value1 = Array.Find(array1,  element => element.StartsWith("car", StringComparison.Ordinal));
                    //var foundField = listDeIDPRJ.First(element => element.Contains(FieldToRemove));
                    //bool foundField = listDeIDPRJ[1].Contains(FieldToRemove);
                    //if (foundField == true)
                    //{
                    //    MessageBox.Show(FieldToRemove);
                    //    TableToRemove = "Projects";
                    //}
                    //{
                    //    if (value == FieldToRemove)
                    //    {
                    //        MessageBox.Show(value);
                    //        TableToRemove = "Projects";
                    //        break;
                    //    }
                    //}

                    var fieldToNull = docProject.SelectSingleNode("ProjectPackagingScript/fieldsToNull"); //MLHIDE
                    XmlNode ele = docProject.CreateElement("fieldToNull"); //MLHIDE
                    string test = "formName=\"" + TableToRemove + "\">" + FieldToRemove; //MLHIDE
                    ele.InnerText = test;
                    fieldToNull.AppendChild(ele);
                }


                foreach (string Field in differenceQueryTR)
                {
                    FieldToRemove = Field.ToString();
                    TableToRemove = "TraineeInformation"; //MLHIDE
                    var fieldToNull = docTrainee.SelectSingleNode("ProjectPackagingScript/fieldsToNull"); //MLHIDE
                    XmlNode ele = docTrainee.CreateElement("fieldToNull"); //MLHIDE
                    string test = "formName=\"" + TableToRemove + "\">" + FieldToRemove; //MLHIDE
                    ele.InnerText = test;
                    fieldToNull.AppendChild(ele);
                }


                foreach (string Field in differenceQueryFL)
                {
                    FieldToRemove = Field.ToString();
                    TableToRemove = "Frontline"; //MLHIDE
                    var fieldToNull = docFL.SelectSingleNode("ProjectPackagingScript/fieldsToNull"); //MLHIDE
                    XmlNode ele = docFL.CreateElement("fieldToNull"); //MLHIDE
                    string test = "formName=\"" + TableToRemove + "\">" + FieldToRemove; //MLHIDE
                    ele.InnerText = test;
                    fieldToNull.AppendChild(ele);
                }

                foreach (string Field in differenceQueryCON)
                {
                    FieldToRemove = Field.ToString();
                    TableToRemove = "ConsultantInfo"; //MLHIDE
                    var fieldToNull = docCON.SelectSingleNode("ProjectPackagingScript/fieldsToNull"); //MLHIDE
                    XmlNode ele = docCON.CreateElement("fieldToNull"); //MLHIDE
                    string test = "formName=\"" + TableToRemove + "\">" + FieldToRemove; //MLHIDE
                    ele.InnerText = test;
                    fieldToNull.AppendChild(ele);
                }


                foreach (string t in differenceQueryRemovePRJ)
                {
                    foreach (XmlNode xnode in docProject.SelectSingleNode("ProjectPackagingScript/fieldsToNull")) //MLHIDE
                    {
                        if (!listDeIDReq[0].Contains(t.ToString() + ";Projects") && !listDeIDReq[0].Contains(t.ToString() + ";Trainees")) //MLHIDE
                        {
                            if (xnode.InnerText == t.ToString())
                            {
                                xnode.ParentNode.RemoveChild(xnode);
                            }
                        }
                    }
                }

                foreach (string t in differenceQueryRemoveTR)
                {
                    foreach (XmlNode xnode in docTrainee.SelectSingleNode("ProjectPackagingScript/fieldsToNull")) //MLHIDE
                    {

                        if (!listDeIDReq[0].Contains(t.ToString() + ";TraineeInformation")) //MLHIDE
                        {
                            var tt = t.ToString() + ";TraineeInformation"; //MLHIDE
                            if (xnode.InnerText == t.ToString())
                            {
                                xnode.ParentNode.RemoveChild(xnode);
                            }
                        }
                    }
                }

                foreach (string t in differenceQueryRemoveFL)
                {
                    foreach (XmlNode xnode in docFL.SelectSingleNode("ProjectPackagingScript/fieldsToNull")) //MLHIDE
                    {
                        if (!listDeIDReq[0].Contains(t.ToString() + ";Frontline")) //MLHIDE
                        {
                            if (xnode.InnerText == t.ToString())
                            {
                                xnode.ParentNode.RemoveChild(xnode);
                            }
                        }
                    }
                }

                foreach (string t in differenceQueryRemoveCON)
                {
                    foreach (XmlNode xnode in docCON.SelectSingleNode("ProjectPackagingScript/fieldsToNull")) //MLHIDE
                    {
                        var x1 = t.ToString() + ";ConsultantInfo"; //MLHIDE
                        if (!listDeIDReq[0].Contains(t.ToString() + ";ConsultantInfo")) //MLHIDE
                        {
                            if (xnode.InnerText == t.ToString())
                            {
                                xnode.ParentNode.RemoveChild(xnode);
                            }
                        }
                    }
                }

                docProject.Save(appDir + "\\Projects\\TrackingMaster\\ImportExport\\ProjectsDeIdentified.pks7"); //MLHIDE
                string text1 = File.ReadAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\ProjectsDeIdentified.pks7"); //MLHIDE
                text1 = text1.Replace("&gt;", ">"); //MLHIDE
                text1 = text1.Replace("<fieldToNull>", "<fieldToNull "); //MLHIDE
                File.WriteAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\ProjectsDeIdentified.pks7", text1); //MLHIDE

                docTrainee.Save(appDir + "\\Projects\\TrackingMaster\\ImportExport\\TraineeInfoDeIdentified.pks7"); //MLHIDE
                string text2 = File.ReadAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\TraineeInfoDeIdentified.pks7"); //MLHIDE
                text2 = text2.Replace("&gt;", ">"); //MLHIDE
                text2 = text2.Replace("<fieldToNull>", "<fieldToNull "); //MLHIDE
                File.WriteAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\TraineeInfoDeIdentified.pks7", text2); //MLHIDE

                docFL.Save(appDir + "\\Projects\\TrackingMaster\\ImportExport\\FrontlineDeIdentified.pks7"); //MLHIDE
                string text3 = File.ReadAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\FrontlineDeIdentified.pks7"); //MLHIDE
                text3 = text3.Replace("&gt;", ">"); //MLHIDE
                text3 = text3.Replace("<fieldToNull>", "<fieldToNull "); //MLHIDE
                File.WriteAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\FrontlineDeIdentified.pks7", text3); //MLHIDE

                docCON.Save(appDir + "\\Projects\\TrackingMaster\\ImportExport\\ConsultantDeidentified.pks7"); //MLHIDE
                string text4 = File.ReadAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\ConsultantDeidentified.pks7"); //MLHIDE
                text4 = text4.Replace("&gt;", ">"); //MLHIDE
                text4 = text4.Replace("<fieldToNull>", "<fieldToNull "); //MLHIDE
                File.WriteAllText(appDir + "\\Projects\\TrackingMaster\\ImportExport\\ConsultantDeidentified.pks7", text4); //MLHIDE

            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem. Error: " + ex.Message, "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void testmouse(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //popupC.IsOpen = false;
        }

        private void SpanelLostFocus(object sender, RoutedEventArgs e)
        {
            Spanel.Visibility = Visibility.Collapsed;
        }

        private void Spanel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Spanel.Visibility = Visibility.Collapsed;
        }

        public void btnVideos_Click(object sender, RoutedEventArgs e)
        {

            //MessageBox.Show("TEST OPEN");
            //CanvasSelection.Margin = new Thickness(0, 32, 0, 0);

            //if (CanvasSelection.GotFocus true)
            //CanvasSelection.LostFocus += new EventHandler(CanvasSelection_LostFocus);
            //if (CanvasSelection.Margin.Left == 0)
            //{
            //}
            //ChangeCriteria_flatColorButton.Content = "Modifier Les Critères";
            //CanvasSelection.Margin = new Thickness(-190, 32, 0, 0);
            //    //ChangeCriteria_flatColorButton.ToolTip = "Cliquez ici pour mettre à jour ou de modifier les rapports.";
            //    //documentViewer1.Visibility = Visibility.Hidden;
            //    //browser2.Visibility = Visibility.Hidden;
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
            //    //ChangeCriteria_flatColorButton.Content = "Sous-Menu Collapse";
            //    CanvasSelection.Margin = new Thickness(0, 32, 0, 0);
            //    //    //ChangeCriteria_flatColorButton.ToolTip = "Cliquez ici pour fermer ce menu";
            //    //    //documentViewer1.Visibility = Visibility.Hidden;
            //    //    //browser2.Visibility = Visibility.Hidden;
            //    //    //ArrowIN.Visibility = System.Windows.Visibility.Visible;
            //    //    //ArrowOUT.Visibility = System.Windows.Visibility.Hidden;
            //    //    //DropShadowEffect Drop = new DropShadowEffect();
            //    //    //CanvasSelection.Effect = new DropShadowEffect();
            //    //    //Drop.Direction = 305;
            //    //    //Drop.BlurRadius = 15;
            //    //    //Drop.ShadowDepth = 6;
            //    //    //Drop.Color = Colors.Black;
            //    //    //documentViewer1.Margin = new Thickness(396, 2, 7, 73);
            //    //    //documentViewer1.Visibility = System.Windows.Visibility.Collapsed;
            //}

        }

        //private void CanvasSelection_LostFocus(object sender, System.Windows.Input.MouseEventArgs e)
        //{

        //    //int closeNum = -20;

        //    //for (int i = 1; i <= 21; i++)
        //    //{ // print numbers from 1 to 5
        //    //    if (closeNum > -190)
        //    //    {
        //            //closeNum = closeNum - 10;
        //            //CanvasSelection.Margin = new Thickness(closeNum, 32, 0, 0);
        //            CanvasSelection.Margin = new Thickness(-190, 32, 0, 0);
        //    //Thread.Sleep(400);
        //    //    }
        //    //}
        //}

        //private void FieldSites27RAPasswords(object sender, System.Windows.Input.MouseEventArgs e)

        private void FieldSites27RAPasswords()
        {
            permvar3 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm");
            permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC");


            object Permanent_FilterPWD = EpiInfoMenuManager.GetPermanentVariableValue("Permanent_FilterPWD");
            object UserSettingResident = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC");

            Project project = new Project(appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.prj");

            IDbDriver dbDistricts = project.CollectedData.GetDatabase();
            DataTable dtDistricts = dbDistricts.GetTableData("codeFieldSites120RAStatus");

            int cnt = 0;

            // 

            foreach (DataRow DRow in dtDistricts.Rows)
            {

                if (cnt < dtDistricts.Rows.Count)
                {
                    //dtDistricts.Rows[cnt]["FilteSiteSettingrPWD"].ToString() != null

                    string FilterPWDString = dtDistricts.Rows[cnt]["SitePassword"].ToString();
                    string RAStatusString = dtDistricts.Rows[cnt]["RAStatus"].ToString();
                    string FilterSiteSetting = dtDistricts.Rows[cnt]["SiteSetting"].ToString();

                    if (RAStatusString == permvar2.ToString() && FilterSiteSetting != null && FilterSiteSetting.Contains("National Resident Advisor") && permvar3.ToString() == "1")
                    {

                        Properties.Settings.Default.Setting_PWD = FilterPWDString;

                        //String District2 = (dtDistricts.Rows[cnt]["RAStatus"].ToString());
                        //EpiInfoMenuManager.CreatePermanentVariable("Permanent_FilterPWD", 2);
                        //EpiInfoMenuManager.SetPermanentVariable("Permanent_FilterPWD", FilterPWDString);

                        //Object Setting_FilterPWD_str = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC");

                        //Menafrinet.View.Properties.Settings.Default.Setting_PWD = FilterPWDString;

                        // ItemArray = {object[8]}

                    }

                    cnt++;

                }
            }


        }

        int PWTotal;
        bool donevar;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        private void StartBox_First(object sender, RoutedEventArgs e)
        {

            //dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            double chk2 = (ShadeBox.ActualWidth - StartBox.ActualWidth) / 2;
            if (chk2 < 370) { chk2 = chk2 + 50; }
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            dispatcherTimer.Start();
            PW = 167;
            PWTotal = 360 - Convert.ToInt32(chk2);
            //MessageBox.Show("Start: " + PW + " PWTotal: " + PWTotal + " chk2: " + chk2);
            PWcnt = PWcnt + 1;

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (PW > PWTotal)
            {
                PW = PW - 20;                // 30,0,0,171
                StartBox.Margin = new Thickness(30,0,0,PW);
                StartBox.Margin = new Thickness(30, 0, 0, PW);
                StartText.Margin = new Thickness(69, 0, 0, PW + 61);
                StartButton.Margin = new Thickness(89, 0, 0, PW - 2);
                StartCheckbox.Margin = new Thickness(57, 0, 0, PW - 85);
                StartArrow.Margin = new Thickness(110, 0, 0, PW - 102);

                //StartText.Margin = new Thickness(69, 0, 0, PW+61);
                //StartButton.Margin = new Thickness(89, 0, 0, PW-2);
                //StartCheckbox.Margin = new Thickness(57, 0, 0, PW-90);
                //StartArrow.Margin = new Thickness(110, 0, 0, PW-102);
                // 105,0,0,74
                StartText.Text = "User manual is here.";

            }
            else
            {

                //MessageBox.Show("Finished!: " + PW);

                dispatcherTimer.Stop(); // stop timer
                dispatcherTimer.Tick -= dispatcherTimer_Tick; // unsubscribe from timer's ticks

                //return;
                //dispatcherTimer.Stop();
                //dispatcherTimer = null;
            }

            if (PWcnt == 2)
            {
                StartBox.Visibility = Visibility.Hidden;
                StartText.Visibility = Visibility.Hidden;
                StartButton.Visibility = Visibility.Hidden;
                StartCheckbox.Visibility = Visibility.Hidden;
                StartArrow.Visibility = Visibility.Hidden;

                if (StartCheckboxPerm_num != 1)
                {
                    StartBox2.Visibility = Visibility.Visible;
                    StartText2.Visibility = Visibility.Visible;
                    StartButton2.Visibility = Visibility.Visible;
                    StartCheckbox2.Visibility = Visibility.Visible;
                    StartArrow2.Visibility = Visibility.Visible;
                }
            }
        }

        private void StartButton2_Click(object sender, RoutedEventArgs e)
        {
            //dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick2);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            dispatcherTimer.Start();
            PW2 = 10;
            PW3 = -50;
            PWcnt = PWcnt + 1;
        }

        private void dispatcherTimer_Tick2(object sender, EventArgs e)
        {
            if (PW2 < 220)
            {
                PW2 = PW2 + 29;
                PW3 = PW3 - 3.7;
                StartBox2.Margin = new Thickness(0, PW3, PW2, 0);
                StartText2.Margin = new Thickness(0, PW3 + 70, PW2 + 19, 0);
                StartButton2.Margin = new Thickness(0, PW3 + 50, PW2 + 64, 0);
                StartCheckbox2.Margin = new Thickness(0, PW3 + 86, PW2 + 36, 0);
                StartArrow2.Margin = new Thickness(0, PW3 + 90, PW2 + 60, 0);
                
                StartText2.Text = "Your name will appear here.";
            }
            else
            {
                dispatcherTimer.Stop(); // stop timer
                dispatcherTimer.Tick -= dispatcherTimer_Tick; // unsubscribe from timer's ticks
            }

            if (PWcnt == 4)
            {
                StartBox.Visibility = Visibility.Hidden;
                StartText.Visibility = Visibility.Hidden;
                StartButton.Visibility = Visibility.Hidden;
                StartCheckbox.Visibility = Visibility.Hidden;
                StartArrow.Visibility = Visibility.Hidden;

                StartBox2.Visibility = Visibility.Hidden;
                StartText2.Visibility = Visibility.Hidden;
                StartButton2.Visibility = Visibility.Hidden;
                StartCheckbox2.Visibility = Visibility.Hidden;
                StartArrow2.Visibility = Visibility.Hidden;
                ShadeBox.Visibility = Visibility.Hidden;


            }
        }

        private void StartButtonFirstClick(object sender, RoutedEventArgs e)
        {
            //chkbot = StartBox.Margin.Bottom;
            //double chk2 = (ShadeBox.ActualWidth - StartBox.ActualWidth) / 2 - 312;
            //PW = Convert.ToInt32(chk2);
            //PW = Convert.ToInt32(chkbot);
            //PW = 298;
            //PWTotal = Convert.ToInt32(chk2) + 200;
            //StartBox.Margin = new Thickness(30, 0, 0, PW);


            if (StartCheckboxPerm_num != 1)
            {
                StartBox.Visibility = Visibility.Visible;
                StartText.Visibility = Visibility.Visible;
                StartButton.Visibility = Visibility.Visible;
                StartCheckbox.Visibility = Visibility.Visible;
                StartArrow.Visibility = Visibility.Visible;
            }

            StartBoxFirst.Visibility = Visibility.Hidden;
            StartTextFirst.Visibility = Visibility.Hidden;
            StartTextFirst2.Visibility = Visibility.Hidden;
            StartButtonFirst.Visibility = Visibility.Hidden;

            if (StartCheckboxPerm_num == 1)
            {
                StartBox2.Visibility = Visibility.Visible;
                StartText2.Visibility = Visibility.Visible;
                StartButton2.Visibility = Visibility.Visible;
                StartCheckbox2.Visibility = Visibility.Visible;
                StartArrow2.Visibility = Visibility.Visible;
                PWcnt = 2;

            }
        }

        private void StartCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable("StartCheckboxPerm", 1);
            Epi.Menu.EpiInfoMenuManager.SetPermanentVariable("StartCheckboxPerm", 1);
            //if (StartCheckbox.IsChecked == true)
            //{
            //    StartCheckboxPerm_num = 1;
            //}
            //else
            //{
            //    StartCheckboxPerm_num = 0;

            //}
        }

        private void StartCheckbox2_Checked(object sender, RoutedEventArgs e)
        {
            Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable("StartCheckbox2Perm", 1);
            Epi.Menu.EpiInfoMenuManager.SetPermanentVariable("StartCheckbox2Perm", 1);

            //if (StartCheckbox2.IsChecked == true)
            //{
            //    StartCheckbox2Perm_num = 1;
            //}
            //else
            //{
            //    StartCheckbox2Perm_num = 0;

            //}
        }

        private void SetUser()
        {

            OleDbConnection connect = new OleDbConnection();

            connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB";   //MLHIDE

            connect.Open();

            OleDbCommand command = new OleDbCommand();

            command.Connection = connect;

            command.CommandText = "SELECT UserSettings58.SiteCode1 FROM UserSettings58;";   //MLHIDE

            var SiteName = command.ExecuteScalar();

            if (SiteName != null)
            {
                EpiInfoMenuManager.SetPermanentVariable("SiteCodePerm", SiteName.ToString());  //MLHIDE
            }
            //else
            //{
            //    MessageBox.Show("it's still null");
            //}    

            command.CommandText = "SELECT UserSettings58.ResidentName2 FROM UserSettings58;";   //MLHIDE

            var ResName = command.ExecuteScalar();

            if (ResName != null)
            {
                EpiInfoMenuManager.SetPermanentVariable("UserSettingResident", ResName.ToString());  //MLHIDE
            }

            command.CommandText = "SELECT UserSettings63.limitedaccess FROM UserSettings63;";   //MLHIDE

            var LimitedViewName = command.ExecuteScalar();

            if (LimitedViewName != null)
            {
                EpiInfoMenuManager.SetPermanentVariable("PermLimitedAccess", LimitedViewName.ToString());  //MLHIDE
            }

            command.CommandText = "SELECT UserSettings63.ExportOpt FROM UserSettings63;";   //MLHIDE

            var ExportOptName = command.ExecuteScalar();

            if (ExportOptName != null)
            {
                EpiInfoMenuManager.SetPermanentVariable("PermExportOpt", ExportOptName.ToString());  //MLHIDE
            }

            command.CommandText = "SELECT UserSettings61.RAPC FROM UserSettings61;";   //MLHIDE

            var RAPCName = command.ExecuteScalar();

            if (RAPCName != null)
            {
                EpiInfoMenuManager.SetPermanentVariable("UserSettingRAPC", RAPCName.ToString());  //MLHIDE
            }

            command.CommandText = "SELECT UserSettings63.InstOnly FROM UserSettings63;";   //MLHIDE

            var InstOnlyName = command.ExecuteScalar();

            if (InstOnlyName != null)
            {
                EpiInfoMenuManager.SetPermanentVariable("PermInstOnly", InstOnlyName.ToString());  //MLHIDE
            }

            command.CommandText = "SELECT UserSettings63.WhichInst FROM UserSettings63;";   //MLHIDE

            var WhichInstName = command.ExecuteScalar();

            if (WhichInstName != null)
            {
                EpiInfoMenuManager.SetPermanentVariable("PermWhichInst", WhichInstName.ToString());  //MLHIDE
            }

            command.CommandText = "SELECT codefieldsites120rastatus.SitePassword FROM codefieldsites120rastatus INNER JOIN (UserSettings58 INNER JOIN UserSettings61 ON UserSettings58.GlobalRecordId = UserSettings61.GlobalRecordId) ON codefieldsites120rastatus.RAStatus = UserSettings61.RAPC;";   //MLHIDE

            var StoreSitePWD = command.ExecuteScalar();

            if (StoreSitePWD != null)
            {

                Properties.Settings.Default.Setting_PWD = StoreSitePWD.ToString(); // MLHIDE

            }

            //command.CommandText = "SELECT UserSettings63.CurrentCohort FROM UserSettings63;";   //MLHIDE

            //var CurrentCohortName = command.ExecuteScalar();

            //if (CurrentCohortName != null)
            //{
            //    EpiInfoMenuManager.SetPermanentVariable("CurrentCohortPerm", CurrentCohortName.ToString());  //MLHIDE
            //}

            connect.Close();

        }

    }
}
