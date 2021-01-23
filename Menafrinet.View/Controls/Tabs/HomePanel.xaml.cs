using Epi.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Menafrinet.View.Controls
{
    /// <summary>
    /// Interaction logic for HomePanel.xaml
    /// </summary>
    public partial class HomePanel : UserControl
    {
        public HomePanel()
        {
            InitializeComponent();

            //string appDir = Environment.CurrentDirectory; Uri pageUri = new Uri(appDir + "/Projects/Menafrinet/Html/Home.html");
            //browser.Source = pageUri; 
            //string appDir = Environment.CurrentDirectory; Uri pageUri = new Uri(appDir + "/Home.html");
            //browser.Source = pageUri;
        }

        private void btnJobAid1_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Properties.Settings.Default.JobAid1URL;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = filePath;
            //proc.StartInfo.Arguments = string.Format("\"{0}\"", "Projects\\ProjectFolderName\\File.extension");
            proc.StartInfo.UseShellExecute = true;
            try
            {
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnJobAid2_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Properties.Settings.Default.JobAid2URL;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = filePath;
            //proc.StartInfo.Arguments = string.Format("\"{0}\"", "Projects\\ProjectFolderName\\File.extension");
            proc.StartInfo.UseShellExecute = true;
            try
            {
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnJobAid3_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Properties.Settings.Default.JobAid2URL;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = filePath;
            //proc.StartInfo.Arguments = string.Format("\"{0}\"", "Projects\\ProjectFolderName\\File.extension");
            proc.StartInfo.UseShellExecute = true;
            try
            {
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnJobAid4_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Properties.Settings.Default.JobAid2URL;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = filePath;
            //proc.StartInfo.Arguments = string.Format("\"{0}\"", "Projects\\ProjectFolderName\\File.extension");
            proc.StartInfo.UseShellExecute = true;
            try
            {
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnVideos_Click(object sender, RoutedEventArgs e)
        {
            object chkCheck1 = EpiInfoMenuManager.GetPermanentVariableValue("StartCheckboxPerm");
            object chkCheck2 = EpiInfoMenuManager.GetPermanentVariableValue("StartCheckbox2Perm");

            if(chkCheck1.ToString() == "0" && chkCheck2.ToString() == "0")
            {
                StartHelpCheckbox.IsChecked = true;
            }
            else
            {
                StartHelpCheckbox.IsChecked = false;
            }

            //if (StartHelpCheckbox.IsChecked == true)
            //{
            //    Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable((("StartCheckboxPerm")), 1);
            //    Epi.Menu.EpiInfoMenuManager.SetPermanentVariable((("StartCheckboxPerm")), 1);
            //    Epi.Menu.EpiInfoMenuManager.SetPermanentVariable((("StartCheckbox2Perm")), 1);
            //}
            //else
            //{
            //    Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable((("StartCheckboxPerm")), 1);
            //    Epi.Menu.EpiInfoMenuManager.SetPermanentVariable((("StartCheckboxPerm")), 0);
            //    Epi.Menu.EpiInfoMenuManager.SetPermanentVariable((("StartCheckbox2Perm")), 0);
            //}

            CanvasSelection.Margin = new Thickness(-12, 32, 0, 0);
        }

        private void CanvasSelection_LostFocus(object sender, System.Windows.Input.MouseEventArgs e)
        {

            //int closeNum = -20;

            //for (int i = 1; i <= 21; i++)
            //{ // print numbers from 1 to 5
            //    if (closeNum > -190)
            //    {
            //closeNum = closeNum - 10;
            //CanvasSelection.Margin = new Thickness(closeNum, 32, 0, 0);
            CanvasSelection.Margin = new Thickness(-197, 32, 0, 0);
            //Thread.Sleep(400);
            //    }
            //}
        }

        private void CanvasSelection_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void CanvasSelection_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Thread.Sleep(5000);
            CanvasSelection.Margin = new Thickness(-197, 32, 0, 0);

            //if (!CanvasSelection_GotFocus..Location))
            //{

            //}
        }

        private void REDPETTOverviewClick(object sender, RoutedEventArgs e)
        {
           // System.Diagnostics.Process.Start("Video\\redpettoverview.mp4");
        }

        private void Report2_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("Video\\WebinarPrezi_GeneralPresentation_mp4.mp4");
        }

        private void Report3_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("Video\\Workflow_Detailed.mp4");
        }

        private void StartHelpCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (StartHelpCheckbox.IsChecked == true)
            {
                Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable((("StartCheckboxPerm")), 1);
                Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable((("StartCheckbox2Perm")), 1);
                Epi.Menu.EpiInfoMenuManager.SetPermanentVariable((("StartCheckboxPerm")), 0);
                Epi.Menu.EpiInfoMenuManager.SetPermanentVariable((("StartCheckbox2Perm")), 0);
            }
            else
            {
                Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable((("StartCheckboxPerm")), 1);
                Epi.Menu.EpiInfoMenuManager.CreatePermanentVariable((("StartCheckbox2Perm")), 1);
                Epi.Menu.EpiInfoMenuManager.SetPermanentVariable((("StartCheckboxPerm")), 1);
                Epi.Menu.EpiInfoMenuManager.SetPermanentVariable((("StartCheckbox2Perm")), 1);
            }
        }
    }
}
