using System.Net.Mail;

namespace BrainBoost_V2.Service
{
    public class MailService
    {
        readonly string g_Account = "gys52310@gmail.com";
        readonly string g_Password = "atkgycxstctufqtx";
        readonly string g_Email = "gys52310@gmail.com";

        // 製造驗證碼
        public string GenerateAuthCode()
        {
            string[] Code = {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","S","T","U","V","X","Y","Z",
                            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","s","t","u","v","x","y","z",
                            "1","2","3","4","5","6","7","8","9","0"};
            Random rd = new();
            string AuthCode = string.Empty;
            for (int i = 0; i < 10; i++)
            {
                AuthCode += Code[rd.Next(Code.Length)];
            }
            return AuthCode;
        }
        
        // 編輯郵件
        public string GetMailBody(string TempMail, string UserName, string ValidateUrl)
        {
            TempMail = TempMail.Replace("{{UserName}}", UserName);
            TempMail = TempMail.Replace("{{ValidateUrl}}", ValidateUrl);
            return TempMail;
        }
        
        // 寄註冊郵件
        public void SendMail(string MailBody, string ToMail)
        {
            SmtpClient smtp = new("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential(g_Account, g_Password),
                EnableSsl = true
            };
            MailMessage mail = new(g_Email, ToMail, "BrainBoost註冊會員驗證信", MailBody)
            {
                IsBodyHtml = true
            };
            smtp.Send(mail);
        }
        // 寄註冊郵件
        public void SendForgetMail(string MailBody, string ToMail)
        {
            SmtpClient smtp = new("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential(g_Account, g_Password),
                EnableSsl = true
            };
            MailMessage mail = new(g_Email, ToMail, "BrainBoost忘記密碼驗證信", MailBody)
            {
                IsBodyHtml = true
            };
            smtp.Send(mail);
        }
    }
}
