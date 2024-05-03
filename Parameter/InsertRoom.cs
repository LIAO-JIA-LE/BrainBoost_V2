using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Parameter
{
    public class InsertRoom
    {
        public int userId{get;set;}
        public string roomName{get;set;}
        public int classId {get;set;}
        public bool roomFunction{get;set;}
        public bool roomPublic{get;set;}
        public int timeLimit{get;set;}
        public List<int> questionId{get;set;}
    }
}