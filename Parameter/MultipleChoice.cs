using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Parameter
{
    public class MultipleChoice
    {
        public string questionContent {get;set;}

        public int questionLevel {get;set;}

        // public IFormFile? photo{get;set;}

        public string optionA {get;set;}

        public string optionB {get;set;}

        public string optionC {get;set;}

        public string optionD {get;set;}

        public string tagContent{get;set;}

        public string answerContent{get;set;}

        public string? parse{get;set;}
    }
}