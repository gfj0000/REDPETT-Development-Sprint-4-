using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menafrinet.View.DataAccess
{
    public static class CacheHelper
    {
        public static IObservable<List<string>> Residents { get; set; }
    }
}
