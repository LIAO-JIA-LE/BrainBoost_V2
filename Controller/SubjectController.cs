using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BrainBoost.Parameter;
using BrainBoost_V2.Models;
using BrainBoost_V2.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrainBoost_V2.Controller
{
    [Route("BrainBoost/[controller]")]
    public class SubjectController(SubjectService _subjectService, UserService _UserService) : ControllerBase
    {
        #region 呼叫函式
        readonly SubjectService SubjectService = _subjectService;
        readonly UserService UserService = _UserService;
        #endregion

        #region 科目
        // 查看該老師所有的科目
        [HttpGet]
        [Route("AllSubject")]
        public IActionResult GetAllSubject(){
            Response result;
            try{
                User user = UserService.GetDataByAccount(User.Identity.Name);
                result = new(){
                    status_code = 200,
                    data = SubjectService.GetAllSubject(user.userId)
                };
            }
            catch (Exception ex) {
                result = new(){
                    status_code = 400,
                    message = ex.Message
                };
            }
            return Ok(result);
        }
        //查詢單個科目
        [HttpGet]
        [Route("Subject")]
        public IActionResult GetSubject(int subjectId){
            Response result;
            try
            {
                User user = UserService.GetDataByAccount(User.Identity.Name);
                result = new(){
                    status_code = 200,
                    data = SubjectService.GetSubject(user.userId,subjectId)
                };
            }
            catch (Exception ex)
            {
                result = new(){
                    status_code = 400,
                    message = ex.Message
                };
            }
            return Ok(result);
        }

        // 新增科目
        [HttpPost]
        public IActionResult InsertSubject([FromBody]InsertSubject insertData){
            insertData.teacherId = UserService.GetDataByAccount(User.Identity.Name).userId;
            Response result;
            try{
                
                result = new(){
                    status_code = 200,
                    message = "新增成功",
                    data = SubjectService.InsertSubject(insertData)
                };
            }
            catch (Exception e){
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return Ok(result);
        }

        //修改科目名稱
        [HttpPut]
        public IActionResult UpdateSubject([FromQuery]int subjectId,[FromBody]string subjectContent){
            Response result;
            try
            {
                Subject subject = new(){
                                    subjectId = subjectId,
                                    subjectContent = subjectContent,
                                    userId = UserService.GetDataByAccount(User.Identity.Name).userId
                                };
                SubjectService.UpdateSubject(subject);
                result = new(){
                    status_code = 200,
                    message = "修改成功",
                    data = SubjectService.GetSubject(subject.userId,subject.subjectId)
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return Ok(result);
        }

        //刪除科目
        [HttpDelete]
        public IActionResult DeleteSubject([FromQuery]int subjectId){
            Response result;
            try
            {
                User user = UserService.GetDataByAccount(User.Identity.Name);
                if(SubjectService.GetSubject(user.userId,subjectId) != null){
                    SubjectService.DeleteSubject(user.userId,subjectId);
                    result = new(){
                        status_code = 200,
                        message = "刪除成功"
                    };
                }
                else result = new(){
                        status_code = 204,
                        message = "無此資料"
                    };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return Ok(result);
        }

        #endregion
    }
}