using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;

namespace BrainBoost.ViewModels
{
    public class GetQuestionViewModel
    {
        public Question question{get;set;}
        public List<Option> options{get;set;}
    }
}