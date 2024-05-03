using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using BrainBoost_V2.Models;
using BrainBoost_V2.Parameter;
using BrainBoost_V2.Services;
using Dapper;

namespace BrainBoost_V2.Service
{
    public class UserService(IConfiguration configuration)
    {
        #region 宣告連線字串
        private readonly string? cnstr = configuration.GetConnectionString("ConnectionStrings");
        #endregion

        #region 登入
        // 獲得權限
        public int GetRole(string userAccount)
        {
            string sql = $@"SELECT u_r.roleId FROM ""User"" u INNER JOIN UserRole u_r ON u.userId = u_r.userId WHERE u.userAccount = @userAccount";
            using var conn = new SqlConnection(cnstr);
            int role_id = conn.QueryFirstOrDefault<int>(sql,new{userAccount});
            return role_id;
        }

        // 登入確認
        public string LoginCheck(string userAccount, string userPassword)
        {
            User Data = GetDataByAccount(userAccount);
            if (Data != null)
            {
                if (String.IsNullOrWhiteSpace(Data.userAuthCode))
                {
                    if (PasswordCheck(Data.userPassword, userPassword))
                        return "";
                    else
                        return "密碼錯誤";
                }
                else
                    return "此帳號尚未經過Email驗證";
            }
            else
                return "無此會員資料";
        }
        #endregion
        
        // 註冊
        // 密碼確認
        public bool PasswordCheck(string Data, string Password)
        {
            return Data.Equals(HashPassword(Password));
        }

        // 確認註冊
        public string RegisterCheck(string Account,string Email){
            if(GetDataByAccount(Account)!=null)
                return "帳號已被註冊";
            else if(GetDataByEmail(Email)!=null)
                return "電子郵件已被註冊";
            return "";
        }

        // 雜湊密碼
        public string HashPassword(string Password)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                string salt = "foiw03pltmvle6";
                string saltandpas = string.Concat(salt, Password);
                byte[] data = Encoding.UTF8.GetBytes(saltandpas);
                byte[] hash = sha512.ComputeHash(data);
                string result = Convert.ToBase64String(hash);
                return result;
            }
        }

        public void Register(User user)
        {
            string sql = @$"INSERT INTO ""User""(userName,userPhoto,userAccount,userPassword,userEmail,userAuthCode)
                                          VALUES(@userName,@userPhoto,@userAccount,@userPassword,@userEmail,@userAuthCode)
                                        /*('user.userName','user.userPhoto','user.userAccount','user.userPassword','user.userEmail','user.userAuthCode')*/
                            /*設定暫時的變數*/
                            DECLARE @user_Id int;
                            SET @user_Id = (SELECT u.userId FROM ""User"" u WHERE u.userAccount = @userAccount);
                            INSERT INTO UserRole(userId,roleId)
                                        VALUES(@user_Id,1)";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new{ user.userName,user.userPhoto,user.userAccount,user.userPassword,user.userEmail,user.userAuthCode });
        }

        public bool MailValidate(string Account, string AuthCode)
        {
            User Data = GetDataByAccount(Account);
            //判斷有無會員資料並比對驗證碼是否正確
            if (Data != null && Data.userAuthCode == AuthCode)
            {
                string sql = $@"UPDATE ""User"" SET userAuthcode = '{string.Empty}' WHERE userAccount = '{Account}'";
                using var conn = new SqlConnection(cnstr);
                conn.Execute(sql);
                return Data.userAuthCode == AuthCode;
            }
            else return false;
        }

        //(後台管理者)
        //取得所有使用者
        public List<User> GetAllMemberList(string Search,Forpaging forpaging){
            List<User> Data = new();
            //判斷是否有增加搜尋值
            if(string.IsNullOrEmpty(Search)){
                SetMaxPage(forpaging);
                Data = GetUserList(forpaging);
            }
            else{
                SetMaxPage(Search,forpaging);
                Data = GetUserList(Search,forpaging);
            }
            return Data;
        }
        //無搜尋值查詢的使用者列表
        public List<User> GetUserList(Forpaging forpaging){
            List<User> data = [];
            string sql = $@"SELECT * FROM (
                                SELECT ROW_NUMBER() OVER(ORDER BY u.userId DESC) r_num,u.userId,u.userAccount,u.userName,u.userEmail,ur.roleId FROM ""User"" u 
                                JOIN UserRole ur
                                ON u.userId = ur.userId
                            )a
                            WHERE a.r_num BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }";
            using var conn = new SqlConnection(cnstr);
            data = new List<User>(conn.Query<User>(sql));
            return data;
        }
        //有搜尋值查詢的使用者列表
        public List<User> GetUserList(string Search,Forpaging forpaging){
            List<User> data = new();
            string sql = $@"SELECT * FROM (
                                SELECT ROW_NUMBER() OVER(ORDER BY u.userId DESC) r_num,u.userId,u.userAccount,u.userName,u.userEmail,ur.roleId FROM ""User"" u 
                                JOIN UserRole ur
                                ON u.userId = ur.userId
                                WHERE u.userAccount LIKE '%{Search}%' OR u.userName LIKE '%{Search}%'
                            )a
                            WHERE a.r_num BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }";
            using var conn = new SqlConnection(cnstr);
            data = new List<User>(conn.Query<User>(sql));
            return data;
        }
        //無搜尋值計算所有使用者並設定頁數
        public void SetMaxPage(Forpaging forpaging){
            string sql = $@"SELECT COUNT(*) FROM ""User""";
            using var conn = new SqlConnection(cnstr);
            int row = conn.QueryFirst<int>(sql);
            forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / forpaging.Item));
            forpaging.SetRightPage();
        }
        //有搜尋值計算所有使用者並設定頁數
        public void SetMaxPage(string Search,Forpaging forpaging){
            string sql = $@"SELECT COUNT(*) FROM ""User"" WHERE userAccount LIKE '%{Search}%' OR userName LIKE '%{Search}%'";
            using var conn = new SqlConnection(cnstr);
            int row = conn.QueryFirst<int>(sql);
            forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / forpaging.Item));
            forpaging.SetRightPage();
        }

        #region 忘記密碼
        // 更新驗證碼
        public void ChangeAuthCode(string NewAuthCode,string Email){
            string sql = $@" UPDATE ""User"" SET userAuthcode = '{NewAuthCode}' WHERE userEmail = '{Email}';";
            using (var conn = new SqlConnection(cnstr))
            conn.Execute(sql);
        }
        
        // 清除驗證碼
        public void ClearAuthCode(string Email){
            string sql = $@" UPDATE ""User"" SET userAuthcode = '{String.Empty}' WHERE userEmail = '{Email}';";
            using (var conn = new SqlConnection(cnstr))
            conn.Execute(sql);
        }

        // 更改密碼ByForget
        public void ChangePasswordByForget(CheckForgetPassword Data){
            User user = GetDataByEmail(Data.Email);
            user.userPassword = HashPassword(Data.NewPassword);
            string sql = $@"UPDATE ""User"" SET userPassword = '{user.userPassword}' WHERE userEmail = '{Data.Email}';
                            DECLARE @userId int;
                            SELECT @userId = userId FROM User WHERE userEmail = '{Data.Email}'
                            UPDATE UserRole SET roleId -= 4 WHERE userId = @userId";
            using (var conn = new SqlConnection(cnstr))
            conn.Execute(sql,new{user.userId});
        }
        
        // 用mail獲得資料
        public User GetDataByEmail(string mail){
            string sql = $@"SELECT * FROM ""User"" WHERE userEmail = '{mail}' ";
            using (var conn = new SqlConnection(cnstr))
            return conn.QueryFirstOrDefault<User>(sql);
        }
        // 用account獲得資料
        public User GetDataByAccount(string account){
            string sql = $@"SELECT 
                                u.*,
                                ur.roleId
                            FROM ""User"" u 
                            JOIN UserRole ur
                            ON u.userId = ur.userId
                            WHERE userAccount = '{account}'
                            ";
            using (var conn = new SqlConnection(cnstr))
            return conn.QueryFirstOrDefault<User>(sql);
        }
        #endregion

        #region 修改密碼
        public void ChangePassword(int member_id, string pwd){
            string password = HashPassword(pwd);
            string sql = $@" UPDATE User SET userPassword = '{password}' WHERE userId = {member_id} ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }
        #endregion

        #region 修改個人資訊
        //修改個人資料
        public void UpdateUserData(UserUpdate user){
            string sql = $@"UPDATE ""User"" SET 
                                userName = @userName, 
                                userPhoto = @userPhoto
                            WHERE userId = @userId";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new{user.userName,user.userPhoto,user.userId});
        }
        #endregion
    }
}