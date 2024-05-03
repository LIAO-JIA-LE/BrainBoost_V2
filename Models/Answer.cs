using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Models
{
    public class Answer
    {
        public int answerId{get;set;}
        public int questionId{get;set;}
        public string answerContent{get;set;}
        public string parse{get;set;}
    }
}