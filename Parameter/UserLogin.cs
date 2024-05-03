using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Parameter
{
    public class UserLogin
    {
        
        // 帳號
        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        public string userAccount { get; set; }

        // 密碼
        [DisplayName("密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        public string userPassword { get; set; }
    }
}