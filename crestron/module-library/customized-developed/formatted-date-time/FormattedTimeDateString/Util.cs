using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace FormattedTimeDateString
{
    public static class Util
    {
        public static ushort ConvertToUshort(bool status)
        {
            return (ushort) (status == true ? 1 : 0);
        }
    }
}