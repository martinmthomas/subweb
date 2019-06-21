using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubWeb.Client.Model
{
    public class NavItem
    {
        public NavItem()
        {

        }
        public NavItem(string title, string uri, NavType navType, bool isDefault)
        {
            Title = title;
            Uri = uri;
            Type = navType;
            IsDefault = isDefault;
        }

        public string Title { get; set; }
        public string Uri { get; set; }
        public NavType Type { get; set; }
        public bool IsDefault { get; set; }
    }
}
