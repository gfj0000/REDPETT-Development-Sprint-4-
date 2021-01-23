using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using Epi;
using Epi.Data;
using Epi.Fields;
using Epi.ImportExport;
using Epi.ImportExport.Filters;
using Epi.ImportExport.ProjectPackagers;
using Menafrinet.Core;
using Menafrinet.ViewModel;
using Menafrinet.ViewModel.Events;
using Menafrinet.ViewModel.Locations;
using Epi.Menu;
using System.Windows;

namespace Menafrinet.ViewModel
{
    /// <summary>
    /// Data helper class for the Menafrinet application
    /// </summary>
    public class DataHelper : DataHelperBase
    {
        #region Members
        private ObservableCollection<CaseViewModel> _caseCollection = new ObservableCollection<CaseViewModel>();
        private ObservableCollection<CaseViewModel> _filteredCaseCollection = new ObservableCollection<CaseViewModel>();
        private ObservableCollection<SiteMergeStatusViewModel> _siteCollection = new ObservableCollection<SiteMergeStatusViewModel>();
        private ObservableCollection<ISiteType> _siteTypes = new ObservableCollection<ISiteType>();
        private string _searchString = String.Empty;
        private System.Windows.Shell.TaskbarItemProgressState _taskbarProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
        private double _taskbarProgressValue = 0.0;
        private bool _isShowingRegionDistrictEditor = false;
        // added for the settings page for the region district editor

        #endregion

        #region Properties

        string appDir = Environment.CurrentDirectory;

        public bool IsShowingRegionDistrictEditor
        {
            get
            {
                return this._isShowingRegionDistrictEditor;
            }
            set
            {
                if (!IsShowingRegionDistrictEditor == value)
                {
                    this._isShowingRegionDistrictEditor = value;
                    RaisePropertyChanged("IsShowingRegionDistrictEditor");
                }
            }
        }


        /// <summary>
        /// Gets whether any of the cases in the system contain invalid EPID values.
        /// </summary>
        public bool ContainsInvalidEpids
        {
            get
            {
                foreach (CaseViewModel caseVM in CaseCollection)
                {
                    if (caseVM.FlagEPIDAsInvalid == true && caseVM.RecStatus == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Gets whether any of the cases in the system have duplicate EPID values.
        /// </summary>
        public bool ContainsDuplicateCases
        {
            get
            {
                foreach (CaseViewModel caseVM in CaseCollection)
                {
                    if (caseVM.FlagAsDuplicate == true && caseVM.RecStatus == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool ContainsAttachment
        {
            get
            {
                foreach (CaseViewModel caseVM in CaseCollection)
                {
                    if (caseVM.FlagAsAttachment == true && caseVM.RecStatus == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool ContainsReviewedCases
        {
            get
            {
                foreach (CaseViewModel caseVM in CaseCollection)
                {
                    if (caseVM.FlagAsReviewed == true)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// Gets/sets the current search string
        /// </summary>
        public string SearchString
        {
            get
            {
                return this._searchString;
            }
            set
            {
                if (this._searchString != value)
                {
                    this._searchString = value;
                    RaisePropertyChanged("SearchString");
                    Search(SearchString);
                }
            }
        }

        /// <summary>
        /// Gets/sets the current progress value for the progress bar that shows up in the app's taskbar icon.
        /// </summary>
        public double TaskbarProgressValue
        {
            get
            {
                return this._taskbarProgressValue;
            }
            set
            {
                if (this._taskbarProgressValue != value)
                {
                    this._taskbarProgressValue = value;
                    RaisePropertyChanged("TaskbarProgressValue");
                }
            }
        }

        /// <summary>
        /// Gets/sets the current progress state for the progress bar that shows up in the app's taskbar icon.
        /// </summary>
        public System.Windows.Shell.TaskbarItemProgressState TaskbarProgressState
        {
            get
            {
                return this._taskbarProgressState;
            }
            set
            {
                if (this._taskbarProgressState != value)
                {
                    this._taskbarProgressState = value;
                    RaisePropertyChanged("TaskbarProgressState");
                }
            }
        }

        public View ContactForm { get; set; }
        private XmlDocument SiteXmlData { get; set; }
        public ObservableCollection<CaseViewModel> CaseCollection
        {
            get
            {
                return this._caseCollection;
            }
            private set
            {
                if (this._caseCollection != value)
                {
                    this._caseCollection = value;
                    RaisePropertyChanged("CaseCollection");
                }
            }
        }

        public ObservableCollection<SiteMergeStatusViewModel> SiteCollection
        {
            get
            {
                return this._siteCollection;
            }
            private set
            {
                if (this._siteCollection != value)
                {
                    this._siteCollection = value;
                    RaisePropertyChanged("SiteCollection");
                }
            }
        }

        public ObservableCollection<CaseViewModel> FilteredCaseCollection
        {
            get
            {
                return this._filteredCaseCollection;
            }
            private set
            {
                if (this._filteredCaseCollection != value)
                {
                    this._filteredCaseCollection = value;
                    RaisePropertyChanged("FilteredCaseCollection");
                }
            }
        }

        public ObservableCollection<ISiteType> SiteTypes
        {
            get
            {
                return this._siteTypes;
            }
            private set
            {
                if (this._siteTypes != value)
                {
                    this._siteTypes = value;
                    RaisePropertyChanged("SiteTypes");
                }
            }
        }

        public string LastSearchTerm { get; private set; }

        public string ProjectPath
        {
            get
            {
                return this.Project.FilePath; // verify this
            }
            set
            {
                this.Project = new Project(value);
                RaisePropertyChanged("ProjectPath");
            }
        }
        #endregion // Properties

        #region Delegates
        public delegate void CaseDataPopulatedHandler(object sender, CaseDataPopulatedArgs e);
        public delegate void InitialSetupRunHandler(object sender, EventArgs e);
        public delegate void InvalidIdDetectedHandler(object sender, InvalidIdDetectedArgs e);
        public delegate void CaseDeletedHandler(object sender, CaseDeletedArgs e);
        public delegate void DuplicateIdDetectedHandler(object sender, DuplicateIdDetectedArgs e);
        #endregion // Delegates

        #region Events
        public event InitialSetupRunHandler InitialSetupRun;
        public event CaseDataPopulatedHandler CaseDataPopulated;
        public event InvalidIdDetectedHandler InvalidIdDetected;
        //public event CaseDeletedHandler CaseDeleted; // Not implemented at this time
        public event DuplicateIdDetectedHandler DuplicateIdDetected;
        private string exportArchive;
        #endregion // Events



        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public DataHelper()
        {
            if (LoadConfig())
            {
                LastSearchTerm = String.Empty;
                TaskbarProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
            }
        }
        #endregion // Constructors

        private void Search(string inputSearchString)
        {
            inputSearchString = inputSearchString.ToLower();

            LastSearchTerm = inputSearchString;

            FilteredCaseCollection.Clear();

            if (String.IsNullOrEmpty(inputSearchString.Trim()))
            {
                foreach (CaseViewModel caseVM in CaseCollection)
                {
                    FilteredCaseCollection.Add(caseVM);
                }
            }
            else
            {
                inputSearchString = inputSearchString.Replace(" =", "=").Replace("= ", "=").Replace(" = ", "=");

                string[] termsArray = inputSearchString.Split(' ');

                int count = 0;

                foreach (CaseViewModel caseVM in CaseCollection)
                {
                    count++;
                    Type t = caseVM.GetType();
                    foreach (PropertyInfo propertyInfo in t.GetProperties())
                    {
                        if (propertyInfo.CanRead)
                        {
                            object value = propertyInfo.GetValue(caseVM, null);

                            if (value == null)
                            {
                                value = String.Empty;
                            }

                            string name = propertyInfo.Name.ToLower();

                            foreach (string term in termsArray)
                            {
                                if (term.ToLower().Equals(value.ToString().ToLower()))
                                {
                                    if (!FilteredCaseCollection.Contains(caseVM))
                                    {
                                        FilteredCaseCollection.Add(caseVM);
                                    }
                                }
                                else if (term.ToLower().Contains("*") && MatchWildcardString(term.ToLower(), value.ToString().ToLower()))
                                {
                                    if (!FilteredCaseCollection.Contains(caseVM))
                                    {
                                        FilteredCaseCollection.Add(caseVM);
                                    }
                                }
                                //Added to search to allow a type while search action 5/15/15
                                else if (value.ToString().ToLower().Contains(term.ToLower()) && !term.ToLower().Contains("*"))
                                {
                                    if (!FilteredCaseCollection.Contains(caseVM))
                                    {
                                        FilteredCaseCollection.Add(caseVM);
                                    }
                                }
                            }
                        }
                    }
                }
            } //USE THIS ONE 7/31/15
        }

        public bool MatchWildcardString(string pattern, string input)
        {
            if (String.Compare(pattern, input) == 0)
            {
                return true;
            }
            else if (String.IsNullOrEmpty(input))
            {
                if (String.IsNullOrEmpty(pattern.Trim(new Char[1] { '*' })))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (pattern.Length == 0)
            {
                return false;
            }
            else if (pattern[0] == '?')
            {
                return MatchWildcardString(pattern.Substring(1), input.Substring(1));
            }
            else if (pattern[pattern.Length - 1] == '?')
            {
                return MatchWildcardString(pattern.Substring(0, pattern.Length - 1), input.Substring(0, input.Length - 1));
            }
            else if (pattern[0] == '*')
            {
                if (MatchWildcardString(pattern.Substring(1), input))
                {
                    return true;
                }
                else
                {
                    return MatchWildcardString(pattern, input.Substring(1));
                }
            }
            else if (pattern[pattern.Length - 1] == '*')
            {
                if (MatchWildcardString(pattern.Substring(0, pattern.Length - 1), input))
                {
                    return true;
                }
                else
                {
                    return MatchWildcardString(pattern, input.Substring(0, input.Length - 1));
                }
            }
            else if (pattern[0] == input[0])
            {
                return MatchWildcardString(pattern.Substring(1), input.Substring(1));
            }
            return false;
        }

        /// <summary>
        /// Used to repopulate all collections. This is an expensive process and should only be called when absolutely necessary.
        /// </summary>
        public override void RepopulateCollections()
        {
            SetupDatabase();
            ClearCollections();
            PopulateCollections();
            //Task.Factory.StartNew(PopulateCollections, System.Threading.CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Used to clear all case, contact, and link objects from memory.
        /// </summary>
        protected override void ClearCollections()
        {
            CaseCollection.Clear(); // = new ObservableCollection<CaseViewModel>();
            FilteredCaseCollection.Clear(); // = new ObservableCollection<CaseViewModel>();
        }

        /// <summary>
        /// Used to get a list of all duplicate cases, using the EPID field as the match value.
        /// </summary>
        /// <returns></returns>
        private List<CaseViewModel> GetDuplicateCasesBasedOnID()
        {
            List<CaseViewModel> duplicates = new List<CaseViewModel>();
            foreach (CaseViewModel iCaseVM in CaseCollection)
            {
                UnflagCaseAsDuplicate(iCaseVM);

                string iID = iCaseVM.EPID;

                foreach (CaseViewModel jCaseVM in CaseCollection)
                {
                    if (jCaseVM != iCaseVM)
                    {
                        string jID = jCaseVM.EPID;

                        if (jID.Equals(iID))
                        {
                            if (!duplicates.Contains(iCaseVM) && !duplicates.Contains(jCaseVM) && !String.IsNullOrEmpty(jID) && (jCaseVM.RecStatus == 1 && iCaseVM.RecStatus == 1))
                            {
                                duplicates.Add(iCaseVM);
                                FlagCaseAsDuplicate(iCaseVM);
                                FlagCaseAsDuplicate(jCaseVM);
                            }
                        }
                    }
                }


                object permAlert = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm"); //MLHIDE


                if (iCaseVM.Reviewed == permAlert.ToString())
                {
                    FlagCaseAsInvalidEpid(iCaseVM);
                }

            }

            return duplicates;
        }

        /// <summary>
        /// Used to flag duplicate records based on EPID
        /// </summary>
        /// <returns></returns>
        private void FlagDuplicateRecords()
        {
            foreach (CaseViewModel iCaseVM in CaseCollection)
            {
                string iID = iCaseVM.Type;
                string iGeneralDiseaseCategory = iCaseVM.GeneralDiseaseNameSuspected;

                if (iCaseVM.RecStatus == 1)
                {

                    foreach (CaseViewModel jCaseVM in CaseCollection)
                    { // gfj0

                        DateTime DateIssue1;

                        DateTime DateIssue2;

                        if (iCaseVM.DateAssigned != null)
                        {
                            DateIssue1 = DateTime.Parse(iCaseVM.DateAssigned.ToString());
                        }
                        else
                        {
                            DateIssue1 = DateTime.Now;
                        }

                        if (jCaseVM.DateAssigned != null)
                        {
                            DateIssue2 = DateTime.Parse(jCaseVM.DateAssigned.ToString());
                        }
                        else
                        {
                            DateIssue2 = DateTime.Now;
                        }

                        var ts = DateIssue1 - DateIssue2;
                        var diff = Math.Abs(ts.TotalDays);

                        if (jCaseVM != iCaseVM && jCaseVM.RecStatus == 1)
                        {
                            string jID = jCaseVM.Type;
                            string jGeneralDiseaseCategory = jCaseVM.GeneralDiseaseNameSuspected;

                            if (jID.Equals(iID) && jGeneralDiseaseCategory.Equals(iGeneralDiseaseCategory) && diff <= 3) // gfj 
                            {
                                if (!String.IsNullOrEmpty(jID))
                                {
                                    FlagCaseAsDuplicate(iCaseVM);
                                    FlagCaseAsDuplicate(jCaseVM);
                                }
                            }
                        }
                    }
                }

                // gfj 3-28-2020
                //var testvar = iCaseVM.DateRecordCreated.ToString();

                //if (iCaseVM.DateRecordCreated.ToString() != "" && iCaseVM.DateRecordCreated.ToString() != null)
                //{
                //    DateTime DateComp1 = DateTime.Parse(iCaseVM.DateRecordCreated.ToString());
                //    DateTime DateComp2 = DateTime.Parse(iCaseVM.DateRecordUpdated.ToString());
                //    var dc = DateComp1 - DateComp2;
                //    var dcDiff = Math.Abs(dc.TotalMinutes);
                //}
                //else
                //{
                //    MessageBox.Show("Date Creation is missing.");
                //    //break;

                //    DateTime DateComp1 = DateTime.Now;
                //    DateTime DateComp2 = DateTime.Now;
                //    var dc = DateComp1 - DateComp2;
                //    var dcDiff = Math.Abs(dc.TotalMinutes);

                //}
                //if (StatusCheckbox == )  
                //    {

                //    } gfj check

                //Epi.Configuration.Load("Configuration\\EpiInfo.config.xml");

                object permAlert = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm");

                // temporarily set permalert to the sitecode

                if (permAlert == null)
                {
                    MessageBox.Show("Site settings must be configured", "Warning");
                    Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable("SiteCodePerm", 1);
                    Epi.Menu.EpiInfoMenuManager.SetPermanentVariable("SiteCodePerm", "3");
                    permAlert = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm");
                }

                //if (iCaseVM.LastUpdate != permAlert.ToString() && permAlert.ToString() != iCaseVM.Reviewed)
                //if (iCaseVM.LastUpdate != permAlert.ToString() && iCaseVM.FlagEPIDAsInvalid != true && iCaseVM.LastUpdate != "0")
                //{
                //    if (iCaseVM.RecStatus == 1)
                //    {


                //        //if (iCaseVM.LastUpdate != permAlert.ToString() && iCaseVM.RecStatus == 1 && iCaseVM.FlagAsReviewed == false)
                //        //if (iCaseVM.DateRecordUpdated > iCaseVM.DateRecordCreated && iCaseVM.RecStatus == 1 && iCaseVM.FlagAsReviewed == false)
                //        //if (iCaseVM.DateRecordUpdated > iCaseVM.DateRecordCreated && iCaseVM.RecStatus == 1)
                //        //{
                //            //if (dcDiff > 3)
                //            //{
                //            FlagCaseAsInvalidEpid(iCaseVM);   // updated comparison gfj
                //                                              //FlagCaseAsReviewed(iCaseVM);

                //            //}
                //        //}
                //    }
                //}

                //MessageBox.Show(iCaseVM.FlagEPIDAsInvalid.ToString());

                //FlagCaseAsReviewed2(iCaseVM);

                //if (iCaseVM.RecStatus == 1 && permAlert.ToString() == iCaseVM.Reviewed)
                //    //if (iCaseVM.RecStatus == 1)

                //    //if (iCaseVM.Reviewed == iCaseVM.LastUpdate)
                //{

                //    FlagCaseAsReviewed(iCaseVM);
                //}

                OleDbConnection connect = new OleDbConnection();

                connect.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + appDir + "\\Projects\\TrackingMaster\\TrackingMasterNew.MDB";   //MLHIDE

                connect.Open();

                OleDbCommand command = new OleDbCommand();

                command.Connection = connect;

                var permvar2 = EpiInfoMenuManager.GetPermanentVariableValue("SiteCodePerm"); //MLHIDE

                string UserSettingPERM = null;
                string UserSettingName = null;

                if (permvar2 != null)
                {
                    if (permvar2.ToString() == "3")
                    {
                        UserSettingPERM = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingResident").ToString();  //MLHIDE
                        UserSettingName = "DeletedResident"; //MLHIDE
                    }
                    if (permvar2.ToString() == "2")
                    {
                        UserSettingPERM = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC").ToString(); //MLHIDE
                        UserSettingName = "DeletedPC"; //MLHIDE
                    }
                    if (permvar2.ToString() == "1")
                    {
                        UserSettingPERM = EpiInfoMenuManager.GetPermanentVariableValue("UserSettingRAPC").ToString(); //MLHIDE
                        UserSettingName = "DeletedNational"; //MLHIDE
                    }
                }

                //command.CommandText = "SELECT Requiredwrittenmaterials13.IsClickAttachment FROM Requiredwrittenmaterials13 INNER JOIN Requiredwrittenmaterials ON Requiredwrittenmaterials13.GlobalRecordId = Requiredwrittenmaterials.GlobalRecordId WHERE (((Requiredwrittenmaterials.FKEY)='" + iCaseVM.Guid + "'));";   //MLHIDE

                command.CommandText = "SELECT Requiredwrittenmaterials13.IsClickAttachment FROM ActivityLog INNER JOIN(Requiredwrittenmaterials13 INNER JOIN Requiredwrittenmaterials ON Requiredwrittenmaterials13.GlobalRecordId = Requiredwrittenmaterials.GlobalRecordId) ON ActivityLog.GUID = Requiredwrittenmaterials13.AttachGUID WHERE(((Requiredwrittenmaterials.FKEY) = '" + iCaseVM.Guid + "') AND (Not(ActivityLog.CreatedBy) ='" + UserSettingPERM + "') AND ((ActivityLog." + UserSettingName + ")= 0));";   //MLHIDE


                //UPDATE Projects4 INNER JOIN(Projects INNER JOIN ((Requiredwrittenmaterials INNER JOIN Requiredwrittenmaterials13 ON Requiredwrittenmaterials.GlobalRecordId = Requiredwrittenmaterials13.GlobalRecordId) INNER JOIN ActivityLog ON Requiredwrittenmaterials13.AttachGUID = ActivityLog.GUID) ON Projects.GlobalRecordId = Requiredwrittenmaterials.FKEY) ON Projects4.GlobalRecordId = Projects.GlobalRecordId SET Projects4.HasAttachment = 'Yes' WHERE((Not(ActivityLog.CreatedBy) = '" + [UserSettingPERM] + "'));

                var IsClickAttachment = command.ExecuteScalar();

                if (IsClickAttachment != null)
                {
                    if (iCaseVM.HasAttachment == "Yes" && IsClickAttachment.ToString() != "Yes") //MLHIDE
                    {
                        FlagCaseAsAttachment(iCaseVM);
                    }
                }

                connect.Close();

            }
        }

        /// <summary>
        /// Flags a given case as having an invalid EPID
        /// </summary>
        /// <param name="caseVM">The case to flag as having an invalid EPID</param>
        private void FlagCaseAsInvalidEpid(CaseViewModel caseVM)
        {
            caseVM.FlagEPIDAsInvalid = true; // bad; move this to the case model at some point
        }

        /// <summary>
        /// Flags a given case as being a duplicate record
        /// </summary>
        /// <param name="caseVM">The case to flag as being a duplicate</param>
        private void FlagCaseAsDuplicate(CaseViewModel caseVM)
        {
            caseVM.FlagAsDuplicate = true;

        }

        private void FlagCaseAsAttachment(CaseViewModel caseVM)
        {
            caseVM.FlagAsAttachment = true;

        }

        private void FlagCaseAsReviewed(CaseViewModel caseVM)
        {
            caseVM.FlagAsReviewed = true;

        }
        private void FlagCaseAsReviewed2(CaseViewModel caseVM)
        {
            caseVM.FlagAsReviewed = false;

        }

        /// <summary>
        /// Unflags a given case as being a duplicate record
        /// </summary>
        /// <param name="caseVM">The case to flag as being a duplicate</param>
        private void UnflagCaseAsDuplicate(CaseViewModel caseVM)
        {
            caseVM.FlagAsDuplicate = false;
        }
        private void UnflagCaseAsAttachment(CaseViewModel caseVM)
        {
            caseVM.FlagAsAttachment = false;
        }

        /// <summary>
        /// Loads data from a given data row into the given case view model
        /// </summary>
        /// <param name="row">The row that contains the data to be loaded</param>
        /// <param name="c">The case view model whose fields will be updated</param>
        private void LoadCaseData(DataRow row, CaseViewModel c)
        {
            string appDir = Environment.CurrentDirectory;

            double? ageYears = null;
            double? ageMonths = null;
            double? year = null;

            // TODO: Move this code to Init/constructor so we read XML only 1 time - may be during the app load
            // Get the path of XML Menafrinet.ViewModel.Properties.Settings.Default.
            // Load the XML - XmlDoc 
            // Write a method to query XML with project id and it should return its stauts IsReviewed true/false
            // Write a method to update the project status IsReviewed true/false
            // At end of the app's execution, right the xml file back to the disk


            //if (!String.IsNullOrEmpty(row["Annee"].ToString())) db name change
            //if (!String.IsNullOrEmpty(row["EpidYear"].ToString()))
            //{
            //    //year = double.Parse(row["Annee"].ToString()); db name change
            //    year = double.Parse(row["EpidYear"].ToString());
            //    c.Year = Math.Truncate(year.Value);
            //}
            //if (!String.IsNullOrEmpty(row["Agean"].ToString())) db name change
            //if (!String.IsNullOrEmpty(row["AgeYear"].ToString()))
            //{
            //    //ageYears = double.Parse(row["Agean"].ToString()); db name change
            //    ageYears = double.Parse(row["AgeYear"].ToString());
            //    c.AgeYears = Math.Truncate(ageYears.Value);
            //}
            //if (!String.IsNullOrEmpty(row["Agemois"].ToString())) db name change
            //if (!String.IsNullOrEmpty(row["AgeMonths"].ToString()))       
            //{
            //    //ageMonths = double.Parse(row["Agemois"].ToString()); db name change
            //    ageMonths = double.Parse(row["AgeMonths"].ToString());
            //    c.AgeMonths = Math.Truncate(ageMonths.Value);
            //}

            //c.Age = ageYears;

            //if (c.Age.HasValue && ageMonths.HasValue)
            //{
            //    c.Age = c.Age.Value + (ageMonths.Value / 12);
            //}
            //else if (!c.Age.HasValue && ageMonths.HasValue)
            //{
            //    c.Age = (ageMonths.Value / 12);
            //}

            //if (c.Age.HasValue)
            //{
            //    c.Age = Math.Truncate(c.Age.Value);
            //}

            c.RecStatus = 1;

            if (row["RecStatus"] != DBNull.Value)
            {
                c.RecStatus = byte.Parse(row["RecStatus"].ToString());
            }

            c.Type = row["Type"].ToString();
            c.ProjectClassification = row["ProjectClassification"].ToString();
            if (!String.IsNullOrEmpty(row["DateAssigned"].ToString()))
            {
                c.DateAssigned = DateTime.Parse(row["DateAssigned"].ToString());
            }
            c.projecttitle = row["projecttitle"].ToString();
            c.LeadResident = row["LeadResident"].ToString();
            c.City = row["City"].ToString();
            c.Reviewed = row["Reviewed"].ToString(); //MLHIDE
            c.ICD10CodeSuspected = row["ICD10CodeSuspected"].ToString(); //MLHIDE
            c.leadcohort = row["leadcohort"].ToString(); //MLHIDE
            c.ProjectStatus = row["ProjectStatus"].ToString(); //MLHIDE

            var st = " ";

            if (c.ProjectStatus != null)
            {
                if (c.ProjectStatus == "01") { st = "Protocol Under Development"; };
                if (c.ProjectStatus == "02") { st = "Protocol Under Review (IRB, NRD)"; };
                if (c.ProjectStatus == "03") { st = "Data Collection Underway"; };
                if (c.ProjectStatus == "04") { st = "Data Analysis and Report Preparation Underway"; };
                if (c.ProjectStatus == "05") { st = "Report Under Review"; };
                if (c.ProjectStatus == "06") { st = "Project Presentation Completed"; };
                if (c.ProjectStatus == "07") { st = "Final Report Submitted"; };
            }

            c.ProjectStatus = string.IsNullOrEmpty(st) ? "Missing" : st;
            c.HasAttachment = row["HasAttachment"].ToString();
            c.LastUpdate = row["LastUpdate"].ToString();
            c.Setting = row["Setting"].ToString();
            c.DiseaseSuspected = row["DiseaseSuspected"].ToString();
            c.City = row["City"].ToString();
            c.Country = row["Country"].ToString();
            c.UniversityTrainingInstitution = row["UniversityTrainingInstitution"].ToString();
            c.DiseaseCategorySuspected = row["DiseaseCategorySuspected"].ToString();
            c.MentorFullName = row["MentorFullName"].ToString();
            c.SupervisorFullName = row["SupervisorFullName"].ToString();
            c.GeneralDiseaseNameSuspected = row["GeneralDiseaseNameSuspected"].ToString();
            c.UniqueKey = int.Parse(row["UniqueKey"].ToString());

            //// TODO: During LoadCaseData
            //// You have the Project's Id, query the xml and get the status vale
            //// c.FlasAsReviewed = ReviewStatusHelper.GetReviewStatus(ProjectUniqueID);
            bool reviewFlag;
            bool.TryParse(row["Reviewed"] as string, out reviewFlag);
            c.FlagAsReviewed = reviewFlag;

            //OleDbConnection connect = new OleDbConnection();
            //connect.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Projects\TrackingMaster\TrackingMasterNew.MDB";
            //connect.Open();
            //OleDbCommand ChkCommand = new OleDbCommand();
            //ChkCommand.Connection = connect;
            //ChkCommand.CommandText = "SELECT [REVIEWED] FROM StatusTab41 WHERE (((StatusTab41.UniqueID)=" + int.Parse(row["UniqueKey"].ToString()) + "));";

            //int st = Convert.ToInt32(ChkCommand.ExecuteScalar());

            ////MessageBox.Show("Rev Status= " + st);
            //reviewFlag = Convert.ToBoolean(st);

            //connect.Close();

            ////CheckBox_Update(c.UniqueKey);
            //c.FlagAsReviewed = reviewFlag;
            //c.Reviewed = st.ToString();



            ////c.City = row["Ville"].ToString(); db name change
            //c.City = row["Neighborhood"].ToString();
            ////c.Classification = row["Classificationfinale"].ToString(); db name change
            //c.Classification = row["FinalClassification"].ToString();

            ////if (!String.IsNullOrEmpty(row["DateDebut"].ToString())) db name change
            //if (!String.IsNullOrEmpty(row["DateOnset"].ToString()))
            //{
            //    //c.DateOnset = DateTime.Parse(row["DateDebut"].ToString()); db name change
            //    c.DateOnset = DateTime.Parse(row["DateOnset"].ToString());
            //}

            ////c.FinalLabResult = row["ResultatFinalLNR"].ToString(); db name change again on 9/16/15 LNR to NRL
            //c.FinalLabResult = row["FinalResultNRL"].ToString();
            ////c.Outcome = row["Evolution"].ToString().Trim(); db name change
            //c.Outcome = row["Outcome"].ToString().Trim();

            //switch (row["Evolution"].ToString().Trim()) db name change
            //switch (row["Outcome"].ToString().Trim())
            //{
            //    case "1":
            //        if (System.Threading.Thread.CurrentThread.CurrentCulture.ToString().ToLower().Equals("fr-fr"))
            //        {
            //            c.Outcome = "Vivant";
            //        }
            //        else if (System.Threading.Thread.CurrentThread.CurrentCulture.ToString().ToLower().Equals("en-us"))
            //        {
            //            //c.Outcome = "Alive"; //temp fix to translation
            //            c.Outcome = "Vivant";
            //        }
            //        break;
            //    case "2":
            //        if (System.Threading.Thread.CurrentThread.CurrentCulture.ToString().ToLower().Equals("fr-fr"))
            //        {
            //            c.Outcome = "Décédé";
            //        }
            //        else if (System.Threading.Thread.CurrentThread.CurrentCulture.ToString().ToLower().Equals("en-us"))
            //        {
            //            //c.Outcome = "Dead";
            //            c.Outcome = "Décédé";
            //        }
            //        break;
            //    case "3":
            //        if (System.Threading.Thread.CurrentThread.CurrentCulture.ToString().ToLower().Equals("fr-fr"))
            //        {
            //            c.Outcome = "Inconnu";
            //        }
            //        else if (System.Threading.Thread.CurrentThread.CurrentCulture.ToString().ToLower().Equals("en-us"))
            //        {
            //            //c.Outcome = "Unknown";
            //            c.Outcome = "Inconnu";
            //        }
            //        break;
            //}

            ////c.DistrictResidence = row["Districtresidence"].ToString(); db name change
            //c.DistrictResidence = row["DistrictOfResidence"].ToString();
            ////c.DistrictReporting = row["Districts"].ToString(); db name change
            //c.DistrictReporting = row["District"].ToString();

            ////if (!String.IsNullOrEmpty(row["Semaine"].ToString())) db name change
            //if (!String.IsNullOrEmpty(row["EpidWeek"].ToString()))
            //{
            //    //c.EpiWeek = int.Parse(row["Semaine"].ToString()); db name change
            //    c.EpiWeek = int.Parse(row["EpidWeek"].ToString());
            //}

            if (!String.IsNullOrEmpty(row["FirstSaveTime"].ToString()))
            {
                c.DateRecordCreated = DateTime.Parse(row["FirstSaveTime"].ToString());
            }

            if (!String.IsNullOrEmpty(row["LastSaveTime"].ToString()))
            {
                c.DateRecordUpdated = DateTime.Parse(row["LastSaveTime"].ToString());

                if (c.DateRecordUpdated > c.DateRecordCreated)
                {
                    this.FlagEPIDAsInvalid = true;
                }

            }

            ////if (!String.IsNullOrEmpty(row["DateDebut"].ToString())) db name change
            //if (!String.IsNullOrEmpty(row["DateOnset"].ToString()))
            //{
            //    //c.DOB = DateTime.Parse(row["DateDebut"].ToString()); db name change
            //    c.DOB = DateTime.Parse(row["DateOnset"].ToString());
            //}

            //c.EPID = row["EpidShow"].ToString();
            ////c.EPID = row["EpidNumber"].ToString(); db name change chng bk to orig
            ////c.FirstName = row["Prenom"].ToString(); db name change
            //c.FirstName = row["FirstName"].ToString();
            ////c.LastName = row["NomPrenom"].ToString(); db name change
            //c.LastName = row["FamilyName"].ToString();
            ////c.NameOfParent = row["NomParent"].ToString(); db name change
            //c.NameOfParent = row["ParentName"].ToString();
            ////c.Sex = row["Sexe"].ToString(); db name change
            //c.Sex = row["Sex"].ToString();
            //c.UrbanRural = row["UrbanRural"].ToString();
            c.Guid = row["t.GlobalRecordId"].ToString();
            ////c.LPPerformed = row["PonctionLombaire"].ToString(); db name change
            //c.LPPerformed = row["SpecimenCollected"].ToString();
            ////c.HealthFacility = row["FormationSanitaire"].ToString(); db name change
            //c.HealthFacility = row["HealthFacility"].ToString();
        }
        string selSTMT;
        private Case CreateCaseFromGuid(string guid)
        {
            CaseViewModel c = new CaseViewModel();

            //Query selectQuery = Database.CreateQuery("SELECT[t.GlobalRecordId], UniqueKey, LastSaveTime, FirstSaveTime, DateAssigned, FETP, Type, LeadResident, ProjectClassification, MentorFullName, SupervisorFullName, projecttitle, Setting, RecStatus  " +

            if (DelRecStatus == true)
            {
                selSTMT = " AND [t.RecStatus] > 0";
            }
            else
            {
                selSTMT = " ";
            }

            Query selectQuery = Database.CreateQuery("SELECT *  " +

                // Query selectQuery = Database.CreateQuery("SELECT * " +
                CaseForm.FromViewSQL + " " +
                "WHERE [t.GlobalRecordId] = '" + guid + "'" + selSTMT); //MLHIDE
            DataTable dt = Database.Select(selectQuery);

            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];
                LoadCaseData(row, c);
            }
            return c.Case;
        }

        private void AddCase(Case newCase)
        {
            if (CaseCollection == null)
                return;

            CaseViewModel newCaseVM = new CaseViewModel { Case = newCase };
            CaseCollection.Add(newCaseVM);

            this.Search(LastSearchTerm);
        }

        public ICommand ToggleRegionEditorCommand { get { return new RelayCommand<bool>(ToggleRegionEditorCommandExecute); } }
        private void ToggleRegionEditorCommandExecute(bool show)
        {
            IsShowingRegionDistrictEditor = show;
        }

        public ICommand DeleteCase { get { return new RelayCommand<string>(DeleteCaseExecute); } }
        void DeleteCaseExecute(string caseGuid)
        {
            Logger.Log(DateTime.Now + ":  " +
                System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString() + ": " +
                "Case deletion requested for record with GUID " + caseGuid);

            if (CaseCollection == null)
                return;

            CaseViewModel cvm = null;
            for (int i = CaseCollection.Count - 1; i >= 0; i--)
            {
                cvm = CaseCollection[i];
                if (cvm.Guid == caseGuid)
                {
                    CaseCollection.Remove(cvm);
                    break;
                }
            }

            for (int i = FilteredCaseCollection.Count - 1; i >= 0; i--)
            {
                cvm = FilteredCaseCollection[i];
                if (cvm.Guid == caseGuid)
                {
                    FilteredCaseCollection.Remove(cvm);
                    break;
                }
            }

            int rows = 0;
            Query deleteQuery = Database.CreateQuery("DELETE * FROM [" + CaseForm.TableName + "] WHERE [GlobalRecordId] = '" + caseGuid + "'");
            rows = Database.ExecuteNonQuery(deleteQuery);

            if (rows != 1)
            {
                throw new ApplicationException("A deletion was requested for case with GUID " + caseGuid + " but the case no longer exists.");
            }

            foreach (Epi.Page page in CaseForm.Pages)
            {
                deleteQuery = Database.CreateQuery("DELETE * FROM [" + page.TableName + "] WHERE [GlobalRecordId] = '" + caseGuid + "'");
                Database.ExecuteNonQuery(deleteQuery);
            }
        }

        public ICommand UpdateOrAddCase { get { return new RelayCommand<string>(UpdateOrAddCaseExecute); } }

        public bool FlagEPIDAsInvalid { get; private set; }
        public object StatusCheckbox { get; private set; }

        void UpdateOrAddCaseExecute(string caseGuid)
        {
            if (CaseCollection == null)
                return;

            Case c = CreateCaseFromGuid(caseGuid);
            CaseViewModel newCaseVM = null;
            bool found = false;
            foreach (var iCase in CaseCollection)
            {
                if (iCase.Guid == caseGuid)
                {
                    newCaseVM = iCase;
                    iCase.Update.Execute(c);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                AddCase(c);
            }

            foreach (CaseViewModel caseVM in this.CaseCollection)
            {
                UnflagCaseAsDuplicate(caseVM);
            }

            FlagDuplicateRecords();

            //List<CaseViewModel> duplicates = GetDuplicateCasesBasedOnID();

            //if (duplicates.Count > 0)
            //{
            //    if (DuplicateIdDetected != null)
            //    {
            //        DuplicateIdDetected(this, new DuplicateIdDetectedArgs(duplicates));
            //    }
            //}
        }

        public void UpdateSite(string siteCode)
        {
            foreach (SiteMergeStatusViewModel svm in SiteCollection)
            {
                if (svm.SiteName == siteCode)
                {
                    svm.LastMerged = DateTime.Now;
                    svm.DaysElapsed = 0;

                    foreach (XmlNode node in SiteXmlData.ChildNodes[0].ChildNodes)
                    {
                        if (node.Attributes["Name"].Value.Equals(siteCode))
                        {
                            node.ChildNodes[0].InnerText = svm.LastMerged.Value.Ticks.ToString();
                        }
                    }

                    string path = Path.GetDirectoryName(
                     Assembly.GetAssembly(typeof(Common)).CodeBase);

                    SiteXmlData.Save(System.IO.Path.Combine(path.Replace("file:\\", string.Empty), @"Projects\TrackingMaster\SiteMergeStatus.xml"));
                    //SiteXmlData.Save(System.IO.Path.Combine(path.Replace("file:\\", string.Empty), @"Projects\TrackingMaster\SiteMergeStatusGFJ.xml"));
                    break;
                }
            }
        }

        public void LoadSiteMergeData()
        {

            SiteCollection = new ObservableCollection<SiteMergeStatusViewModel>();

            Districts districts = new Districts();
            Regions regions = new Regions();

            XmlDocument xmlDoc = new XmlDocument();
            SiteXmlData = xmlDoc;
            string path = Path.GetDirectoryName(
            Assembly.GetAssembly(typeof(Common)).CodeBase);

            xmlDoc.Load(System.IO.Path.Combine(path, "Projects/TrackingMaster/SiteMergeStatus.xml"));

            foreach (XmlNode node in xmlDoc.ChildNodes[0].ChildNodes)
            {
                SiteMergeStatusViewModel siteVM = new SiteMergeStatusViewModel();
                siteVM.SiteName = node.Attributes["Name"].Value;
                siteVM.SiteCode = node.Attributes["Code"].Value;
                string region = node.Attributes["Region"].Value;
                string regionCode = node.Attributes["RegionCode"].Value;

                District d = new District(siteVM.SiteName, siteVM.SiteCode, region);
                districts.Sites.Add(d);

                Region r = new Region(region, regionCode);
                //bool found = false;
                //foreach (Locations.ISite site in regions.Sites)
                //{
                //    if (site.Name == region)
                //    {
                //        found = true;
                //        break;
                //    }
                //}
                //if (!found)
                //{
                //    regions.Sites.Add(r);
                //}

                //gfj0 set color to yellow
                if (node.ChildNodes.Count > 0 && node.ChildNodes[0].InnerText != null)
                {
                    long ticks = long.Parse(node.ChildNodes[0].InnerText.ToString());
                    siteVM.LastMerged = new DateTime(ticks);
                }

                if (siteVM.LastMerged.HasValue)
                {
                    DateTime now = DateTime.Now;

                    TimeSpan ts = now - siteVM.LastMerged.Value;
                    siteVM.DaysElapsed = (int)ts.TotalDays;
                }

                SiteCollection.Add(siteVM);
            }

            SiteTypes.Add(districts);
            SiteTypes.Add(regions);
            SiteTypes.Add(new NRL());
            SiteTypes.Add(new MoH());
        }

        /// <summary>
        /// Populates the data helper's internal collection of cases
        /// </summary>
        protected void PopulateCollections()
        { // gfj site merge panel
            LoadSiteMergeData();

            System.Diagnostics.Stopwatch swMain = new System.Diagnostics.Stopwatch();
            swMain.Start();

            System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
            sw1.Start();
            DataTable CaseTable = GetCasesTable(); // time-consuming
            sw1.Stop();
            System.Diagnostics.Debug.Print("CaseTable Get: " + sw1.Elapsed.TotalMilliseconds.ToString());

            DataView CaseView = new DataView(CaseTable, String.Empty, String.Empty, DataViewRowState.CurrentRows);

            System.Diagnostics.Stopwatch sw3 = new System.Diagnostics.Stopwatch();
            sw3.Start();
            //CaseView.Sort = "EPID_CASNO"; db name change
            CaseView.Sort = "DateAssigned"; //MLHIDE
            foreach (DataRowView rowView in CaseView)
            {
                // gfj now
                CaseViewModel c = new CaseViewModel();
                LoadCaseData(rowView.Row, c);
                CaseCollection.Add(c);
            }
            sw3.Stop();
            System.Diagnostics.Debug.Print("Load Case Data: " + sw3.Elapsed.TotalMilliseconds.ToString());

            if (CaseDataPopulated != null)
            {
                CaseDataPopulated(this, new CaseDataPopulatedArgs());
            }

            //List<CaseViewModel> duplicates = GetDuplicateCasesBasedOnID();

            //if (duplicates.Count > 0)
            //{
            //    if (DuplicateIdDetected != null)
            //    {
            //        DuplicateIdDetected(this, new DuplicateIdDetectedArgs(duplicates));
            //    }
            //}

            FlagDuplicateRecords();

            // Check for valid IDs
            List<string> invalidIds = new List<string>();

            foreach (CaseViewModel caseVM in CaseCollection)
            {
                // gfj now2
                string actualID = caseVM.Guid;
            } // caseVM.DateRecordCreated.ToString().Substring(1, 8);

            if (InvalidIdDetected != null && invalidIds.Count > 0)
            {
                InvalidIdDetected(this, new InvalidIdDetectedArgs(invalidIds));
            }

            swMain.Stop();
            System.Diagnostics.Debug.Print("PopulateCollections END: " + swMain.Elapsed.TotalMilliseconds.ToString());

            FilteredCaseCollection.Clear();
            foreach (CaseViewModel caseVM in CaseCollection)
            {
                FilteredCaseCollection.Add(caseVM);
            }

            TaskbarProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
        }

        /// <summary>
        /// Sets up the database for the first time.
        /// </summary>
        private void SetupDatabase()
        {
            Database = Project.CollectedData.GetDatabase();
            foreach (View view in Project.Views)
            {
                // ugly hack... this is needed to get the view object without needing to specify the view name (which may differ between countries)
                // and because we can't just say Project.Views[0]
                CaseForm = view;
                break;
            }

            #region Set meta DB info

            //if (!Database.ColumnExists("metaDbInfo", "ProjectTrackingNew"))
            //{
            //    List<Epi.Data.TableColumn> tcList = new List<Epi.Data.TableColumn>();
            //    tcList.Add(new Epi.Data.TableColumn("TrackingMaster", GenericDbColumnType.Boolean, false));

            //    foreach (Epi.Data.TableColumn tableColumn in tcList)
            //    {
            //        Database.AddColumn("metaDbInfo", tableColumn);
            //    }

            //    Query updateQuery = Database.CreateQuery("UPDATE metaDbInfo SET [TrackingMaster] = true");
            //    Database.ExecuteNonQuery(updateQuery);

            //    FileInfo fi = new FileInfo(this.ProjectPath);
            //    DirectoryInfo di = fi.Directory;

            //    string exports = System.IO.Path.Combine(di.FullName, "DataExports");
            //    string exportArchives = System.IO.Path.Combine(exports, "Archive");
            //    string imports = System.IO.Path.Combine(di.FullName, "DataImports");
            //    string importArchives = System.IO.Path.Combine(imports, "Archives");

            //    System.IO.Directory.CreateDirectory(exports);
            //    System.IO.Directory.CreateDirectory(exportArchive);
            //    System.IO.Directory.CreateDirectory(imports);
            //    System.IO.Directory.CreateDirectory(importArchives);


            //    if (InitialSetupRun != null)
            //    {
            //        InitialSetupRun(this, new EventArgs());
            //    }
            //}
            #endregion // Set meta DB info
        }

        private void CheckBox_Update(int Uniquekey)
        {

            //DataTable results;

            string appDir = Environment.CurrentDirectory;

            //string connString =
            //    "Provider=Microsoft.ACE.OLEDB.12.0" +
            //    ";data Source=" + "Projects\\TrackingMaster\\TrackingMasterNew.mdb";

            ////results = new DataTable();


            //using (OleDbConnection conn = new OleDbConnection(connString))
            //{
            //    OleDbCommand cmd = new OleDbCommand();

            //    conn.Open();

            //    //OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

            //    //if (adapter == null)

            ////    //{
            //        OleDbConnection connect = new OleDbConnection();
            //        connect.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Projects\TrackingMaster\TrackingMasterNew.MDB";
            //        connect.Open();
            //        OleDbCommand ChkCommand = new OleDbCommand();
            //        ChkCommand.Connection = connect;
            //        //ChkCommand.CommandText = "UPDATE StatusTab41 SET [REVIEWED]=0 WHERE (((StatusTab41.UniqueID)=[" + Uniquekey + "]));";
            //        //ChkCommand.ExecuteNonQuery();
            //        //connect.Close();

            //        ChkCommand.CommandText = "SELECT [REVIEWED] FROM StatusTab41 WHERE (((StatusTab41.UniqueID)=[" + Uniquekey + "]));";
            //        reviewFlag = Convert.ToBoolean(ChkCommand.ExecuteScalar());
            //        connect.Close();

            //}

            //public void SendMessageForAwaitAll()
            //{
            //    SendMessage("Updating district codes", string.Empty, ServerUpdateType.LockAllClientIsRefreshing);
            //}

            //****================== Reports_ReportProcessing Execution =========================== ****  

            //try
            //{


            //}
        }

        public void FillResidents_Sub()
        {

            XmlDocument doc = new XmlDocument();
            //XmlElement root2 = doc.DocumentElement;
            //XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            //doc.InsertBefore(xmlDeclaration, root2);

            //XmlElement element1 = doc.CreateElement(string.Empty, "<rootElement>", string.Empty);
            XmlText text1 = doc.CreateTextNode("ResidentList");
            //element1.AppendChild(text1);

            string appDir = Environment.CurrentDirectory;


            doc.Load(appDir + "\\Projects\\TrackingMaster\\SiteMergeStatus.xml");


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

                            bool found = false;

                            foreach (XmlNode tnode in doc.DocumentElement)
                            {
                                string chkvar = tnode.Attributes[0].Value.ToString();

                                if (District2 == chkvar)
                                {
                                    outputFile.WriteLine("    <LastReported>" + tnode.InnerText.ToString() + "</LastReported>");
                                    found = true;
                                    break;
                                }
                            }

                            if (found == false)
                            {
                                //outputFile.WriteLine("     <LastReported>635190000000000000</LastReported>");

                                // Datetime to ticks
                                long ticks = DateTime.Today.AddDays(-10).Ticks;

                                outputFile.WriteLine("    <LastReported>" + ticks + "</LastReported>");
                            }

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

                            bool found = false;

                            foreach (XmlNode tnode in doc.DocumentElement)
                            {
                                string chkvar = tnode.Attributes[0].Value.ToString();

                                if (District == chkvar)
                                {
                                    //MessageBox.Show("var: " + District);
                                    outputFile.WriteLine("    <LastReported>" + tnode.InnerText.ToString() + "</LastReported>");
                                    found = true;
                                    break;
                                }
                            }

                            if (found == false)
                            {
                                //outputFile.WriteLine("     <LastReported>635190000000000000</LastReported>");
                                // Datetime to ticks
                                long ticks = DateTime.Today.AddDays(-10).Ticks;

                                outputFile.WriteLine("    <LastReported>" + ticks + "</LastReported>");
                            }

                            outputFile.WriteLine("   </District>");
                            cnt++;

                        }
                    }
                }

                outputFile.WriteLine("</Districts>");
                outputFile.Close(); ;
                //doc.Save(appDir + "\\Projects\\Menafrinet\\SiteMergeStatus.xml");


            }

        }
    }
}
