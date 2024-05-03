using System.ComponentModel;

namespace BrainBoost_V2.Models{
    public class User{
        //編號
        public int userId {get;set;}
        //名稱
        public string userName {get;set;}
        //照片
        public string userPhoto {get;set;}
        //帳號
        public string userAccount {get;set;}
        //密碼
        public string userPassword {get;set;}
        //電子郵件
        public string userEmail {get;set;}
        //驗證碼
        public string? userAuthCode {get;set;}
        public int roleId{get;set;}
    }
}