using System.Data.SqlClient;
using BrainBoost_V2.Parameter;
using BrainBoost_V2.Models;
using BrainBoost_V2.ViewModels;
using Dapper;
using System.Text;
using BrainBoost.ViewModels;

namespace BrainBoost_V2.Service
{
    public class RoomService(IConfiguration configuration)
    {
        private readonly string? cnstr = configuration.GetConnectionString("ConnectionStrings");

        #region 新增搶答室
        public int InsertRoom(InsertRoom raceData){
            string roomPinCode = GetCode();
            while(GetClassByPinCode(roomPinCode) != 0){
                roomPinCode = GetCode();
            }
            string createTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            // 新增搶答室資訊
            string sql = $@"INSERT INTO Room( userId, roomName, classId, createTime, roomPinCode, roomFunction, roomPublic, timeLimit)
                            VALUES(@userId,@roomName,@classId, @createTime, @roomPinCode, @roomFunction, @roomPublic, @timeLimit)
                            
                            DECLARE @roomId int;
                            SET @roomId = SCOPE_IDENTITY(); /*自動擷取剛剛新增資料的id*/

                            SELECT * FROM Room WHERE roomId = @roomId
                        ";
            using var conn = new SqlConnection(cnstr);
            Room data = conn.QueryFirstOrDefault<Room>(sql, new {raceData.userId,raceData.roomName,raceData.classId,createTime,roomPinCode,raceData.roomFunction,raceData.roomPublic,raceData.timeLimit});
            RoomQuestion(raceData,data.roomId);
            return data.roomId;
        }

        #region 匯入搶答室問題
        public void RoomQuestion(InsertRoom raceData,int roomId)
        {
            if(raceData.questionId.Count > 0){
                StringBuilder stringBuilder = new();
                for (int i = 0; i < raceData.questionId.Count; i++)
                    stringBuilder.AppendLine($@"INSERT INTO RoomQuestion (roomId, questionId) 
                                                VALUES({roomId}, {raceData.questionId[i]}) ");
                using var conn = new SqlConnection(cnstr);
                conn.Execute(stringBuilder.ToString());
            }
        }
        #endregion

        #endregion


        #region 隨機邀請碼
        public string GetCode()
        {
            string[] Code = {"1","2","3","4","5","6","7","8","9","0"};
            Random rd = new();
            string ValidateCode = string.Empty;
            for (int i = 0; i < 6; i++)
                ValidateCode += Code[rd.Next(Code.Length)];
            return ValidateCode;
        }
        #endregion

        #region 用pinCode獲取roomId
        public int GetRoomIdByPinCode(string pinCode){
            string sql = $@"SELECT roomId FROM Room r
                            JOIN ""Class"" c
                            ON r.classId = c.classId
                            WHERE r.roomPinCode = @pinCode";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<int>(sql,new{pinCode});
        }
        #endregion

        #region 用pinCode獲取資訊
        public int GetClassByPinCode(string pinCode){
            string sql = $@"SELECT Count(*) FROM Room r
                            JOIN ""Class"" c
                            ON r.classId = c.classId
                            WHERE r.roomPinCode = @pinCode";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<int>(sql,new{pinCode});
        }
        #endregion

    #region 顯示搶答室
        //全部搶答室
        public List<RoomList> GetAllRoom(AllRoomViewModel data){
            List<RoomList> dataList;
            if(string.IsNullOrEmpty(data.search)){
                if(data.classId == 0){
                    //無搜尋、無班級篩選
                    SetMaxPage(data.userId,data.forpaging);
                    dataList = GetRoomList(data.userId,data.forpaging);
                }
                else{
                    //無搜尋、有班級篩選
                    SetMaxPage(data.userId,data.classId,data.forpaging);
                    dataList = GetRoomList(data.userId,data.classId,data.forpaging);
                }
            }
            else{
                if(data.classId == 0){
                    //有搜尋、無班級篩選
                    SetMaxPage(data.userId,data.forpaging,data.search);
                    dataList = GetRoomList(data.userId,data.forpaging,data.search);
                }
                else{
                    //有搜尋、有班級篩選
                    SetMaxPage(data.userId,data.classId,data.forpaging,data.search);
                    dataList = GetRoomList(data.userId,data.classId,data.forpaging,data.search);
                }
            }
            return dataList;
        }
        #region 無搜尋、無班級篩選
        public void SetMaxPage(int userId,Forpaging forpaging){
            string sql = $@"SELECT COUNT(*) FROM (
                                SELECT 
                                    ROW_NUMBER() OVER(ORDER BY r.roomId) rNum
                                    ,c.className
                                    ,r.roomId
                                    ,r.roomName
                                FROM Room r 
                                JOIN ""Class"" c
                                ON r.classId = c.classId
                                WHERE r.userId = @userId AND r.isDelete = 0 AND c.isDelete = 0 
                            )rc
                            ";//WHERE rc.rNum BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }
            using var conn = new SqlConnection(cnstr);
            int row = conn.QueryFirst<int>(sql,new{userId});
            forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / forpaging.Item));
            forpaging.SetRightPage();
        }
        public List<RoomList> GetRoomList(int userId,Forpaging forpaging){
            string sql = $@"SELECT * FROM (
                                SELECT 
                                    ROW_NUMBER() OVER(ORDER BY r.roomId) rNum
                                    ,c.className
                                    ,r.roomId
                                    ,r.roomName
                                FROM Room r 
                                JOIN ""Class"" c
                                ON r.classId = c.classId
                                WHERE r.userId = @userId AND r.isDelete = 0 AND c.isDelete = 0
                            )rc
                            WHERE rc.rNum BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }";//AND r.classId = @classId 
            using var conn = new SqlConnection(cnstr);
            return new List<RoomList>(conn.Query<RoomList>(sql,new{userId}));
        }
        #endregion
        #region 無搜尋、有班級篩選
        public void SetMaxPage(int userId,int classId,Forpaging forpaging){
            string sql = $@"SELECT COUNT(*) FROM (
                                SELECT 
                                    ROW_NUMBER() OVER(ORDER BY r.roomId) rNum
                                    ,c.className
                                    ,r.roomId
                                    ,r.roomName
                                FROM Room r 
                                JOIN ""Class"" c
                                ON r.classId = c.classId
                                WHERE r.userId = @userId AND r.classId = @classId AND r.isDelete = 0 AND c.isDelete = 0 
                            )rc
                            ";
            using var conn = new SqlConnection(cnstr);
            int row = conn.QueryFirst<int>(sql,new{userId,classId});
            forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / forpaging.Item));
            forpaging.SetRightPage();
        }
        public List<RoomList> GetRoomList(int userId,int classId,Forpaging forpaging){
            string sql = $@"SELECT * FROM (
                                SELECT 
                                    ROW_NUMBER() OVER(ORDER BY r.roomId) rNum
                                    ,c.className
                                    ,r.roomId
                                    ,r.roomName
                                FROM Room r 
                                JOIN ""Class"" c
                                ON r.classId = c.classId
                                WHERE r.userId = @userId AND r.classId = @classId AND r.isDelete = 0 AND c.isDelete = 0 
                            )rc
                            WHERE rc.rNum BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }";//AND r.classId = @classId 
            using var conn = new SqlConnection(cnstr);
            return new List<RoomList>(conn.Query<RoomList>(sql,new{userId, classId}));
        }
        #endregion
        #region 有搜尋、無班級篩選
        public void SetMaxPage(int userId,Forpaging forpaging,string search){
            string sql = $@"SELECT COUNT(*) FROM (
                                SELECT 
                                    ROW_NUMBER() OVER(ORDER BY r.roomId) rNum
                                    ,c.className
                                    ,r.roomId
                                    ,r.roomName
                                FROM Room r 
                                JOIN ""Class"" c
                                ON r.classId = c.classId
                                WHERE r.userId = @userId AND r.roomName LIKE '%{search}%' AND r.isDelete = 0 AND c.isDelete = 0 
                            )rc
                            ";
            using var conn = new SqlConnection(cnstr);
            int row = conn.QueryFirst<int>(sql,new{userId});
            forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / forpaging.Item));
            forpaging.SetRightPage();
        }
        public List<RoomList> GetRoomList(int userId,Forpaging forpaging,string search){
            string sql = $@"SELECT * FROM (
                                SELECT 
                                    ROW_NUMBER() OVER(ORDER BY r.roomId) rNum
                                    ,c.className
                                    ,r.roomId
                                    ,r.roomName
                                FROM Room r 
                                JOIN ""Class"" c
                                ON r.classId = c.classId
                                WHERE r.userId = @userId AND r.roomName LIKE '%{search}%' AND r.isDelete = 0 AND c.isDelete = 0 
                            )rc
                            WHERE rc.rNum BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }";//AND r.classId = @classId 
            using var conn = new SqlConnection(cnstr);
            return new List<RoomList>(conn.Query<RoomList>(sql,new{userId}));
        }
        #endregion
        #region 有搜尋、有班級篩選
        public void SetMaxPage(int userId,int classId,Forpaging forpaging,string search){
            string sql = $@"SELECT COUNT(*) FROM (
                                SELECT 
                                    ROW_NUMBER() OVER(ORDER BY r.roomId) rNum
                                    ,c.className
                                    ,r.roomId
                                    ,r.roomName
                                FROM Room r 
                                JOIN ""Class"" c
                                ON r.classId = c.classId
                                WHERE r.userId = @userId AND r.classId = @classId AND r.roomName LIKE '%{search}%' AND r.isDelete = 0 AND c.isDelete = 0 
                            )rc
                            ";
            using var conn = new SqlConnection(cnstr);
            int row = conn.QueryFirst<int>(sql,new{userId,classId});
            forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / forpaging.Item));
            forpaging.SetRightPage();
        }
        public List<RoomList> GetRoomList(int userId,int classId,Forpaging forpaging,string search){
            string sql = $@"SELECT * FROM (
                                SELECT 
                                    ROW_NUMBER() OVER(ORDER BY r.roomId) rNum
                                    ,c.className
                                    ,r.roomId
                                    ,r.roomName
                                FROM Room r 
                                JOIN ""Class"" c
                                ON r.classId = c.classId
                                WHERE r.userId = @userId AND r.classId = @classId AND r.roomName LIKE '%{search}%' AND r.isDelete = 0 AND c.isDelete = 0 
                            )rc
                            WHERE rc.rNum BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }";//AND r.classId = @classId 
            using var conn = new SqlConnection(cnstr);
            return new List<RoomList>(conn.Query<RoomList>(sql,new{userId, classId}));
        }
        #endregion
        //string sql = $@" SELECT	* FROM Room WHERE isDelete = 0 AND userId = @userId ORDER BY createTime DESC ";
        //單一搶答室
        public RoomClassViewModel GetRoom(int roomId,int userId){
            string sql = $@"SELECT
                                r.*,
                                c.className
                            FROM Room r
                            JOIN ""Class"" c
                            ON r.classId = c.classId
                            WHERE r.userId = @userId AND r.isDelete = 0 AND c.isDelete = 0 AND r.roomId = @roomId
                            ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<RoomClassViewModel>(sql, new { roomId, userId });
        }
    #endregion
        #region 修改搶答室資訊
        public void UpdateRoom(UpdateRoomInfo roomData){
            // 新增搶答室資訊
            string sql = $@"UPDATE Room SET 
                                roomName = @roomName,
                                roomFunction = @roomFunction,
                                timeLimit = @timeLimit,
                                classId = @classId
                            WHERE roomId = @roomId AND isDelete = 0 AND isOpen = 0";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql, new {roomData.roomName,roomData.roomFunction, roomData.timeLimit, roomData.roomId, roomData.classId});
        }
        #endregion
        #region 刪除搶答室
        public string DeleteRoom(int roomId,int userId){
            string sql = $@"UPDATE Room SET isDelete = 1 WHERE roomId = @roomId AND userId = @userId AND isDelete = 0
                            SELECT roomName FROM Room WHERE roomId = @roomId AND userId = @userId
                        ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<string>(sql, new{roomId, userId});
        }
        #endregion
        #region 搶答室的題目（只有題目內容）
        public List<Question> RoomQuestionList(int roomId,int userId){
            string sql = $@"SELECT
                                q.*
                            FROM(
                                SELECT
                                    rq.roomId,
                                    rq.questionId
                                FROM RoomQuestion rq
                                JOIN Room r
                                ON rq.roomId = r.roomId
                                WHERE rq.isOutput = 0 AND rq.isDelete = 0 AND r.isDelete = 0 AND r.userId = @userId AND r.roomId = @roomId
                            )rrq
                            INNER JOIN Question q
                            ON rrq.questionId = q.questionId";
            using (var conn = new SqlConnection(cnstr))
            return (List<Question>)conn.Query<Question>(sql,new {roomId,userId});
        }
        #endregion
        #region 搶答室的題目（只有題目id）
        public List<int> RoomQuestionIdList(int roomId,int userId){
            string sql = $@"SELECT
                                q.questionId
                            FROM(
                                SELECT
                                    rq.roomId,
                                    rq.questionId
                                FROM RoomQuestion rq
                                JOIN Room r
                                ON rq.roomId = r.roomId
                                WHERE rq.isOutput = 0 AND rq.isDelete = 0 AND r.isDelete = 0 AND r.userId = @userId AND r.roomId = @roomId
                            )rrq
                            INNER JOIN Question q
                            ON rrq.questionId = q.questionId";
            using (var conn = new SqlConnection(cnstr))
            return (List<int>)conn.Query<int>(sql,new {roomId,userId});
        }
        #endregion
        #region 修改搶答室題目
        public void UpdateRoomQuestion(UpdateRoomQuestion UpdateData){
            using var conn = new SqlConnection(cnstr);
            List<int> RoomQuestions = RoomQuestionIdList(UpdateData.roomId,UpdateData.userId);
            var qToAdd = UpdateData.questionId.Except(RoomQuestions).ToList();
            var qToDelete = RoomQuestions.Except(UpdateData.questionId).ToList();
            // 刪除題目
            if(qToDelete.Count > 0){
                foreach (var questionId in qToDelete){
                    conn.Execute(
                        $@" DELETE FROM RoomQuestion
                            WHERE roomId = @roomId AND questionId = @questionId
                        ",
                        new { UpdateData.roomId, questionId }
                    );
                }
            }
            // 新增題目
            if(qToAdd.Count > 0){
                foreach (var questionId in qToAdd){
                    conn.Execute(
                        $@" INSERT INTO RoomQuestion(roomId,questionId)
                            VALUES(@roomId,@questionId)
                        ",
                        new { UpdateData.roomId, questionId }
                    );
                }
            }
        }
        #endregion
        #region 檢查搶答室
        public bool CheckRoom(string roomName,int userId){
            string sql = $@"SELECT COUNT(*) FROM ""Room"" WHERE roomName = @roomName AND userId = @userId AND isDelete = 0 ";
            using var conn = new SqlConnection(cnstr);
            if(conn.QueryFirst<int>(sql,new{roomName,userId}) >= 1)
                return true;
            return false;

        }
        #endregion

        #region 隨機出題
        public RoomQuestionViewModel RandomQuestion(int roomId){
            // 獲得題目id跟題型id
            List<RoomQuestionListType> questionIdList = GetRoomQuestionType(roomId);

            if(questionIdList.Count > 0){
                // 隨機出題
                Random rd = new();
                // if(questionIdList.type_id == 1)
                RoomQuestionViewModel question = GetRandomQuestion(questionIdList[rd.Next(questionIdList.Count)]);
                return question;
            }
            else{
                //已經出完所有題目 重製題目的is_output
                ResetRaceRoomQuestion(roomId);
                return null;
            }
        }
        public List<RoomQuestionListType> GetRoomQuestionType(int roomId){
            string sql = $@"SELECT
                                R.roomId,
                                Q.questionId,
                                typeId
                            FROM Question Q
                            INNER JOIN RoomQuestion R
                            ON Q.questionId = R.questionId
                            WHERE roomId = @roomId AND isOutput = 0 AND q.isDelete = 0
                            ORDER BY typeId";
            using var conn = new SqlConnection(cnstr);
            return (List<RoomQuestionListType>)conn.Query<RoomQuestionListType>(sql, new{roomId});
        }
        public RoomQuestionViewModel GetRandomQuestion(RoomQuestionListType Question){
            Random rd = new();
            string sql2 = String.Empty;
            // 顯示題目和答案 
            // 將出過的題目設定已出現
            string sql = $@"SELECT
                                questionId,
                                questionContent,
                                questionPicture
                            FROM Question
                            WHERE questionId = @questionId AND isDelete = 0
                            
                            UPDATE RoomQuestion
                            SET isOutput = 1
                            WHERE roomId = @roomId AND questionId = @questionId";
            // 選項
            if(Question.typeId == 2){
                sql2 = $@"SELECT
                            O.questionId,
                            O.optionContent,
                            O.optionPicture
                        FROM ""Option"" O
                        INNER JOIN Question Q
                        ON O.questionId = Q.questionId
                        WHERE q.isDelete = 0 AND Q.questionId = @questionId";
            }
            using var conn = new SqlConnection(cnstr);
            // 宣告raceQuestionList
            RoomQuestionViewModel roomQuestionList = new();
            
            // 執行顯示題目和答案
            roomQuestionList.question = conn.QueryFirstOrDefault<Question>(sql, new {Question.questionId, Question.roomId});
            
            // 如果是選擇題的話顯示選項
            if(!String.IsNullOrEmpty(sql2)){
                List<Option> options = new List<Option>(conn.Query<Option>(sql2, new {Question.questionId}));
                roomQuestionList.options = options.OrderBy(x => rd.Next()).ToList();
            }
            return roomQuestionList;
                
        }
        public void ResetRaceRoomQuestion(int roomId){
            string sql = $@"
                            UPDATE RoomQuestion
                            SET isOutput = 0
                            WHERE roomId = @roomId
                        ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new{ roomId });
        }
        #endregion

        #region 取得限時
        public float GetTimeLimitByRId(int roomId){
            string sql = $@"SELECT
                                timeLimit
                            FROM Room
                            WHERE roomId = @roomId";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<float>(sql, new{ roomId });
        }
        #endregion

        #region 計時&答案
        public void StorageTimers(int level, string answer, StudentResponse studentResponse)
        {
            float limit = 0;
            if(studentResponse.timeLimit > studentResponse.timeCose){
                if(studentResponse.answer.Equals(answer)){
                    limit = studentResponse.timeLimit - studentResponse.timeCose;
                    SaveResponse(level, limit, studentResponse, true);
                }
                else{
                    SaveResponse(level, limit, studentResponse, false);
                }
            }
            else{
                SaveResponse(level, limit, studentResponse, false);
            }  
        }
        #endregion

        #region 儲存回應
        public void SaveResponse(int level, float limit, StudentResponse studentResponse, bool isCurrect){
            string sql = $@"INSERT INTO RoomResponse(roomId, roomQuestionId, guestId, answer, score, isCurrect, timeCose)
                            VALUES(@roomId, @roomQuestionId, @guestId, @answer, @score, @isCurrect, @timeCose )";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql, new{roomId = studentResponse.roomId, roomQuestionId = studentResponse.questionId, guestId = studentResponse.guestId, answer = studentResponse.answer, score = studentResponse.score, isCurrect = isCurrect, timeCose = studentResponse.timeCose});
        }
        #endregion

        #region 統計學生回應
        public object GetStudentReseponse(int roomId, int questionId, List<string> optionContent){
            // 統計
            string sql = $@"SELECT answer, COUNT(guestId) AS option_static FROM RoomResponse 
                            WHERE roomId = @roomId AND questionId = @questionId AND answer IN @options GROUP BY answer";
            using var conn = new SqlConnection(cnstr);
            conn.Open();

            var parameters = new { roomId, questionId, options = optionContent };
            var optionStatics = conn.Query<(string option, int option_static)>(sql, parameters).ToDictionary(t => t.option, t => t.option_static);

            List<StaticOption> staticOptions = optionContent.Select(option => new StaticOption
            {
                option = option,
                optionStatic = optionStatics.ContainsKey(option) ? optionStatics[option] : 0
            }).ToList();

            conn.Close();

            return staticOptions;
        }
        #endregion

        #region 記分板
        public object GetScoreBoard(int roomId){
            string sql = $@"SELECT
                                g.guestId,
                                guestName,
                                SUM(score) guestTotalScore
                            FROM Guest g
                            INNER JOIN RoomResponse r
                            ON g.guestId, = r.g.guestId,
                            WHERE r.roomId = @roomId
                            GROUP BY g.guestId, guestName
                            ORDER BY guestTotalScore DESC";
            using var conn = new SqlConnection(cnstr);
            return conn.Query<ScoreBoard>(sql, new{roomId});
        }
        #endregion

        #region 個人分數
        public object GetScoreByGuest(int roomId, int guestId){
            string sql = $@"SELECT
                                g.guestId,
                                guestName,
                                SUM(score) guestTotalScore
                            FROM Guest g
                            INNER JOIN RoomResponse r
                            ON g.guestId, = r.g.guestId,
                            WHERE r.roomId = @roomId AND g.guestId = @guestId
                            GROUP BY g.guestId, guestName
                            ORDER BY guestTotalScore DESC";
            using var conn = new SqlConnection(cnstr);
            return conn.Query<ScoreBoard>(sql, new{roomId, guestId});
        }
        #endregion
    }
}