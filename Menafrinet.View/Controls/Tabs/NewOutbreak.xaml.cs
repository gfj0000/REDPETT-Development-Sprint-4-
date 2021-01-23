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
//using ContactTracing.Core.Data;

namespace ContactTracing.Controls
{
    /// <summary>
    /// Interaction logic for NewOutbreak.xaml
    /// </summary>
    public partial class NewOutbreak : UserControl
    {
        //private ProjectInfo _projectInfo;
        
        public event RoutedEventHandler Closed;

        public NewOutbreak()
        {
            InitializeComponent();
            this.dpOutbreakDate.SelectedDate = DateTime.Today;
            this.dpOutbreakDate.DisplayDate = DateTime.Today;
        }

        public string FileName
        {
            get { return this.txtFileName.Text; }
            set
            {
                this.txtFileName.Text = value;
            }
        }

        public string OutbreakName
        {
            get { return this.txtOutbreakName.Text; }
            set
            {
                this.txtOutbreakName.Text = value;
            }
        }

        public string IDPrefix
        {
            get { return this.txtPrefix.Text; }
            set
            {
                this.txtPrefix.Text = value;
            }
        }

        public string IDSeparator
        {
            get { return this.txtSep.Text; }
            set
            {
                this.txtSep.Text = value;
            }
        }

        public DateTime? OutbreakDate
        {
            get { return this.dpOutbreakDate.SelectedDate; }
            set
            {
                this.dpOutbreakDate.SelectedDate = value;
            }
        }

        public string IDPattern
        {
            get
            {
                switch (cmbPattern.SelectedIndex)
                {
                    case 0:
                        return "##";
                    case 1:
                        return "###";
                    case 2:
                        return "####";
                    default:
                        return "###";
                }
            }
            set
            {
                cmbPattern.Text = value;
            }
        }

        public string Virus
        {
            get
            {
                switch (cmbVirus.SelectedIndex)
                {
                    case 0:
                        return "Ebola";
                    case 1:
                        return "Sudan";
                    case 2:
                        return "Marburg";
                    case 3:
                        return "Bundibugyo";
                    case 4:
                        return "Rift";
                    case 5:
                        return "Lassa";
                    case 6:
                        return "CCHF";
                    default:
                        return "Ebola";
                }
            }
            set
            {
                cmbVirus.Text = value;
            }
        }

        public string Country
        {
            get { return cmbCountry.Text; }
            set
            {
                cmbCountry.Text = value;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtFileName.Text))
            {
                MessageBox.Show("File name cannot be blank.");
                return;
            }

            if (String.IsNullOrEmpty(txtOutbreakName.Text))
            {
                MessageBox.Show("Outbreak name cannot be blank.");
                return;
            }

            if (String.IsNullOrEmpty(txtPrefix.Text))
            {
                MessageBox.Show("The ID prefix cannot be blank.");
                return;
            }

            if (String.IsNullOrEmpty(txtSep.Text))
            {
                MessageBox.Show("The ID separator cannot be blank.");
                return;
            }

            if (Closed != null)
            {
                RoutedEventArgs args = new RoutedEventArgs(NewOutbreak.OkClickEvent);
                Closed(this, args);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Closed != null)
            {
                RoutedEventArgs args = new RoutedEventArgs(NewOutbreak.CloseClickEvent);
                Closed(this, args);
            }
        }

        //public static readonly RoutedEvent OkClickEvent = EventManager.RegisterRoutedEvent(
        //"OK", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExistingCase));

        public static readonly RoutedEvent CloseClickEvent = EventManager.RegisterRoutedEvent(
        "Close", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewOutbreak));

        public static readonly RoutedEvent OkClickEvent = EventManager.RegisterRoutedEvent(
        "OK", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewOutbreak));

        // Provide CLR accessors for the event 
        //public event RoutedEventHandler Ok
        //{
        //    add { AddHandler(OkClickEvent, value); }
        //    remove { RemoveHandler(OkClickEvent, value); }
        //}

        public event RoutedEventHandler Close
        {
            add { AddHandler(CloseClickEvent, value); }
            remove { RemoveHandler(CloseClickEvent, value); }
        }

        private void txtFileName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(e.Text, "[A-Za-z0-9_]");
            if (!m.Success)
            {
                e.Handled = true;
            }
        }
    }
}
