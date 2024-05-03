using System.Data.SqlClient;
using BrainBoost_V2.Models;
using BrainBoost_V2.Parameter;
using Dapper;

namespace BrainBoost_V2.Service
{
    public class GuestService(IConfiguration configuration)
    {
        private readonly string? cnstr = configuration.GetConnectionString("ConnectionStrings");
        public int PinCodeCheck(string roomPinCode){
            string sql = $@"SELECT COUNT(*) FROM ""Room"" WHERE roomPinCode = @roomPinCode";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault(sql,new{roomPinCode});
        }
        public Guest InsertGuest(Guest guest){
            string sql = $@"INSERT INTO Guest(guestName,classId)
                            VALUES(@guestName,@classId)

                            DECLARE @guestId INT
                            SET @guestId = SCOPE_IDENTITY()

                            SELECT * FROM Guest WHERE guestId = @guestId    
                        ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<Guest>(sql,new{guest.guestName,guest.classId});
        }
    }
}