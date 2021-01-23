using System;
using System.Collections.Generic;
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

namespace Menafrinet.View.Controls
{
    /// <summary>
    /// Interaction logic for CaseActionsRowControl.xaml
    /// </summary>
    public partial class CaseActionsRowControl : UserControl
    {
        public event EventHandler DeleteCaseRequested;

        public CaseActionsRowControl()
        {
            InitializeComponent();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteCaseRequested != null)
            {
                DeleteCaseRequested(this, new RoutedEventArgs());
            }
            else
            {
                MessageBox.Show("That action is not allowed from here.");  //MLHIDE
            }
        }
    }
}
