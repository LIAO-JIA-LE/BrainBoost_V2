using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Models
{
    public class Response
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public object data {get;set;}
    }
}