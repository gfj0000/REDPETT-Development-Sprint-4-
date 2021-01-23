using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Data;
using System.Linq;
using System.Windows;

namespace Menafrinet.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public bool CanMerge { get; set; }
        public bool CanTransmit { get; set; }

        public App()
            : base()
        {
            // Culture is force here date format is here gfj
            //string culture = Menafrinet.View.Properties.Settings.Default.Language;
            //Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            //this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(OnDispatcherUnhandledException);

            CanMerge = true;
            CanTransmit = true;
        }

        //void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        //{
        //    MessageBoxResult result = MessageBox.Show("An unhandled exception has occurred in the application (" + e.Exception.Message + "). Do you want to keep working?", "Exception", MessageBoxButton.YesNo, MessageBoxImage.Error);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //        e.Handled = true;
        //    }
        //    else
        //    {
        //        e.Handled = false;
        //        Application curApp = Application.Current;
        //        curApp.Shutdown();
        //    }
        //}

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Show the language select dialog
            System.Windows.ShutdownMode sm = this.ShutdownMode;
            this.ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;
            this.ShutdownMode = sm;

            MultiLang.SelectLanguage sl = new MultiLang.SelectLanguage();
            sl.LoadSettingsAndShow();
            
        }

    }
}
