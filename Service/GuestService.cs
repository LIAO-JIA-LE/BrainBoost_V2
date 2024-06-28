using System.Data.SqlClient;
using BrainBoost_V2.Models;
using BrainBoost_V2.Parameter;
using Dapper;

namespace BrainBoost_V2.Service
{
    public class GuestService(IConfiguration configuration)
    {
        private readonly string? cnstr = configuration.GetConnectionString("ConnectionStrings");

        #region 判斷pin碼
        public bool PinCodeCheck(string pinCode) {
            string sql = $@"SELECT COUNT(*) FROM ""Room"" WHERE roomPinCode = @roomPinCode";
            using var conn = new SqlConnection(cnstr);
            int count = conn.QueryFirstOrDefault<int>(sql, new { roomPinCode = pinCode });
            return count > 0;
        }
        #endregion

        #region 新增訪客
        public Guest InsertGuest(Guest guest){
            string sql = $@"INSERT INTO Guest(guestName, roomId, isJoined, visitTime)
                            VALUES(@guestName, @roomId, @isJoined, @visitTime)

                            DECLARE @guestId INT
                            SET @guestId = SCOPE_IDENTITY()

                            SELECT * FROM Guest WHERE guestId = @guestId    
                        ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<Guest>(sql,new{guest.guestName,guest.roomId, isJoined = true, visitTime = DateTime.Now});
        }
        #endregion

        #region 訪客列表
        public List<string> GetGuestListByRoomId(int roomId){
            string sql = $@" SELECT guestName FROM Guest WHERE roomId = @roomId ";
            using var conn = new SqlConnection(cnstr);
            return (List<string>)conn.Query<string>(sql, new { roomId } );
        }
        #endregion

        #region 訪客數量
        public int GetGuestCountByRoomId(int roomId) {
            string sql = $@"SELECT COUNT(*) FROM ""Guest"" WHERE roomId = @roomId";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<int>(sql, new { roomId });
        }
        #endregion
    }
}