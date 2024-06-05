using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.Parameter
{
    public class InsertSubject
    {
        public int subjectId;
        //科目名稱
        public string subjectContent{get;set;}
        //老師帳號
        public int userId{get;set;}
    }
}