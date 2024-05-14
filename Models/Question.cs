using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Models
{
    public class Question
    {
        public int questionId{get;set;}
        public int userId{get;set;}
        public int typeId{get;set;}
        public string questionContent{get;set;}
        public string? questionPicture{get;set;}
        public int questionLevel{get;set;}
        public bool isDelete{get;set;}
    }
}