using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Interface
{
    public interface IGitManagerService
    {
        public IGitUser GitUser{ get; set; }
        public IGitEpic GitEpic{ get; set; }
        public IGitIssue GitIssue { get; set; }

    }
}
