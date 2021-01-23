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


namespace Menafrinet.View
{
    /// <summary>
    /// Interaction logic for PhotoIntro.xaml  This page is for the intro photo collage to the MenAfriNet application.
    /// </summary>
    public partial class PhotoIntro : Window
    {
        public PhotoIntro()
        {
            InitializeComponent();
        }

        private void c_folderDrop_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.None;
            else
                e.Effects = e.AllowedEffects;
            e.Handled = true;
        }

        private void c_folderDrop_Drop(object sender, DragEventArgs e)
        {
            var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            var folder = paths.Where((p) => System.IO.Directory.Exists(p)).FirstOrDefault();
            if (folder == null)
                return;
            c_pictureFrame.ImageFolder = folder;

            c_folderDrop.Background = Brushes.BlanchedAlmond;
        }

        private void c_folderDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }
            e.Effects = DragDropEffects.Copy;
            c_folderDrop.Background = Brushes.Beige;
            e.Handled = true;
        }

        private void c_folderDrop_DragLeave(object sender, DragEventArgs e)
        {
            c_folderDrop.Background = Brushes.BlanchedAlmond;
        }

        private void c_interval_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (c_pictureFrame == null)
                // This will be called before the pictureframe has loaded
                return;
            int seconds;
            if (Int32.TryParse(c_interval.Text, out seconds))
                c_pictureFrame.Interval = TimeSpan.FromSeconds(seconds);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }
    }
}
