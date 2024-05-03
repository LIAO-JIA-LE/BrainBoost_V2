using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Models
{
    public class Option
    {
        public int optionId{get;set;}
        public int questionId{get;set;}
        public string optionContent{get;set;}
        public string optionPicture{get;set;}
        public bool isAnswer{get;set;}
        public bool isDelete{get;set;}
    }
}