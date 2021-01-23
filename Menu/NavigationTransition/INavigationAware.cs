using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menu.NavigationTransition
{
    public interface INavigationAware
    {
        // Setup the transition to go forward
        void GoForward();


        // Setup the transition to go backward
        void GoBackward();
    }
}
