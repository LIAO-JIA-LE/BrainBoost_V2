using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;

namespace BrainBoost_V2.ViewModels
{
    public class GetQuestion
    {
        public Question questionData { get; set; } = new();
        public Subject subjectData{get;set;} = new();
        
        public List<string> options { get; set; }

        public List<string>? optionImg { get; set; }

        public Answer answerData { get; set; } = new();

        public Tag tagData{ get; set; } = new();
    }
}