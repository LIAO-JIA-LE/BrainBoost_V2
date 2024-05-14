using System.Configuration;
using System.Data.SqlClient;
using BrainBoost.Parameter;
using BrainBoost_V2.Models;
using BrainBoost_V2.ViewModels;
using Dapper;

namespace BrainBoost_V2.Service
{
    public class SubjectService(IConfiguration configuration)
    {
        // 連線字串
        private string? cnstr = configuration.GetConnectionString("ConnectionStrings");

        #region 科目
        // 查詢科目
        public List<Subject> GetAllSubject(int userId){
            string sql = $@"SELECT * FROM ""Subject"" WHERE userId = @userId";
            using var conn = new SqlConnection(cnstr);
            return new List<Subject>(conn.Query<Subject>(sql,new {userId}));
        }
        //新增科目
        public Subject InsertSubject(InsertSubject insertData){
            string sql = $@"DECLARE @SubjectID INT
                            INSERT INTO ""Subject""(userId,subjectContent)
                            VALUES(@userId,@subjectContent)
                            SET @SubjectID = SCOPE_IDENTITY();"; //自動擷取剛剛新增資料的id
            sql += $@"SELECT * FROM ""Subject""
                      WHERE subjectId = @SubjectID
                    ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirst<Subject>(sql,new {userId = insertData.teacherId, insertData.subjectContent});
        }

        //查詢科目詳細資料
        public SubjectViewModel GetSubject(int teacher_id,int subjectId){
            SubjectViewModel Data = new();
            string subject_sql = $@"
                                    SELECT 
                                        s.subjectId,
                                        s.userId,
                                        s.subjectContent
                                    FROM ""Subject"" s
                                    WHERE s.userId = @teacher_id AND s.subjectId = @subjectId
                                ";
            string teacher_sql = $@"
                                    SELECT
                                        u.userId,
                                        u.userPhoto,
                                        u.userAccount,
                                        u.userName,
                                        u.userEmail
                                    FROM ""User"" u
                                    WHERE u.userId = @teacher_id
                                ";
            using var conn = new SqlConnection(cnstr);
            Data.subject = conn.QueryFirstOrDefault<Subject>(subject_sql,new{teacher_id,subjectId});
            Data.teacher = conn.QueryFirstOrDefault<User>(teacher_sql,new{teacher_id});
            // Data.student_List = new List<User>(conn.Query<User>(student_sql,new{teacher_id,subjectId}));
            return Data;
        }
    
        //軟刪除科目
        public void DeleteSubject(int teacher_id,int subjectId){
            string sql = $@"UPDATE ""Subject""
                            SET is_delete = 1
                            WHERE subjectId = @subjectId AND teacher_id = @teacher_id";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new{ teacher_id, subjectId});
        }
        //更新科目資訊
        public void UpdateSubject(Subject subject){
            string sql = $@"UPDATE ""Subject""
                            SET subject_name = @subject_name
                            WHERE subject_id = @subject_id AND member_id = @member_id
                        ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,subject);
        }
        #endregion
    }
}
