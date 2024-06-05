using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;
using Dapper;

namespace BrainBoost_V2.Service
{
    public class TagService(IConfiguration configuration)
    {
        // 連線字串
        private string? cnstr = configuration.GetConnectionString("ConnectionStrings");
        
        public List<Tag> GetAllTag(int userId,int subjectId){
            string sql;
            if(subjectId == 0){
                sql = $@"
                        SELECT t.tagId,t.tagContent FROM TagQuestion tq
                        JOIN SubjectTag st ON tq.tagId = st.tagId
                        JOIN [Subject] s ON st.subjectId = s.subjectId
                        JOIN Tag t ON tq.tagId = t.tagId
                        JOIN Question q ON q.questionId = tq.questionId
                        WHERE s.userId = @userId AND s.subjectId = @subjectId AND q.isDelete = 0
                        GROUP BY t.tagId,t.tagContent
                    ";
            }
            else{
                sql = $@"
                        SELECT t.tagId,t.tagContent FROM TagQuestion tq
                        JOIN SubjectTag st ON tq.tagId = st.tagId
                        JOIN [Subject] s ON st.subjectId = s.subjectId
                        JOIN Tag t ON tq.tagId = t.tagId
                        WHERE s.userId = @userId AND s.subjectId = @subjectId
                        GROUP BY t.tagId,t.tagContent
                    ";
            }
            using var conn = new SqlConnection(cnstr);
            return (List<Tag>)conn.Query<Tag>(sql,new{userId,subjectId});
        }
    }
}