using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubWeb.Client.Ext.Github.Model
{
    public class SearchResponse
    {
        public IList<Repo> Items { get; set; }
        public int Total_Count { get; set; }
    }
}
