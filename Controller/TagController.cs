using System.Data;
using BrainBoost.Parameter;
using BrainBoost_V2.Models;
using BrainBoost_V2.Parameter;
using BrainBoost_V2.Service;
using BrainBoost_V2.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BrainBoost_V2.Controller
{
    [Route("BrainBoost/[controller]")]
    public class TagController(QuestionService _questionService,UserService _userService,TagService _tagService) : ControllerBase
    {
        readonly QuestionService QuestionService = _questionService;
        readonly UserService UserService = _userService;
        readonly TagService TagService = _tagService;

        [HttpGet]
        [Route("AllTag")]
        public IActionResult GetAllTag([FromQuery]int subjectId){
            try{
                if(User.Identity.Name == null) return BadRequest(new Response{status_code = 400, message = "請先登入"});
                // if(subjectId == 0) return BadRequest(new Response{status_code = 400, message = "請先輸入科目編號"});
                int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                List<Tag> tagList = TagService.GetAllTag(userId,subjectId);
                return Ok(new Response{
                    status_code = 200,
                    message = "讀取成功",
                    data = tagList
                });
            }
            catch(Exception e){
                return BadRequest(new Response{status_code = 400 , message = e.Message});
            }
        }
    }
}