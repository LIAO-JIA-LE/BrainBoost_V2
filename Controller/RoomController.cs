using BrainBoost_V2.Models;
using BrainBoost_V2.Service;
using BrainBoost_V2.Parameter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrainBoost_V2.ViewModels;

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
        public IActionResult InsertRoom([FromBody]InsertRoom roomData){
            try{
                roomData.userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                var Response = roomData;
                int roomId = RoomService.InsertRoom(roomData);
                RoomClassViewModel room = RoomService.GetRoom(roomId,roomData.userId);
                return Ok(new Response{
                    status_code = 200,
                    message = "新增成功",
                    data = room
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
        //全部搶答室
        [HttpGet]
        [Route("AllRoom")]
        public IActionResult GetAllRoom([FromQuery]string search,[FromQuery]int classId,[FromQuery]int page = 1){
            try
            {
                AllRoomViewModel allRoomViewModel = new AllRoomViewModel
                {
                    userId = UserService.GetDataByAccount(User.Identity.Name).userId,
                    classId = classId,
                    forpaging = new Forpaging(page),
                    search = search
                };
                allRoomViewModel.roomList = RoomService.GetAllRoom(allRoomViewModel);
                if (allRoomViewModel.roomList == null){
                    return Ok(new Response{
                        status_code = 200,
                        message = "查無資料",
                        data = allRoomViewModel
                    });
                }
                else{
                    return Ok(new Response{
                        status_code = 200,
                        message = "讀取成功",
                        data = allRoomViewModel
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
        //單一搶答室
        [HttpGet]
        [Route("")]
        public IActionResult GetRoom([FromQuery]int roomId){
            try
            {
                int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                RoomClassViewModel roomClassViewModel = RoomService.GetRoom(roomId, userId);
                if(roomClassViewModel == null){
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "該班級已刪除或無搶答室"
                    });
                }
                else{
                    return Ok(new Response{
                        status_code = 200,
                        message = "讀取成功",
                        data = roomClassViewModel
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
        //單一搶答室(題目)
        [HttpGet]
        [Route("Question")]
        public IActionResult GetRoomQuestion([FromQuery]int roomId){
            try
            {
                int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                RoomQuestionViewModel roomQuestionList = new()
                {
                    room = RoomService.GetRoom(roomId, userId),
                    questionList = RoomService.RoomQuestionList(roomId, userId)
                };
                if (roomQuestionList == null){
                    return Ok(new Response{
                        status_code = 200,
                        message = "該班級已刪除"
                    });
                }
                else{
                    return Ok(new Response{
                        status_code = 200,
                        message = "讀取成功",
                        data = roomQuestionList
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
        #region 修改搶答室
        // 修改 搶答室資訊
        [HttpPut]
        [Route("")]
        public IActionResult UpdateRoom([FromBody]UpdateRoom raceData){
            try
            {
                int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                RoomService.UpdateRoom(raceData);
                var Response = RoomService.GetRoom(raceData.roomId, userId);
                return Ok(new Response{
                    status_code = 200,
                    message = "修改成功",
                    data = Response
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
        #region 刪除搶答室
        [HttpDelete]
        [Route("")]
        public IActionResult DeleteRoom([FromQuery]int roomId){
            try
            {
                int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                var roomname = RoomService.DeleteRoom(roomId, userId);
                return Ok(new Response{
                    status_code = 200,
                    message = "刪除成功 " + roomname + " 搶答室"
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
        #region 新增搶答室題目
        
        #endregion     
    }
}