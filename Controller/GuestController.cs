using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpPost]
        public IActionResult GuestLogin(GuestLogin guestData){
            try
            {
                if(GuestService.PinCodeCheck(guestData.roomPinCode) > 0){
                    Guest guest = new(){
                        guestName = guestData.guestName,
                        classId = RoomService.GetClassByPinCode(guestData.roomPinCode),
                    };
                    guest = GuestService.InsertGuest(guest);
                    var guestJWT = JwtHelpers.GenerateGuestToken(guest.classId.ToString(),guestData.roomPinCode);
                    return Ok(new Response(){
                        status_code = 200,
                        message = "登入成功",
                        data = guestJWT
                    });
                }
                else
                    return BadRequest(new Response(){status_code = 400,message = "該搶答室不存在"});
            }
            catch (System.Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
    }
}