using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;
using BrainBoost_V2.Service;

namespace BrainBoost_V2.ViewModels
{
    public class AllQuestionViewModel
    {
        public string search{get;set;}
        public Forpaging forpaging {get;set;}
        public List<Question> question  {get;set;}
    }
}