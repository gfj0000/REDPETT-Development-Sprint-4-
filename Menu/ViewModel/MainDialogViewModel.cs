using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Menu.ViewModel
{
    public class MainDialogViewModel : ViewModelBase
    {
        //private readonly ObservableCollection<PageViewModelBase> views;
        //private readonly ICollectionView collectionview;
        //private bool goForward;
        //private PageViewModelBase currentContent;

        //public ObservableCollection<PageViewModelBase> Views
        //{
        //    get { return this.views; }
        //}

        //public bool GoForward
        //{
        //    get
        //    {
        //        return this.goForward;
        //    }
        //    set
        //    {
        //        this.goForward = value;
        //        OnPropertyChanged("GoForward");
        //    }
        //}

        //public PageViewModelBase CurrentContent
        //{
        //    get
        //    {
        //        return this.currentContent;
        //    }
        //    private set
        //    {
        //        this.currentContent = value;
        //        OnPropertyChanged("CurrentContent");
        //    }
        //}

        public MainDialogViewModel()
        {
            //this.views = new ObservableCollection<PageViewModelBase>();
            //this.collectionview = CollectionViewSource.GetDefaultView(this.views);
            //this.collectionview.CurrentChanged += new EventHandler(this.OnCurrentChanged);

            //this.views.Add(new Page1ViewModel());
            //this.views.Add(new Page2ViewModel());
            //this.views.Add(new Page3ViewModelcs());
            //this.views.Add(new Page4ViewModel());

            //this.currentContent = this.views[0];
        }

        private void OnCurrentChanged(object sender, EventArgs e)
        {
            // check the orientation of the change made by the user
            // to update the GoForward property
            //if (this.collectionview.CurrentPosition < this.views.IndexOf(this.currentContent))
            //{
            //    this.GoForward = true;
            //}
            //else
            //{
            //    this.GoForward = false;
            //}

            //this.CurrentContent = this.collectionview.CurrentItem as PageViewModelBase;
        }
    }
}
