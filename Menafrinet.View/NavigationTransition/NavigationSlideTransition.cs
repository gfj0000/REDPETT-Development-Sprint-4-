using System;
using System.Collections.Generic;
using System.Linq;
using FluidKit.Controls;
using System.Text;
using System.Threading.Tasks;
using Menafrinet.View;
using Menafrinet.ViewModel;



namespace Menafrinet.View.NavigationTransition
{
    public class NavigationSlideTransition : SlideTransition, INavigationAware
    {
        public void GoForward()
        {
            this.Direction = Direction.LeftToRight;
        }

        public void GoBackward()
        {
            this.Direction = Direction.RightToLeft;
        }
    }
}
