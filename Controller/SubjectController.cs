using BrainBoost.Parameter;
using BrainBoost_V2.Models;
using BrainBoost_V2.Parameter;
using BrainBoost_V2.Service;
using BrainBoost_V2.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        // 分頁拿掉
        // [HttpGet]
        // [Route("AllSubject")]
        // public IActionResult GetAllSubject([FromQuery]string search,[FromQuery]int page = 1){
        //     try{
        //         if(User.Identity == null || User.Identity.Name == null) return BadRequest(new Response(){status_code = 400, message = "請先登入"});
        //         AllSubjectViewModel AllSubjectViewModel = new(){
        //                                                 forpaging = new Forpaging(page),
        //                                                 userId = UserService.GetDataByAccount(User.Identity.Name).userId,
        //                                                 search = search
        //                                             };
        //         AllSubjectViewModel.subjectList = SubjectService.GetAllSubject(AllSubjectViewModel);
        //         return Ok(new Response(){
        //                                 status_code = 200,
        //                                 message = "讀取成功",
        //                                 data = AllSubjectViewModel
        //                             });
        //     }
        //     catch (Exception e) {
        //         return BadRequest(new Response(){
        //                                 status_code = 400,
        //                                 message = e.Message
        //                             });
        //     }
        // }
        [HttpGet]
        [Route("AllSubject")]
        public IActionResult GetAllSubject([FromQuery]string search){
            try{
                if(User.Identity == null || User.Identity.Name == null) 
                    return BadRequest(
                        new Response(){
                            status_code = 400,
                            message = "請先登入"
                        });
                AllSubjectViewModel AllSubjectViewModel = new(){
                        userId = UserService.GetDataByAccount(User.Identity.Name).userId,
                        search = search
                    };
                AllSubjectViewModel.subjectList = SubjectService.GetAllSubject(AllSubjectViewModel);
                if(AllSubjectViewModel.subjectList.Count > 0){
                    return Ok(new Response(){
                                            status_code = 200,
                                            message = "讀取成功",
                                            data = AllSubjectViewModel
                                        });
                }
                else return Ok(new Response{status_code = 204, message="無資料"}) ;
            }
            catch (Exception e) {
                return BadRequest(new Response(){
                                        status_code = 400,
                                        message = e.Message
                                    });
            }
        }
        //查詢單個科目
        [HttpGet]
        [Route("")]
        public IActionResult GetSubject([FromQuery]int subjectId){
            try
            {
                if(User.Identity == null || User.Identity.Name == null) return BadRequest(new Response(){status_code = 400, message = "請先登入"});
                Subject subject = new(){
                                        userId = UserService.GetDataByAccount(User.Identity.Name).userId,
                                        subjectId = subjectId
                                    };
                SubjectTagViewModel subjectTagViewModel = SubjectService.GetSubject(subject.userId,subject.subjectId);
                if(subjectTagViewModel.subject != null){  //
                    return Ok(new Response(){
                                            status_code = 200,
                                            message = "讀取成功",
                                            data = subjectTagViewModel
                                        });
                }
                else
                    return Ok(new Response(){
                                            status_code = 204,
                                            message = "查無科目或您無此權限"
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

        // 新增科目
        [HttpPost]
        public IActionResult InsertSubject([FromBody]InsertSubject insertData){
            Response result;
            try{
                if(User.Identity == null || User.Identity.Name == null) return BadRequest(new Response{status_code = 400, message = "請先登入"});
                insertData.userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                if(SubjectService.CheckSubject(insertData)) return BadRequest(new Response{status_code = 400, message = "該科目已存在"});
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
        public IActionResult UpdateSubject([FromBody]UpdateSubject updateSubject){
            try
            {
                if(User.Identity == null || User.Identity.Name == null) return BadRequest(new Response(){status_code = 400, message = "請先登入"});
                Subject subject = new(){
                                    subjectId = updateSubject.subjectId,
                                    subjectContent = updateSubject.subjectContent,
                                    userId = UserService.GetDataByAccount(User.Identity.Name).userId
                                };
                SubjectService.UpdateSubject(subject);
                SubjectTagViewModel newSubjectViewModel = SubjectService.GetSubject(subject.userId,subject.subjectId);
                string newSubjectContnet = newSubjectViewModel.subject.subjectContent;
                return Ok(new Response{
                    status_code = 200,
                    message = "已將該班級名稱修改為" + newSubjectContnet
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

        //刪除科目
        [HttpDelete]
        public IActionResult DeleteSubject([FromQuery]int subjectId){
            Response result;
            try
            {
                if(User.Identity.Name == null)
                    return BadRequest(new Response{status_code = 400,message = "請先登入"});
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