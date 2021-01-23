using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menafrinet.ViewModel
{
    public abstract class PageViewModelBase : ViewModelBase
    {
        public abstract string Name { get; }
    }
}
