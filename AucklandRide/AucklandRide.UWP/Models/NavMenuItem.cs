using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace AucklandRide.UWP.Models
{
    public class NavMenuItem
    {
        public string Label { get; set; }
        public string SymbolAsString
        {
            get
            {
                return System.Net.WebUtility.HtmlDecode(string.Format("{0}{1};", "&#xE", SymbolId));
            }
        }
        public string SymbolId { get; set; }
        public string Path { get; set; }
        public Type DestPage { get; set; }
        public object Arguments { get; set; }
    }
}
