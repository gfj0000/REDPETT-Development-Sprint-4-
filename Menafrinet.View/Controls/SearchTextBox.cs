using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Menafrinet.View.Controls
{
    public class SearchTextBox : TextBox
    {
        public SearchTextBox()
        : base()
        {
            PreviewKeyDown += new KeyEventHandler(SearchTextBox_PreviewKeyDown);
        }

        void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter)
            //{
            //    //BindingExpression be = GetBindingExpression(TextBox.TextProperty);
            //    //if (be != null)
            //    //{
            //    //    be.UpdateSource();
            //    //}
            //    ((this.DataContext) as Menafrinet.ViewModel.DataHelper).Search(this.Text);
            //}
        }
    }
}
