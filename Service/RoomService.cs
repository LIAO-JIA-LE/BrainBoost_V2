using System.Data.SqlClient;
using BrainBoost_V2.Parameter;
using BrainBoost_V2.Models;
using BrainBoost_V2.ViewModels;
using Dapper;
using System.Text;

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
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < raceData.questionId.Count; i++)
                stringBuilder.AppendLine($@"INSERT INTO RoomQuestion (roomId, questionId) 
                                            VALUES({roomId}, {raceData.questionId[i]}) ");
            using var conn = new SqlConnection(cnstr);
            conn.Execute(stringBuilder.ToString());
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
        #region 用pinCode獲取資訊
        //classId
        public int GetClassByPinCode(string pinCode){
            string sql = $@"SELECT COUNT(*) FROM Room r
                            JOIN ""Class"" c
                            ON r.classId = c.classId
                            WHERE r.roomPinCode = @pinCode";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<int>(sql,new{pinCode});
        }
        //
        #endregion

        #region 顯示搶答室
        //全部搶答室
        public List<Room> GetAllRoom(int userId){
            string sql = $@" SELECT	* FROM Room WHERE isDelete = 0 AND userId = @userId ORDER BY createTime DESC ";
            using (var conn = new SqlConnection(cnstr))
            return (List<Room>)conn.Query<Room>(sql, new{userId});
        }
        //單一搶答室
        public Room GetRoom(int roomId,int userId){
            string sql = $@" SELECT	* FROM Room WHERE roomId = @roomId AND isDelete = 0 AND userId = @userId";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<Room>(sql, new { roomId, userId });
        }
        #endregion
        #region 修改搶答室資訊
        public void UpdateRoom(RoomUpdate roomData){
            // 新增搶答室資訊
            string sql = $@"UPDATE Room SET roomName = @roomName, roomFunction = @roomFunction,
                            timeLimit = @timeLimit WHERE roomId = @roomId ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql, new {roomData.roomName,roomData.roomFunction, roomData.timeLimit, roomData.roomId});
        }
        #endregion
    }
}