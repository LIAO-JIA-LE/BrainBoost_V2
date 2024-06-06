using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;
using BrainBoost_V2.Service;

namespace BrainBoost_V2.ViewModels
{
    public class AllSubjectViewModel
    {
        public int userId {get;set;}
        public string search{get;set;}
        // public Forpaging forpaging{get;set;}
        public List<Subject> subjectList {get;set;}
    }
}