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
        public void UpdateMemberRole(int member_id,int role){
            string sql = $@"UPDATE Member_Role SET role_id = {role} WHERE member_id = {member_id}";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }
        public void SetMemberRole_ForgetPassword(int member_id){
            string sql = $@"UPDATE Member_Role SET role_id += 4 WHERE member_id = {member_id}";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }
    }
}