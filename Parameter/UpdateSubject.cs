using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Parameter
{
    public class UpdateSubject
    {
        public string subjectContent { get; set; }
        public int subjectId { get; set; }
        public int userId;
    }
}