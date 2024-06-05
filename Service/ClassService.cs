using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;
using BrainBoost_V2.Parameter;
using Dapper;

namespace BrainBoost_V2.Service
{
    public class ClassService(IConfiguration _configuration)
    {
        private readonly string? cnstr = _configuration.GetConnectionString("ConnectionStrings");
        //新增班級
        //防呆student_id/課程名稱不重複
        public int InsertClass(InsertClass Data){
            string sql = $@"
                            DECLARE @ClassID INT
                            INSERT INTO ""Class""(className,userId)
                            VALUES(@className,@userId)
                            SET @ClassID = SCOPE_IDENTITY()
                        ";
            sql += @"SELECT @ClassID";
            using var conn = new SqlConnection(cnstr);
            int class_id = conn.QueryFirstOrDefault<int>(sql,new{Data.className, Data.userId});
            return class_id;
        }
        //檢查班級資訊
        public bool CheckClass(InsertClass data){
            string sql = $@"SELECT COUNT(*) FROM ""Class"" WHERE className = @className AND userId = @userId AND isDelete = 0 ";
            using var conn = new SqlConnection(cnstr);
            if(conn.QueryFirst<int>(sql,new{data.className,data.userId}) >= 1)
                return true;
            return false;
        }
        //檢查班級(classId)
        public bool CheckClassById(int classId,int userId){
            string sql = $@"SELECT COUNT(*) FROM ""Class"" WHERE classid = @classid AND userId = @userId AND isDelete = 0";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<int>(sql,new{classId,userId}) == 1;
        }
        //取得單一班級資訊
        public Class GetClass(int classId,int userId){
            string sql = $@"SELECT * FROM ""Class"" WHERE classId = @classId AND userId = @userId AND isDelete = 0";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<Class>(sql,new{classId,userId});
        }
        //取得全部班級
        public List<Class> GetAllClass(int userId,string search){
            string sql;
            if(string.IsNullOrEmpty(search)){
                sql = $@"SELECT * FROM ""Class"" WHERE userId = @userId AND isDelete = 0";
            }
            else{
                sql = $@"SELECT * FROM ""Class"" WHERE userId = @userId AND isDelete = 0 AND className LIKE '%{search}%'";
            }
            using var conn = new SqlConnection(cnstr);
            return new List<Class>(conn.Query<Class>(sql,new{userId}));
        }
        //更新班級資訊
        public void UpdateClass(UpdateClass updateData){
            string sql = $@"UPDATE ""Class"" SET className = @className, userId = @userId
                            WHERE classId = @classId";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new{updateData.className,updateData.userId,updateData.classId});
        }
        //刪除班級(軟刪除)
        public void DeleteClass(DeleteClass deleteData){
            string sql = $@"UPDATE ""Class"" SET isDelete = 1
                            WHERE classId = @classId
                            ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new{deleteData.classId});
        }
    }
}