using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubWeb.Client.Model
{
    public class GitRepo
    {
        public GitRepo(string fullName, string description, int stargazersCount)
        {
            FullName = fullName;
            Description = description;
            StargazersCount = stargazersCount;
        }

        public string FullName { get; set; }
        public string Description { get; set; }
        public int StargazersCount { get; set; }
    }
}
