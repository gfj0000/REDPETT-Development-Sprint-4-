using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Menafrinet.Core;

namespace Menafrinet.ViewModel
{
    /// <summary>
    /// Class representing the view model for a case.
    /// </summary>
    public class CaseViewModel : ObservableObject
    {
        #region Members
        /// <summary>
        /// Represents the case model
        /// </summary>
        private Case mCase;

        /// <summary>
        /// Whether to flag this case as a duplicate of another case (in this case by EPID)
        /// </summary>
        private bool _flagAsDuplicate = false;

        /// <summary>
        /// Whether to flag this case as having an Attachment
        /// </summary>
        private bool _flagAsAttachment = false;

        /// <summary>
        /// Whether to flag this case as a reviewed case
        /// </summary>
        private bool _flagAsReviewed;

        /// <summary>
        /// Represents whether the EPID for this case is invalid
        /// </summary>
        private bool _flagEPIDAsInvalid = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets/sets whether to flag this case as being a duplicate (the detection of duplicates should occur based on EPID values)
        /// </summary>
        public bool FlagAsDuplicate
        {
            get { return _flagAsDuplicate; }
            set
            {
                if (_flagAsDuplicate != value)
                {
                    _flagAsDuplicate = value;
                    RaisePropertyChanged("FlagAsDuplicate");
                }
            }
        }
        public bool FlagAsAttachment
        {
            get { return _flagAsAttachment; }
            set
            {
                if (_flagAsAttachment != value)
                {
                    _flagAsAttachment = value;
                    RaisePropertyChanged("FlagAsAttachment");
                }
            }
        }

        public bool FlagAsReviewed
        {
            get { return _flagAsReviewed; }
            set
            {
                _flagAsReviewed = value;
                RaisePropertyChanged("FlagAsReviewed");
            }
        }


        /// <summary>
        /// Gets/sets whether to flag this case's EPID as being invalid
        /// </summary>
        public bool FlagEPIDAsInvalid
        {
            get { return _flagEPIDAsInvalid; }
            set
            {
                if (_flagEPIDAsInvalid != value)
                {
                    _flagEPIDAsInvalid = value;
                    RaisePropertyChanged("FlagEPIDAsInvalid");
                }
            }
        }

        /// <summary>
        /// The Epi Info-generated record status for this record
        /// </summary>
        public byte RecStatus
        {
            get { return Case.RecStatus; }
            set
            {
                if (Case.RecStatus != value)
                {
                    Case.RecStatus = value;
                    RaisePropertyChanged("RecStatus");
                }
            }
        }

        /// <summary>
        /// The Epi Info-generated GUID for this record
        /// </summary>
        public string Guid
        {
            get { return Case.Guid; }
            set
            {
                if (Case.Guid != value)
                {
                    Case.Guid = value;
                    RaisePropertyChanged("Guid");
                }
            }
        }
        /// <summary>
        /// The Epi Info unique key for this record
        /// </summary>
        public int UniqueKey
        {
            get { return Case.UniqueKey; }
            set
            {
                if (Case.UniqueKey != value)
                {
                    Case.UniqueKey = value;
                    RaisePropertyChanged("UniqueKey");
                }
            }
        }

        public Case Case
        {
            get
            {
                return this.mCase;
            }
            set
            {
                this.mCase = value;

                #region EPID validation
                this.FlagEPIDAsInvalid = false;

                // gfj
                // Check #1 - Is it empty?
                //if (String.IsNullOrEmpty(Case.EPID.Trim()))
                //{
                //    this.FlagEPIDAsInvalid = true;
                //}
                //// Check #2 - Is it the correct length?
                //if (Case.EPID.Length != 23)
                //{
                //    this.FlagEPIDAsInvalid = true;
                //}
                //// Check #3 - Are all dash positions correct?
                //else if (Case.EPID[3] != '-' || Case.EPID[7] != '-' || Case.EPID[11] != '-' || Case.EPID[14] != '-' || Case.EPID[18] != '-')
                //{
                //    this.FlagEPIDAsInvalid = true;
                //}

                #endregion // EPID validation
            }
        }

        /// <summary>
        /// The EPID for this record
        /// </summary>
        public string EPID
        {
            get { return Case.EPID; }
            set
            {
                if (Case.EPID != value)
                {
                    Case.EPID = value;
                    RaisePropertyChanged("EPID");

                    #region EPID validation
                    this.FlagEPIDAsInvalid = false;

                    // Check #1 - Is it empty?
                    if (String.IsNullOrEmpty(Case.EPID.Trim()))
                    {
                        this.FlagEPIDAsInvalid = true;
                    }
                    // Check #2 - Is it the correct length?
                    if (Case.EPID.Length != 23)
                    {
                        this.FlagEPIDAsInvalid = true;
                    }
                    // Check #3 - Are all dash positions correct?
                    else if (Case.EPID[3] != '-' || Case.EPID[7] != '-' || Case.EPID[11] != '-' || Case.EPID[14] != '-' || Case.EPID[18] != '-')
                    {
                        this.FlagEPIDAsInvalid = true;
                    }

                    #endregion // EPID validation
                }
            }
        }
        /// <summary>
        /// The case's first name
        /// </summary>
        public string Type
        {
            get { return Case.Type; }
            set
            {
                if (Case.Type != value)
                {
                    Case.Type = value;
                    RaisePropertyChanged("Type");
                }
            }
        }

        public DateTime? DateAssigned
        {
            get { return Case.DateAssigned; }
            set
            {
                if (Case.DateAssigned != value)
                {
                    Case.DateAssigned = value;
                    RaisePropertyChanged("DateAssigned");
                }
            }
        }

        public string ProjectClassification
        {
            get { return Case.ProjectClassification; }
            set
            {
                if (Case.ProjectClassification != value)
                {
                    Case.ProjectClassification = value;
                    RaisePropertyChanged("ProjectClassification");
                }
            }
        }
        public string projecttitle
        {
            get { return Case.projecttitle; }
            set
            {
                if (Case.projecttitle != value)
                {
                    Case.projecttitle = value;
                    RaisePropertyChanged("projecttitle");
                }
            }
        }
        //public bool StatusCheckbox
        //{
        //    get { return Case.StatusCheckbox; }
        //    set
        //    {
        //        if (Case.StatusCheckbox != value)
        //        {
        //            Case.StatusCheckbox = value;
        //            RaisePropertyChanged("StatusCheckbox");
        //        }
        //    }
        //}

        public string LeadResident
        {
            get { return Case.LeadResident; }
            set
            {
                if (Case.LeadResident != value)
                {
                    Case.LeadResident = value;
                    RaisePropertyChanged("LeadResident");
                }
            }
        }

        public string City
        {
            get { return Case.City; }
            set
            {
                if (Case.City != value)
                {
                    Case.City = value;
                    RaisePropertyChanged("City");
                }
            }
        }

        public string DelRecDataEntry
        {
            get { return Case.DelRecDataEntry; }
            set
            {
                if (Case.DelRecDataEntry != value)
                {
                    Case.DelRecDataEntry = value;
                    RaisePropertyChanged("DelRecDataEntry");
                }
            }
        }

        public string Reviewed
        {
            get { return Case.Reviewed; }
            set
            {
                if (Case.Reviewed != value)
                {
                    Case.Reviewed = value;
                    RaisePropertyChanged("Reviewed");
                }
            }
        }

        public string ICD10CodeSuspected
        {
            get { return Case.ICD10CodeSuspected; }
            set
            {
                if (Case.ICD10CodeSuspected != value)
                {
                    Case.ICD10CodeSuspected = value;
                    RaisePropertyChanged("ICD10CodeSuspected");
                }
            }
        }


        public string leadcohort
        {
            get { return Case.leadcohort; }
            set
            {
                if (Case.leadcohort != value)
                {
                    Case.leadcohort = value;
                    RaisePropertyChanged("leadcohort");
                }
            }
        }

        public string ProjectStatus
        {
            get { return Case.ProjectStatus; }
            set
            {
                if (Case.ProjectStatus != value)
                {
                    Case.ProjectStatus = value;
                    RaisePropertyChanged("ProjectStatus"); //MLHIDE
                }
            }
        }

        public string HasAttachment
        {
            get { return Case.HasAttachment; }
            set
            {
                if (Case.HasAttachment != value)
                {
                    Case.HasAttachment = value;
                    RaisePropertyChanged("HasAttachment");
                }
            }
        }

        public string LastUpdate
        {
            get { return Case.LastUpdate; }
            set
            {
                if (Case.LastUpdate != value)
                {
                    Case.LastUpdate = value;
                    RaisePropertyChanged("LastUpdate");
                }
            }
        }

        public string Setting
        {
            get { return Case.Setting; }
            set
            {
                if (Case.Setting != value)
                {
                    Case.Setting = value;
                    RaisePropertyChanged("Setting");
                }
            }
        }

        public string GeneralDiseaseNameSuspected
        {
            get { return Case.GeneralDiseaseNameSuspected; }
            set
            {
                if (Case.GeneralDiseaseNameSuspected != value)
                {
                    Case.GeneralDiseaseNameSuspected = value;
                    RaisePropertyChanged("GeneralDiseaseNameSuspected");
                }
            }
        }


        public string UniversityTrainingInstitution
        {
            get { return Case.UniversityTrainingInstitution; }
            set
            {
                if (Case.UniversityTrainingInstitution != value)
                {
                    Case.UniversityTrainingInstitution = value;
                    RaisePropertyChanged("UniversityTrainingInstitution");
                }
            }
        }

        public string Country
        {
            get { return Case.Country; }
            set
            {
                if (Case.Country != value)
                {
                    Case.Country = value;
                    RaisePropertyChanged("Country");
                }
            }
        }


        public string DiseaseSuspected
        {
            get { return Case.DiseaseSuspected; }
            set
            {
                if (Case.DiseaseSuspected != value)
                {
                    Case.DiseaseSuspected = value;
                    RaisePropertyChanged("DiseaseSuspected");
                }
            }
        }

        public string DiseaseCategorySuspected
        {
            get { return Case.DiseaseCategorySuspected; }
            set
            {
                if (Case.DiseaseCategorySuspected != value)
                {
                    Case.DiseaseCategorySuspected = value;
                    RaisePropertyChanged("DiseaseCategorySuspected");
                }
            }
        }


        public string MentorFullName
        {
            get { return Case.MentorFullName; }
            set
            {
                if (Case.MentorFullName != value)
                {
                    Case.MentorFullName = value;
                    RaisePropertyChanged("MentorFullName");
                }
            }
        }

        public string SupervisorFullName
        {
            get { return Case.SupervisorFullName; }
            set
            {
                if (Case.SupervisorFullName != value)
                {
                    Case.SupervisorFullName = value;
                    RaisePropertyChanged("SupervisorFullName");
                }
            }
        }

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime? DateRecordCreated
        {
            get { return Case.DateRecordCreated; }
            set
            {
                if (Case.DateRecordCreated != value)
                {
                    Case.DateRecordCreated = value;
                    RaisePropertyChanged("DateRecordCreated");
                }
            }
        }

        /// <summary>
        /// Last updated date
        /// </summary>
        public DateTime? DateRecordUpdated
        {
            get { return Case.DateRecordUpdated; }
            set
            {
                if (Case.DateRecordUpdated != value)
                {
                    Case.DateRecordUpdated = value;
                    RaisePropertyChanged("DateRecordUpdated");
                }
            }
        }
        #endregion // Properties

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public CaseViewModel()
        {
            Case = new Case();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="newCase">The case model to copy from</param>
        public CaseViewModel(Case newCase)
        {
            Case = new Case();
            UpdateCase(newCase);
        }
        #endregion // Constructors

        #region Methods
        /// <summary>
        /// Updates this case view model with data from a case model
        /// </summary>
        /// <param name="updatedCase">The case model that should update this case view model</param>
        public void UpdateCase(Case updatedCase)
        {
            this.Guid = updatedCase.Guid;
            this.UniqueKey = updatedCase.UniqueKey;
            this.EPID = updatedCase.EPID;
            this.Type = updatedCase.Type;
            this.RecStatus = updatedCase.RecStatus;
            this.ProjectClassification = updatedCase.ProjectClassification;
            this.projecttitle = updatedCase.projecttitle;
            //this.StatusCheckbox = updatedCase.StatusCheckbox;
            this.LeadResident = updatedCase.LeadResident;
            this.City = updatedCase.City;
            this.Reviewed = updatedCase.Reviewed;
            this.DelRecDataEntry = updatedCase.DelRecDataEntry;
            this.ICD10CodeSuspected = updatedCase.ICD10CodeSuspected;
            this.leadcohort = updatedCase.leadcohort;
            this.HasAttachment = updatedCase.HasAttachment;
            this.ProjectStatus = updatedCase.ProjectStatus;
            this.LastUpdate = updatedCase.LastUpdate;
            this.Setting = updatedCase.Setting;
            this.DiseaseSuspected = updatedCase.DiseaseSuspected;
            this.UniversityTrainingInstitution = updatedCase.UniversityTrainingInstitution;
            this.GeneralDiseaseNameSuspected = updatedCase.GeneralDiseaseNameSuspected;
            //this.City = updatedCase.City;
            this.DelRecDataEntry = updatedCase.DelRecDataEntry;
            this.Country = updatedCase.Country;
            this.MentorFullName = updatedCase.MentorFullName;
            this.SupervisorFullName = updatedCase.SupervisorFullName;
            this.DateRecordCreated = updatedCase.DateRecordCreated;
            this.DateRecordUpdated = updatedCase.DateRecordUpdated;
            bool reviewFlag;
            bool.TryParse(updatedCase.Reviewed, out reviewFlag);
            this.FlagAsReviewed = reviewFlag;
        }
        #endregion // Methods

        #region Commands
        void UpdateExecute(Case updatedCase)
        {
            UpdateCase(updatedCase);
        }

        public ICommand Update { get { return new RelayCommand<Case>(UpdateExecute); } }

        //public static explicit operator CaseViewModel(bool v)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

    }
}
