using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Interface
{
    public interface IGitManagerService
    {

        public IGitIssue Issue { get; }
        public IGitEpic Epic { get; }
        public IGitUser User{ get; }
    }
}
