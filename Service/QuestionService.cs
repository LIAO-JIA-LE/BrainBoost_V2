using System.Data.SqlClient;
using Dapper;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using BrainBoost_V2.ViewModels;
using BrainBoost_V2.Models;
using BrainBoost_V2.Services;
using NPOI.HSSF.UserModel;

namespace BrainBoost_V2.Service
{
    public class QuestionService(IConfiguration configuration)
    {
        #region 宣告函式
        private readonly string? cnstr = configuration.GetConnectionString("ConnectionStrings");
        #endregion

        #region 新增題目
        public void InsertQuestion(GetQuestion getQuestion)
        {
            string sql = $@"INSERT INTO Question(userId, typeId, questionContent, questionPicture, questionLevel, isDelete)
                            VALUES(@UserId, @TypeId, @QuestionContent, @QuestionPicture, @QuestionLevel, @IsDelete);
                            
                            SELECT CAST(SCOPE_IDENTITY() as int)";
            
            using var conn = new SqlConnection(cnstr);
            getQuestion.questionData.questionId = conn.QueryFirstOrDefault<int>(sql, new 
            { 
                UserId = getQuestion.questionData.userId, 
                TypeId = getQuestion.questionData.typeId, 
                QuestionContent = getQuestion.questionData.questionContent, 
                QuestionPicture = getQuestion.questionData.questionPicture, 
                QuestionLevel = getQuestion.questionData.questionLevel, 
                IsDelete = getQuestion.questionData.isDelete
            });
            InsertOption(getQuestion);
        }
        public void InsertOption(GetQuestion getQuestion)
        {
            StringBuilder stringBuilder = new ();
            int questionId = getQuestion.questionData.questionId;
            
            // 答案
            stringBuilder.Append($@"INSERT INTO Answer(questionId, answerContent, parse)
                                    VALUES({questionId},'{getQuestion.answerData.answerContent}','{getQuestion.answerData.parse}')
                                    DECLARE @answerId int;
                                    SET @answerId = SCOPE_IDENTITY()");
                            
            // 題目標籤
            // 看有沒有Tag的資訊
            if(!String.IsNullOrEmpty(getQuestion.tagData.tagContent)){
                // 獲得沒有重複的Tag
                if(String.IsNullOrEmpty(NotRepeatQuestionTag(getQuestion.questionData.userId, getQuestion.tagData.tagContent)))
                    InsertTag(getQuestion.subjectData.subjectId, getQuestion.tagData.tagContent);
                
                getQuestion.tagData.tagId = GetTagId(getQuestion.tagData.tagContent);
                stringBuilder.Append($@" INSERT INTO TagQuestion (tagId, questionId) VALUES ('{getQuestion.tagData.tagId}', '{questionId}') ");
            }
            
            // 選擇題
            if(getQuestion.questionData.typeId == 2)
            {
                for(int i = 0; i < 4; i++)
                {
                    //新增判斷是否為答案
                    stringBuilder.Append($@"INSERT INTO ""Option""(questionId, optionContent, optionPicture, isAnswer)   
                                            VALUES('{questionId}', '{getQuestion.options[i]}', '{getQuestion.optionImg[i]}','{getQuestion.options[i] == getQuestion.answerData.answerContent}')");
                }
            }
            
            // // 問答題
            // else if (getQuestion.questionData.typeId == 3)
            // {
            //     stringBuilder.Append($@"INSERT INTO ""Option""(questionId, optionContent, isAnswer)
            //                             VALUES('{questionId}', '{getQuestion.options}')");
            // }

            // 執行Sql
            using var conn = new SqlConnection(cnstr);
            getQuestion.answerData.answerId = conn.Execute(stringBuilder.ToString());
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
            string sql = $@"INSERT Tag(tagContent) VALUES( @tagContent )
                            DECLARE @tagId int;
                            SET @tagId = SCOPE_IDENTITY()";
            // 執行Sql
            using var conn = new SqlConnection(cnstr);
            // int tagId = conn.QueryFirstOrDefault<int>(sql, new{tagContent});

            sql += $@"INSERT SubjectTag(subjectId, tagId) VALUES(@subjectId, @tagId)";
            conn.Execute(sql, new{ subjectId,tagContent});
        }

        #endregion

        #region 題目列表（只顯示題目內容，不包含選項）
        // 全部的題目(篩選)
        public List<Question> GetQuestionList(int userId,int type, string search,Forpaging forpaging){
            List<Question> questionList;
            if(!String.IsNullOrEmpty(search)){
                SetMaxPage(userId,type,search,forpaging);
                questionList = GetQuestion(userId,type,search,forpaging);
            }
            else{
                SetMaxPage(userId,type,forpaging);
                questionList = GetQuestion(userId,type,forpaging);
            }
            //指定時間格式
            return questionList;
        }
        #region 無搜尋
        public void SetMaxPage(int userId,int typeId,Forpaging forpaging){
            string sql = $@" SELECT COUNT(*) FROM Question WHERE isDelete=0 AND userId = @userId AND 1=1";
            if(typeId != 0)
                sql = sql.Replace("1=1", $@"1=1 AND typeId = @typeId");
            using var conn = new SqlConnection(cnstr);
            int row = conn.QueryFirst<int>(sql,new{userId,typeId});
            forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / forpaging.Item));
            forpaging.SetRightPage();
        }
        public List<Question> GetQuestion(int userId,int typeId,Forpaging forpaging){
            string sql = $@"SELECT
                                *
                            FROM(
                                SELECT 
                                    ROW_NUMBER() OVER(ORDER BY q.questionId) qNum,
                                    *
                                FROM Question q
                                WHERE isDelete=0 AND userId = @userId AND 1=1
                            )Q
                            WHERE Q.qNum BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }";
            if(typeId != 0)
                sql = sql.Replace("1=1", $@"1=1 AND typeId = @typeId");
            using var conn = new SqlConnection(cnstr);
            return new List<Question>(conn.Query<Question>(sql,new{userId,typeId}));
        }
        #endregion
        #region 有搜尋
        public void SetMaxPage(int userId,int typeId,string search,Forpaging forpaging){
            string sql = $@" SELECT COUNT(*) FROM Question WHERE isDelete=0 AND userId = @userId AND questionContent LIKE '%{search}%' AND 1=1";
            if(typeId != 0)
                sql = sql.Replace("1=1", $@"1=1 AND typeId = @typeId");
            using var conn = new SqlConnection(cnstr);
            int row = conn.QueryFirst<int>(sql,new{userId,typeId});
            forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / forpaging.Item));
            forpaging.SetRightPage();
        }
        public List<Question> GetQuestion(int userId,int typeId,string search,Forpaging forpaging){
            string sql = $@"SELECT
                                *
                            FROM(
                                SELECT 
                                    ROW_NUMBER() OVER(ORDER BY q.questionId) qNum,
                                    *
                                FROM Question q
                                WHERE isDelete=0 AND userId = @userId AND questionContent LIKE '%{search}%' AND 1=1
                            )Q
                            WHERE Q.qNum BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }";
            if(typeId != 0)
                sql = sql.Replace("1=1", $@"1=1 AND typeId = @typeId");
            using var conn = new SqlConnection(cnstr);
            return new List<Question>(conn.Query<Question>(sql,new{userId,typeId}));
        }
        #endregion
        // 根據Id獲取題目內容
        public Question GetQuestionById(int questionId){
            string sql = $@" SELECT * FROM Question WHERE questionId = @questionId AND isDelete = 0";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<Question>(sql, new{questionId});
        }

        #endregion

        #region 檔案匯入
        public DataTable FileDataPrecess(IFormFile file){
            Stream stream = file.OpenReadStream();
            DataTable dataTable = new DataTable();
            IWorkbook wb;
            ISheet sheet;
            IRow headerRow;
            int cellCount;
            string outputFolder = "../wwwroot/QuestionPicture/";

            if (file.FileName.ToUpper().EndsWith("XLSX"))
                wb = new XSSFWorkbook(stream);
            else
                wb = new HSSFWorkbook(stream);

            sheet = wb.GetSheetAt(0);
            headerRow = sheet.GetRow(0);
            cellCount = headerRow.LastCellNum;

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                dataTable.Columns.Add(new DataColumn(headerRow.GetCell(i).StringCellValue));

            try{
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (string.IsNullOrEmpty(row.Cells[0].ToString().Trim())) break;
                    DataRow dataRow = dataTable.NewRow();
                    int column = 1;
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                    
                        try
                        {
                            ICell cell;
                            column = j + 1;
                            cell = row.GetCell(j);

                            // 剩下的資料
                            switch (cell.CellType)
                            {
                                // 字串
                                case CellType.String:
                                    if (cell.StringCellValue != null)
                                        // 設定dataRow第j欄位的值，cell以字串型態取值
                                        dataRow[j] = cell.StringCellValue;
                                    else
                                        dataRow[j] = "";
                                    break;

                                // 數字
                                case CellType.Numeric:
                                    // 日期
                                    if (HSSFDateUtil.IsCellDateFormatted(cell))
                                        // 設定dataRow第j欄位的值，cell以日期格式取值
                                        dataRow[j] = DateTime.FromOADate(cell.NumericCellValue).ToString("yyyy/MM/dd HH:mm");
                                    else
                                        // 非日期格式
                                        dataRow[j] = cell.NumericCellValue;
                                    break;

                                // 布林值
                                case CellType.Boolean:
                                    // 設定dataRow第j欄位的值，cell以布林型態取值
                                    dataRow[j] = cell.BooleanCellValue;
                                    break;

                                //空值
                                case CellType.Blank:
                                    dataRow[j] = "";
                                    break;

                                // 預設
                                default:
                                    dataRow[j] = cell.StringCellValue;
                                    break;
                            }
                            dataTable.Rows.Add(dataRow);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("第 " + i + "列，資料格式有誤:\r\r" + e.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
            return dataTable;
        }
        #endregion
    }
}