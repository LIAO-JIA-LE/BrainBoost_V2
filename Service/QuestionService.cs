using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainBoost_V2.Models;
using BrainBoost_V2.Parameter;
using BrainBoost_V2.ViewModels;
using Dapper;

namespace BrainBoost_V2.Service
{
    public class QuestionService(IConfiguration configuration)
    {
        #region 宣告函式
        private readonly string? cnstr = configuration.GetConnectionString("ConnectionStrings");
        #endregion

        #region 新增題目
        public int InsertQuestion(GetQuestion getQuestion)
        {
            string sql = $@"INSERT INTO Question(userId, typeId, questionContent, questionPicture, questionLevel, isDelete)
                            VALUES('{getQuestion.questionData.userId}',{getQuestion.questionData.typeId},{getQuestion.questionData.questionContent},
                            '{getQuestion.questionData.questionPicture}','{getQuestion.questionData.questionLevel}',
                            '{getQuestion.questionData.isDelete}'')
                            
                            DECLARE @questionId int;
                            SET @questionId = SCOPE_IDENTITY();
                            SELECT @questionId";
            
            using var conn = new SqlConnection(cnstr);
            getQuestion.questionData.questionId = conn.QueryFirstOrDefault<int>(sql);
            InsertOption(getQuestion);
            return getQuestion.questionData.questionId;
        }
        public void InsertOption(GetQuestion getQuestion)
        {
            StringBuilder stringBuilder = new ();
            int questionId = getQuestion.questionData.questionId;
            
            // 答案
            stringBuilder.Append($@"INSERT INTO Answer(questionId, answerContent, parse)
                                    VALUES({questionId},'{getQuestion.answerData.answerContent}','{getQuestion.answerData.parse}')");
                            
            // 題目標籤
            // 看有沒有Tag的資訊
            if(!String.IsNullOrEmpty(getQuestion.tagData.tagContent)){
                // 獲得沒有重複的Tag
                if(String.IsNullOrEmpty(NotRepeatQuestionTag(getQuestion.questionData.userId, getQuestion.tagData.tagContent)))
                    InsertTag(getQuestion.subjectData.subjectId, getQuestion.tagData.tagContent);
                
                int tagId = GetTagId(getQuestion.tagData.tagContent);
                stringBuilder.Append($@" INSERT INTO TagQuestion (tagId, questionId) VALUES ('{tagId}', '{questionId}') ");
            }
            
            // 選擇題
            if(getQuestion.questionData.typeId == 2)
            {
                for(int i = 0; i < 4; i++)
                {
                    //新增判斷是否為答案
                    stringBuilder.Append($@"INSERT INTO ""Option""(questionId, optionContent, isAnswer)   
                                            VALUES('{questionId}', '{getQuestion.options[i]}','{getQuestion.options[i] == getQuestion.answerData.answerContent}')");
                }
            }
            // 問答題
            else if (getQuestion.questionData.typeId == 3)
            {
                stringBuilder.Append($@"INSERT INTO ""Option""(questionId, optionContent, isAnswer)
                                        VALUES('{questionId}', '{getQuestion.options}')");
            }

            // 執行Sql
            using var conn = new SqlConnection(cnstr);
            conn.Execute(stringBuilder.ToString());
        }

        public string NotRepeatQuestionTag(int userId, string tagContent){
            string sql = $@"SELECT
                                T.tagContent
                            FROM ""Subject"" S
                            INNER JOIN SubjectTag ST ON S.subjectId = ST.subjectId
                            INNER JOIN Tag T ON ST.tagId = T.tagId
                            WHERE S.userId = @userId AND T.tagContent = @tagContent ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<string>(sql, new{userId, tagContent});
        }
        public int GetTagId(string tagContent){
            string sql = $@" SELECT tagId From Tag WHERE tagContent = @tagContent ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<int>(sql, new{tagContent});
        }
        public void InsertTag(int subjectId, string tagContent){
            string sql = $@"INSERT Tag(tag_name) VALUES( @tagContent )
                            DECLARE @tagId int;
                            SET @tagId = SCOPE_IDENTITY();
                            SELECT @tagId";
            // 執行Sql
            using var conn = new SqlConnection(cnstr);
            int tagId = conn.QueryFirstOrDefault<int>(sql, new{tagContent});

            string sql2 = $@"INSERT SubjectTag(subjectId, tagId) VALUES(@subjectId, @tagId)";
            conn.Execute(sql2, new{ subjectId, tagId });
        }

        #endregion
    }
}