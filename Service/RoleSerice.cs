using BrainBoost_V2.ViewModels;
using BrainBoost_V2.Models;
using System.Data.SqlClient;
using Dapper;

namespace BrainBoost_V2.Service
{
    public class RoleService
    {
        #region 宣告連線字串
        private readonly string? cnstr;
        public RoleService(IConfiguration configuration){
            cnstr = configuration.GetConnectionString("ConnectionStrings");
        }
        #endregion

        //獲取使用者角色名單
        public List<UserRoleList> GetMemberRoleList(){
            List<UserRoleList> data = new();
            string sql = $@"";
            using var conn = new SqlConnection(cnstr);
            return data;
        }
        //修改使用者權限(帳號)
        public void UpdateMemberRole(int userId,int role){
            string sql = $@"UPDATE UserRole SET roleId = {role} WHERE userId = {userId}";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }
        public void SetMemberRole_ForgetPassword(int userId){
            string sql = $@"UPDATE UserRole SET roleId = 5 WHERE userId = {userId}";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }
    }
}