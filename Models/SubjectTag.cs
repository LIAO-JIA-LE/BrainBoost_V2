using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Models
{
    public class SubjectTag
    {
        public int subjectTagId{get;set;}
        public int subjectId{get;set;}
        public int tagId{get;set;}
    }
}