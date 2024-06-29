using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Back.Parameter;
using BrainBoost_V2.Models;
using BrainBoost_V2.Parameter;
using BrainBoost_V2.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrainBoost_V2.Controller
{
    [Route("BrainBoost/[controller]")]
    public class GuestController(GuestService _guestService,RoomService _roomService,JwtHelpers _jwtHelpers) : ControllerBase
    {
        readonly GuestService GuestService = _guestService;
        readonly RoomService RoomService = _roomService;
        readonly JwtHelpers JwtHelpers = _jwtHelpers;

        #region 訪客進入搶答室
        [HttpPost]
        [Route("[Action]")]
        public IActionResult GuestLogin([FromForm]GuestLogin guestData){
            try
            {
                if(GuestService.PinCodeCheck(guestData.roomPinCode)){
                    Guest guest = new(){
                        guestName = guestData.guestName,
                        roomId = RoomService.GetRoomIdByPinCode(guestData.roomPinCode),
                    };
                    // 訪客新增
                    guest = GuestService.InsertGuest(guest);
                    var guestJWT = JwtHelpers.GenerateToken(guestData.guestName, 5);
                    return Ok(new Response(){
                        status_code = 200,
                        message = "進入成功",
                        data = guestJWT
                    });
                }
                else
                    return BadRequest(new Response(){
                        status_code = 400,
                        message = "進入失敗，請重新輸入邀請碼！"
                    });
            }
            catch (Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        #endregion

        #region 顯示進入搶答室的訪客列表
        [HttpGet]
        [Route("[Action]")]
        public IActionResult GetGuestList([FromQuery]int roomId){
            try{
                GuestList guestList = new(){
                    roomId = roomId,
                    guestNameList = GuestService.GetGuestListByRoomId(roomId),
                    guestCount = GuestService.GetGuestCountByRoomId(roomId)
                };
                return Ok(new Response(){
                    status_code = 200,
                    message = "獲取成功",
                    data = guestList
                });
            }
            catch (Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        #endregion

        #region 訪客回應

        #endregion
    }
}