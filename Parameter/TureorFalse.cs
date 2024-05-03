using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Parameter
{
    public class TureorFalse
    {
        public string questionContent {get;set;}

        public string tagContent{get;set;}

        public int questionLevel {get;set;}

        public IFormFile? photo{get;set;}

        public bool isAnswer{get;set;}

        public string parse{get;set;}
    }
}