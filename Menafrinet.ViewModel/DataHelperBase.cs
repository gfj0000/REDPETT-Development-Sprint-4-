using System;
using System.Data;
using System.IO;
using Epi;
using Epi.Data;
using Epi.Menu;

namespace Menafrinet.ViewModel
{
    public abstract class DataHelperBase : ObservableObject, IDataHelper
    {
        public View CaseForm { get; set; }
        public Project Project { get; set; }
        public IDbDriver Database { get; protected set; }

        public abstract void RepopulateCollections();
        protected virtual void ClearCollections() { }

        public static bool DelRecStatus;
        public static bool CurCohortStatus;


        protected virtual DataTable GetCasesTable()
        {
            // Current cohort
            var CohortYNPerm = EpiInfoMenuManager.GetPermanentVariableValue("CohortYNPerm"); //MLHIDE

            Query selectQuery;

            if (DelRecStatus == false)
            {

                if (CohortYNPerm.ToString() == "False") //MLHIDE
                {
                    selectQuery = Database.CreateQuery("SELECT * " + CaseForm.FromViewSQL + " WHERE [RECSTATUS] = 1"); //MLHIDE
                }
                else
                {  //gfj0 cohort filter needs work
                    selectQuery = Database.CreateQuery("SELECT * " + CaseForm.FromViewSQL + " WHERE [RECSTATUS] = 1 AND  [TraineeStatus] = 'Current'"); //MLHIDE
                }

                return Database.Select(selectQuery);
            }
            else
            {
                if (CohortYNPerm.ToString() == "False") //MLHIDE
                {
                    selectQuery = Database.CreateQuery("SELECT * " + CaseForm.FromViewSQL); // + " WHERE [RECSTATUS] > 0"); MLHIDE 
                }
                else
                {
                    selectQuery = Database.CreateQuery("SELECT * " + CaseForm.FromViewSQL + " WHERE [TraineeStatus] =  'Current'"); //MLHIDE
                }

                return Database.Select(selectQuery);
            }
        }

        protected bool LoadConfig()
        {
            string configFilePath = Configuration.DefaultConfigurationPath;
            bool configurationOk = true;
            try
            {
                string directoryName = System.IO.Path.GetDirectoryName(configFilePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                if (!File.Exists(configFilePath))
                {
                    Configuration defaultConfig = Configuration.CreateDefaultConfiguration();
                    Configuration.Save(defaultConfig);
                }

                Configuration.Load(configFilePath);
            }
            catch (Epi.ConfigurationException ex)
            {
            }
            catch (Exception ex)
            {
                configurationOk = ex.Message == "";
            }
            return configurationOk;
        }
    }
}
