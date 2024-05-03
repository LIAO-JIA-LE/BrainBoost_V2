
using System.ComponentModel;

namespace BrainBoost_V2.Parameter
{
    public class Login
    {
        [DisplayName("帳號")]
        public string account{get;set;}
        public string password{get;set;}
    }
}