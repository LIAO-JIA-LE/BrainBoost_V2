using System.Configuration;
using System.Data.SqlClient;
using BrainBoost.Parameter;
using BrainBoost_V2.Models;
using BrainBoost_V2.Service;
using BrainBoost_V2.ViewModels;
using Dapper;

namespace BrainBoost_V2.Service
{
    public class SubjectService(IConfiguration configuration)
    {
        // 連線字串
        private string? cnstr = configuration.GetConnectionString("ConnectionStrings");

        #region 科目
        // 有分頁
        // public List<Subject> GetAllSubject(AllSubjectViewModel AllSubjectViewModel){
        //     List<Subject> subjectList;
        //     if(string.IsNullOrEmpty(AllSubjectViewModel.search)){
        //         SetMaxPage(AllSubjectViewModel.userId,AllSubjectViewModel.forpaging);
        //         subjectList = SubjectList(AllSubjectViewModel.userId,AllSubjectViewModel.forpaging);
        //     }
        //     else{
        //         SetMaxPage(AllSubjectViewModel.userId,AllSubjectViewModel.search,AllSubjectViewModel.forpaging);
        //         subjectList = SubjectList(AllSubjectViewModel.userId, AllSubjectViewModel.search,AllSubjectViewModel.forpaging);
        //     }
        //     return subjectList;
        // }

        // 無分頁
        public List<Subject> GetAllSubject(AllSubjectViewModel AllSubjectViewModel){
            List<Subject> subjectList;
            if(string.IsNullOrEmpty(AllSubjectViewModel.search)){
                subjectList = SubjectList(AllSubjectViewModel.userId);
            }
            else{
                subjectList = SubjectList(AllSubjectViewModel.userId, AllSubjectViewModel.search);
            }
            return subjectList;
        }
        #region 查詢科目 (無搜詢)
        // public void SetMaxPage(int userId,Forpaging forpaging){
        //     string sql = $@"
        //                     SELECT
        //                         COUNT(*)
        //                     FROM(
        //                         SELECT
        //                             ROW_NUMBER() OVER(ORDER BY subjectId) rNum,
        //                             * 
        //                         FROM ""Subject"" s
        //                         WHERE userId = @userId AND isDelete =0
        //                     )S
        //                 ";
        //     using var conn = new SqlConnection(cnstr);
        //     int row = conn.QueryFirst<int>(sql, new{userId});
        //     forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / forpaging.Item));
        //     forpaging.SetRightPage();
        // }
        // public List<Subject> SubjectList(int userId,Forpaging forpaging){
        //     string sql = $@"
        //                     SELECT
        //                         *
        //                     FROM(
        //                         SELECT
        //                             ROW_NUMBER() OVER(ORDER BY subjectId) rNum,
        //                             * 
        //                         FROM ""Subject"" s
        //                         WHERE userId = @userId AND isDelete =0
        //                     )S
        //                     WHERE S.rNum BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }
        //                 ";
        //     using var conn = new SqlConnection(cnstr);
        //     return new List<Subject>(conn.Query<Subject>(sql,new {userId}));
        // }
        public List<Subject> SubjectList(int userId){
            string sql = $@"
                            SELECT
                                ROW_NUMBER() OVER(ORDER BY subjectId) rNum,
                                * 
                            FROM [Subject] s
                            WHERE userId = @userId AND isDelete =0
                            ORDER BY rNum DESC
                        ";
            using var conn = new SqlConnection(cnstr);
            return new List<Subject>(conn.Query<Subject>(sql,new {userId}));
        }
        #endregion
        #region 查詢科目 (有搜詢)
        // public void SetMaxPage(int userId,string search,Forpaging forpaging){
        //     string sql = $@"
        //                     SELECT
        //                         COUNT(*)
        //                     FROM(
        //                         SELECT
        //                             ROW_NUMBER() OVER(ORDER BY subjectId) rNum,
        //                             * 
        //                         FROM ""Subject"" s
        //                         WHERE userId = @userId AND isDelete =0 AND subjectContent LIKE '%{search}%'
        //                     )S
        //                     WHERE S.rNum BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }
        //                 ";
        //     using var conn = new SqlConnection(cnstr);
        //     int row = conn.QueryFirst<int>(sql, new{userId});
        //     forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / forpaging.Item));
        //     forpaging.SetRightPage();
        // }

        // public List<Subject> SubjectList(int userId,string search,Forpaging forpaging){
        //     string sql = $@"
        //                     SELECT
        //                         *
        //                     FROM(
        //                         SELECT
        //                             ROW_NUMBER() OVER(ORDER BY subjectId) rNum,
        //                             * 
        //                         FROM ""Subject"" s
        //                         WHERE userId = @userId AND isDelete =0 AND subjectContent LIKE '%{search}%'
        //                     )S
        //                     WHERE S.rNum BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }
        //                 ";
        //     using var conn = new SqlConnection(cnstr);
        //     return new List<Subject>(conn.Query<Subject>(sql,new {userId}));
        // }
        public List<Subject> SubjectList(int userId,string search){
            string sql = $@"
                            SELECT
                                ROW_NUMBER() OVER(ORDER BY subjectId) rNum,
                                * 
                            FROM ""Subject"" s
                            WHERE userId = @userId AND isDelete =0 AND subjectContent LIKE '%{search}%'
                            ORDER BY rNum DESC
                        ";
            using var conn = new SqlConnection(cnstr);
            return new List<Subject>(conn.Query<Subject>(sql,new {userId}));
        }

        #endregion
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
            return conn.QueryFirst<Subject>(sql,new {insertData.userId, insertData.subjectContent});
        }

        //查詢科目詳細資料
        public SubjectTagViewModel GetSubject(int userId,int subjectId){
            SubjectTagViewModel Data = new();
            string subjectSql = $@"
                                    SELECT 
                                        s.subjectId,
                                        s.userId,
                                        s.subjectContent
                                    FROM ""Subject"" s
                                    WHERE s.userId = @userId AND isDelete =0 AND s.subjectId = @subjectId
                                ";
            string userSql = $@"
                                    SELECT
                                        u.userId,
                                        u.userPhoto,
                                        u.userAccount,
                                        u.userName,
                                        u.userEmail
                                    FROM ""User"" u
                                    WHERE u.userId = @userId
                                ";
            string tagSql = $@"
                                SELECT
                                    t.*
                                FROM SubjectTag st
                                JOIN Tag t ON st.tagId = t.tagId
                                JOIN ""Subject"" s ON s.subjectId = st.subjectId
                                WHERE st.subjectId = @subjectId AND s.isDelete =0 AND s.userId = @userId
                                GROUP BY t.tagId,t.tagContent
                            ";
            using var conn = new SqlConnection(cnstr);
            Data.subject = conn.QueryFirstOrDefault<Subject>(subjectSql,new{userId,subjectId});
            Data.user = conn.QueryFirstOrDefault<User>(userSql,new{userId});
            Data.tagList = new List<Tag>(conn.Query<Tag>(tagSql,new{subjectId,userId}));
            return Data;
        }
        
        //檢查科目資訊
        public bool CheckSubject(InsertSubject data){
            string sql = $@"SELECT COUNT(*) FROM ""Subject"" WHERE subjectContent = @subjectContent AND userId = @userId AND isDelete = 0 ";
            using var conn = new SqlConnection(cnstr);
            if(conn.QueryFirst<int>(sql,new{data.subjectContent,data.userId}) >= 1)
                return true;
            return false;
        }
    
        //軟刪除科目
        public void DeleteSubject(int userId,int subjectId){
            string sql = $@"UPDATE ""Subject""
                            SET isDelete = 1
                            WHERE subjectId = @subjectId AND userId = @userId";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new{ userId, subjectId});
        }
        //更新科目資訊
        public void UpdateSubject(Subject subject){
            string sql = $@"UPDATE ""Subject""
                            SET subjectContent = @subjectContent
                            WHERE subjectId = @subjectId AND userId = @userId
                        ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,subject);
        }
        #endregion
    }
}
