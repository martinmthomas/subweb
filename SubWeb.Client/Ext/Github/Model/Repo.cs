using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubWeb.Client.Ext.Github.Model
{
    public class Repo
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Full_Name { get; set; }
        public int Stargazers_Count { get; set; }
    }
}
