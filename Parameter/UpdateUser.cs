using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Parameter
{
    public class UpdateUser
    {
        public string userName { get; set; }
        public IFormFile file{ get; set; }
        public string userPhoto ;
        public int userId ;
    }
}