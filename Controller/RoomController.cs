using BrainBoost_V2.Models;
using BrainBoost_V2.Service;
using BrainBoost_V2.Parameter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrainBoost_V2.ViewModels;
using Back.ViewModels;
using BrainBoost.ViewModels;

namespace BrainBoost_V2.Controller
{
    [Route("BrainBoost/[controller]")]
    public class RoomController(UserService _userService,RoomService _roomService,QuestionService _questionService) : ControllerBase
    {
        readonly UserService UserService = _userService;
        readonly RoomService RoomService = _roomService;
        readonly QuestionService QuestionService = _questionService;

        #region 新增搶答室
        // 新增搶答室
        // 解題方式(roomFunction)預設為0(0:最後一次顯示,1:逐題解析)
        // 是否公開(roomPublic)預設為0(0:不公開,1:公開)
        // 時間限制(roomTimeLimit)預設為30秒
        [HttpPost]
        [Route("")]
        public IActionResult InsertRoom([FromBody]InsertRoom roomData){
            try{
                if(User.Identity.Name == null)
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "請先登入"
                    });
                //班級防呆、搶答室名稱防呆
                roomData.userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                if(RoomService.CheckRoom(roomData.roomName,roomData.userId))return BadRequest(new Response{status_code=400,message="該搶答室已存在"});
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
                        message = e.Message,
                        data = roomData
                    });
            }
        }

        #endregion
        #region 顯示搶答室
        //全部搶答室
        [HttpGet]
        [Route("AllRoom")]
        public IActionResult GetAllRoom([FromQuery]string search,[FromQuery]int classId,[FromQuery]bool used,[FromQuery]int page = 1){
            try
            {
                if(User.Identity.Name == null)
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "請先登入"
                    });
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
                if(User.Identity.Name == null)
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "請先登入"
                    });
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
                if(User.Identity.Name == null)
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "請先登入"
                    });
                int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                RoomQuestionListViewModel roomQuestionList = new()
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
        #region 刪除搶答室
        [HttpDelete]
        [Route("")]
        public IActionResult DeleteRoom([FromQuery]int roomId){
            try
            {
                if(User.Identity.Name == null)
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "請先登入"
                    });
                int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                if(RoomService.GetRoom(roomId,userId) == null)
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "您無權修改此搶答室或該搶答室以刪除",
                        data = roomId
                    });
                var roomname = RoomService.DeleteRoom(roomId, userId);
                return Ok(new Response{
                    status_code = 200,
                    message = "成功刪除 " + roomname + " 搶答室"
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
        #region 修改搶答室(資訊、題目)
        [HttpPut]
        [Route("")]
        public IActionResult UpdateRoom([FromBody]UpdateRoom UpdateData){
            try
            {
                if(User.Identity.Name == null)
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "請先登入"
                    });
                UpdateData.roomQuestion.userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                UpdateData.roomInfo.userId = UpdateData.roomQuestion.userId;
                if(RoomService.GetRoom(UpdateData.roomInfo.roomId,UpdateData.roomQuestion.userId) == null)
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "您無權修改此搶答室或該搶答室以刪除",
                        data = UpdateData
                    });
                RoomService.UpdateRoom(UpdateData.roomInfo);
                RoomService.UpdateRoomQuestion(UpdateData.roomQuestion);
                return Ok(new Response{
                    status_code = 200,
                    message = "修改成功",
                    data = UpdateData
                });
            }
            catch (Exception e)
            {
                return BadRequest(new Response{
                    status_code = 400 , 
                    message = e.Message ,
                    data = UpdateData
                });
            }
        }
        #endregion     
        #region 檢查搶答是是否存在
        [HttpGet]
        [Route("CheckRoom")]
        public IActionResult CheckRoom([FromQuery]string roomName){
            try{
                if(User.Identity == null || User.Identity.Name == null)
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "請先登入"
                    });
                int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                if(RoomService.CheckRoom(roomName,userId))return BadRequest(new Response{status_code=400,message="該搶答室已存在"});
                return Ok(new Response{status_code=200});
            }
            catch (Exception e){
                return BadRequest(new Response{
                    status_code = 400 ,
                    message = e.Message
                });
            }
        }
        #endregion

        #region 隨機出題
        [HttpGet("[Action]")]
        public IActionResult RandomQuestion([FromQuery]int roomId){
            try{
                var Response = RoomService.RandomQuestion(roomId);
                return Ok(new Response{
                    status_code = 200,
                    message = "顯示成功",
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

        #region 紀錄學生搶答室答案和分數
        [HttpPost]
        [Route("[Action]")]
        public IActionResult StudentResponse([FromBody]StudentResponse studentResponse){
            try{
                if(User.Identity.Name != null){
                    // 取得guestId
                    studentResponse.guestId = UserService.GetDataByGuestName(User.Identity.Name).guestId;
                    // 取得搶答室限時
                    studentResponse.timeLimit = RoomService.GetTimeLimitByRId(studentResponse.roomId);
                    // 取得此題目的答案
                    string Answer = QuestionService.GetQuestionAnswerByQId(studentResponse.questionId);
                    // 取得此題目的難度
                    int Level = QuestionService.GetQuestionLevel(studentResponse.questionId);
                    // 計分
                    if(studentResponse.timeCose < 3.0)
                        studentResponse.score = (decimal)Math.Round(studentResponse.timeLimit * Level, 1, MidpointRounding.AwayFromZero);
                    else
                        studentResponse.score = (decimal)Math.Round(studentResponse.timeLimit * Level);

                    RoomService.StorageTimers(Level, Answer, studentResponse);

                    return Ok(new Response{
                        status_code = 200,
                        message = "匯入成功",
                        data = studentResponse
                    });
                }
                else{
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "沒有name"
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

        #region 統整學生作答答案
        [HttpGet]
        [Route("[Action]")]
        public IActionResult StaticReseponse([FromQuery]int roomId, [FromQuery]int questionId){
            try{
                // 獲得選項
                List<string> option_content = QuestionService.GetOptionByQId(questionId);
                var Response = RoomService.GetStudentReseponse(roomId, questionId, option_content);
                if(Response != null){
                    return Ok(new Response{
                        status_code = 200,
                        message = "顯示成功",
                        data = Response
                    });
                }
                else{
                    return Ok(new Response{
                        status_code = 200,
                        message = "尚無資料"
                    });
                }
            }
            catch(Exception e){
                return BadRequest(new Response{
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        #endregion

        #region 記分板
        [HttpGet]
        [Route("[Action]")]
        public IActionResult ScoreBoard([FromQuery]int roomId){
            try{
                var Response = RoomService.GetScoreBoard(roomId);
                if(Response != null){
                    return Ok(new Response{
                        status_code = 200,
                        message = "顯示成功",
                        data = Response
                    });
                }
                else{
                    return Ok(new Response{
                        status_code = 200,
                        message = "尚無資料"
                    });
                }
            }
            catch(Exception e){
                return BadRequest(new Response{
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        #endregion

        #region 記分板
        [HttpGet]
        [Route("[Action]")]
        public IActionResult ScoreByGuest([FromQuery]int roomId){
            try{
                if(User.Identity != null){
                    int guestId = UserService.GetDataByGuestName(User.Identity.Name).guestId;
                    var Response = RoomService.GetScoreByGuest(roomId, guestId);

                    return Ok(new Response{
                        status_code = 200,
                        message = "顯示成功",
                        data = Response
                    });
                }
                else{
                    return BadRequest(new Response{
                        status_code = 400,
                        message = "未進入搶答室"
                    });
                }
            }
            catch(Exception e){
                return BadRequest(new Response{
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        #endregion
    }
}