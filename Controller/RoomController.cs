using BrainBoost_V2.Models;
using BrainBoost_V2.Service;
using BrainBoost_V2.Parameter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrainBoost_V2.Controller
{
    [Route("BrainBoost/[controller]")]
    public class RoomController(UserService _userService,RoomService _roomService) : ControllerBase
    {
        readonly UserService UserService = _userService;
        readonly RoomService RoomService = _roomService;

        #region 新增搶答室
        // 新增搶答室
        // 解題方式(roomFunction)預設為0(0:最後一次顯示,1:逐題解析)
        // 是否公開(roomPublic)預設為0(0:不公開,1:公開)
        // 時間限制(roomTimeLimit)預設為30秒
        [HttpPost]
        [Route("")]
        public IActionResult InsertRoom([FromBody]InsertRoom raceData){
            try{
                raceData.userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                var Response = raceData;
                RoomService.InsertRoom(raceData);
                return Ok(new Response{
                    status_code = 200,
                    message = "新增成功",
                    data = raceData
                });
                
            }
            catch (Exception e)
            {
                return BadRequest(new Response{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }

        #endregion
        #region 顯示搶答室
        [HttpGet]
        [Route("AllRoom")]
        public IActionResult GetAllRoom(){
            try
            {
                int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                var Response = RoomService.GetAllRoom(userId);
                if(Response == null){
                    return Ok(new Response{
                        status_code = 200,
                        message = "查無資料"
                    });
                }
                else{
                    return Ok(new Response{
                        status_code = 200,
                        message = "讀取成功",
                        data = Response
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new Response{
                        status_code = 400,
                        message = e.Message
                });
            }
        }
        #endregion
    }
}