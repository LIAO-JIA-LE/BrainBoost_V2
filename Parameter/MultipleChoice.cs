using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Parameter
{
    public class MultipleChoice
    {
        public int subjectId { get; set; }
        public int questionLevel {get;set;}
        public string tagContent{get;set;}

        // 題目
        public string questionContent {get;set;}
        public IFormFile? questionFile{ get; set; }

        // 選項
        public List<string> options {get;set;}
        public List<IFormFile>? optionsFile{ get; set; }

        // 答案
        public string answerContent{get;set;}

        public string? parse{get;set;}
    }
}