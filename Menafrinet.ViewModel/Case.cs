using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menafrinet.ViewModel
{
    /// <summary>
    /// A class representing a case (person suspected of being sick with Meningitis)
    /// </summary>
    public class Case
    {
        private string _EPID = String.Empty;

        public string Guid { get; set; }
        public int UniqueKey { get; set; }
        public string EPID
        {
            get
            {
                return this._EPID;
            }
            set
            {
                this._EPID = value;

                // TODO: Move validation checks to here from VM?
            }
        }
        public string Type { get; set; }
        public string ProjectClassification { get; set; }
        public DateTime? DateAssigned { get; set; }
        public string projecttitle { get; set; }
        public string StatusCheckbox { get; set; }
        public string LeadResident { get; set; }
        public string Reviewed { get; set; }
        public string leadcohort { get; set; }
        public string HasAttachment { get; set; }
        public string ProjectStatus { get; set; }
        public string LastUpdate { get; set; }
        public string City { get; set; }
        public string DelRecDataEntry { get; set; }
        public string Setting { get; set; }
        public string ICD10CodeSuspected { get; set; }
        public string GeneralDiseaseNameSuspected { get; set; }
        public string DiseaseCategorySuspected { get; set; }
        public string Country { get; set; }
        public string DiseaseSuspected { get; set; }
        public string UniversityTrainingInstitution { get; set; }
        public string MentorFullName { get; set; }
        public string SupervisorFullName { get; set; }
        public byte RecStatus { get; set; }
        public DateTime? DateRecordCreated { get; set; }
        public DateTime? DateRecordUpdated { get; set; }
    }
}
