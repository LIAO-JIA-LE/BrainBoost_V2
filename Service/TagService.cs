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
            string sql = $@"
                            SELECT t.* FROM Tag t
                            JOIN SubjectTag st ON t.tagId = st.tagId
                            JOIN [Subject] s ON st.subjectId = s.subjectId
                            WHERE s.userId = @userId AND s.subjectId = @subjectId";
            using var conn = new SqlConnection(cnstr);
            return (List<Tag>)conn.Query<Tag>(sql,new{userId,subjectId});
        }
    }
}