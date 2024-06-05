using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;
using BrainBoost_V2.Parameter;
using BrainBoost_V2.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.Util;

namespace BrainBoost_V2.Controller
{
    [Route("BrainBoost/[controller]")]
    public class ClassController(UserService _userService,ClassService _classService) : ControllerBase
    {
        private readonly ClassService classService = _classService;
        private readonly UserService userService = _userService;
        #region 班級
        //新增班級
        [HttpPost]
        [Route("")]
        // [Authorize(Roles = "Teacher,Manager,Admin")]
        public IActionResult InsertClass([FromBody]InsertClass insertClass){
            try
            {
                //是否有登入
                if(User.Identity.Name == null)
                    return BadRequest(new Response(){status_code = 400, message = "請先登入"});
                insertClass.userId = userService.GetDataByAccount(User.Identity.Name).userId;
                //是否有班級存在
                if(classService.CheckClass(insertClass))
                    return BadRequest(new Response(){status_code = 400, message = "該班級已存在"});
                int classId = classService.InsertClass(insertClass);
                return Ok(new Response(){
                    status_code = 200,
                    message = "新增成功",
                    data = classService.GetClass(classId,insertClass.userId)
                });
            }
            catch (System.Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        //取得班級列表
        [HttpGet]
        [Route("")]
        public IActionResult GetClass([FromQuery]int classId,[FromQuery]string search){
            try
            {
                if(User.Identity == null || User.Identity.Name == null) return BadRequest(new Response(){status_code = 400, message = "請先登入"});
                int userId = userService.GetDataByAccount(User.Identity.Name).userId;
                if(classId != 0 && string.IsNullOrEmpty(search)){
                    return Ok(new Response(){
                        status_code = 200,
                        message = "讀取成功",
                        data = classService.GetClass(classId,userId)
                    });
                }
                List<Class> Class_List = classService.GetAllClass(userId,search);
                return Ok(new Response(){
                    status_code = 200,
                    message = @"讀取成功,所有班級",
                    data = Class_List
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
        // [HttpGet]
        // [Route("")]
        // public IActionResult GetClass([FromQuery]int classId){
        //     try
        //     {
        //         int userId = userService.GetDataByAccount(User.Identity.Name).userId;
        //         if(classId != 0){
        //             return Ok(new Response(){
        //                 status_code = 200,
        //                 message = "讀取成功",
        //                 data = classService.GetClass(classId,userId)
        //             });
        //         }
        //         List<Class> Class_List = classService.GetAllClass(userId,"");
        //         return Ok(new Response(){
        //             status_code = 200,
        //             message = @"讀取成功,所有班級",
        //             data = Class_List
        //         });
                
        //     }
        //     catch (System.Exception e)
        //     {
        //         return BadRequest(new Response(){
        //             status_code = 400,
        //             message = e.Message
        //         });
        //     }
        // }
        //刪除班級
        [HttpDelete]
        [Route("")]
        public IActionResult DeleteClass(DeleteClass deleteData){
            try
            {
                if(User.Identity == null || User.Identity.Name == null) return BadRequest(new Response(){status_code = 400, message = "請先登入"});
                classService.DeleteClass(deleteData);
                return Ok(new Response(){
                    status_code = 200,
                    message = "刪除成功"
                });
            }
            catch (System.Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        //修改班級資訊
        [HttpPut]
        [Route("")]
        public IActionResult UpdateClass([FromBody]UpdateClass updateData){
            try
            {
                if(User.Identity == null || User.Identity.Name == null) return BadRequest(new Response(){status_code = 400, message = "請先登入"});
                updateData.userId = userService.GetDataByAccount(User.Identity.Name).userId;
                if(classService.CheckClassById(updateData.classId,updateData.userId))
                    classService.UpdateClass(updateData);
                else
                    return BadRequest(new Response(){status_code = 400,message = "班級不存在"});
                if(string.IsNullOrEmpty(updateData.className)) return BadRequest(new Response{status_code=400,message = "請輸入班級名稱"});
                return Ok(new Response(){
                    status_code = 200,
                    message = "修改成功",
                    data = classService.GetClass(updateData.classId,updateData.userId)
                });
            }
            catch (System.Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }    
        }
        #endregion
    }
}