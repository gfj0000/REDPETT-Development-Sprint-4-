using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menafrinet.View;
using Menafrinet.ViewModel;
using Menafrinet;



namespace Menafrinet.View.NavigationTransition
{
    public interface INavigationAware
    {
        // Setup the transition to go forward
        void GoForward();


        // Setup the transition to go backward
        void GoBackward();
    }
}
