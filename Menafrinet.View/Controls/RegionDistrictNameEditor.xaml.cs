using System;
using System.Collections.Generic;
using System.Data;
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
using Epi.Data;
using Menafrinet.ViewModel;

namespace Menafrinet.View.Controls
{
    /// <summary>
    /// Interaction logic for RegionDistrictNameEditor.xaml
    /// </summary>
    public partial class RegionDistrictNameEditor : UserControl
    {
        public RegionDistrictNameEditor()
        {
            InitializeComponent();
        }

        private IDbDriver Database { get; set; }
        private DataHelper DataHelper { get; set; }

        public void Init(DataHelper dataHelper)
        {
            if (dg.ItemsSource != null)
            {
                dg.ItemsSource = null;
            }

            Database = dataHelper.Database;
            DataHelper = dataHelper;

            Query selectQuery = Database.CreateQuery("SELECT DRS, RegCode, DISTRICT, DistCode FROM [codedistrict1] ORDER BY DRS, DISTRICT");  //MLHIDE
            DataTable dt = Database.Select(selectQuery);
            dg.ItemsSource = (dt).DefaultView;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (dg.ItemsSource != null)
            {
                DataView dv = dg.ItemsSource as DataView;
                if (dv != null && dv.Table != null)
                {
                    DataTable dt = dv.Table;
                    if (dt != null && Database != null)
                    {
                        try
                        {
                            string querySyntax = "DELETE * FROM [codedistrict1]";
                            if (Database.ToString().ToLower().Contains("sql"))
                            {
                                querySyntax = "DELETE FROM [codedistrict1]";
                            }

                            Query deleteQuery = Database.CreateQuery(querySyntax);
                            Database.ExecuteNonQuery(deleteQuery);

                            DataRow[] rows = dt.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);

                            foreach (DataRow row in rows)
                            {
                                Query insertQuery = Database.CreateQuery("INSERT INTO [codedistrict1] (DRS, RegCode, DISTRICT, DistCode) VALUES (" +
                                    "@DRS, @RegCode, @DISTRICT, @DistCode)");
                                insertQuery.Parameters.Add(new QueryParameter("@DRS", DbType.String, row[0].ToString()));
                                insertQuery.Parameters.Add(new QueryParameter("@RegCode", DbType.String, row[1].ToString()));
                                insertQuery.Parameters.Add(new QueryParameter("@DISTRICT", DbType.String, row[2].ToString()));
                                insertQuery.Parameters.Add(new QueryParameter("@DistCode", DbType.String, row[3].ToString()));
                                Database.ExecuteNonQuery(insertQuery);
                            }

                            MessageBox.Show("Changes were committed to the database successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Changes were not committed to the database successfully. Error: " + ex.Message, "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                        }
                    }
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
