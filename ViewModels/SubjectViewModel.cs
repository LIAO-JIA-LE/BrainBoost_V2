using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;

namespace BrainBoost_V2.ViewModels
{
    public class SubjectViewModel
    {
        public Subject subject {get;set;} = new();
        public User teacher {get;set;} = new();
    }
}