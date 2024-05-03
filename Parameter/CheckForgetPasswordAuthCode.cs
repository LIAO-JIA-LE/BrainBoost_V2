using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Parameter
{
    public class CheckForgetPasswordAuthCode
    {
        // 信箱
        public string Email{get;set;}
        // 驗證碼
        public string AuthCode{get;set;}
    }
}