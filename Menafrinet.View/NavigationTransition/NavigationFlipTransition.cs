﻿using System;
using System.Collections.Generic;
using FluidKit.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menafrinet.View;
using Menafrinet.ViewModel;

namespace Menafrinet.View.NavigationTransition
{
    public class NavigationFlipTransition : FlipTransition, INavigationAware
    {
        public void GoForward()
        {
            this.Rotation = Direction.LeftToRight;
        }

        public void GoBackward()
        {
            this.Rotation = Direction.RightToLeft;
        }
    }
}
