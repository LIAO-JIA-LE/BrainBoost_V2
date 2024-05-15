using BrainBoost_V2.Service;
using BrainBoost_V2.Models;
using BrainBoost_V2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrainBoost_V2.Controller
{
    [Route("BrainBoost/[controller]")]
    public class QuestionController : ControllerBase
    {
        #region 呼叫函示
        private readonly QuestionService QuestionService;
        readonly UserService UserService;

        public QuestionController(QuestionService _questionService,UserService _UserService)
        {
            QuestionService = _questionService;
            UserService = _UserService;
        }
        #endregion

        #region 顯示問題
        // 獲得 單一問題
        [HttpGet]
        public Question Question([FromQuery]int question_id){
            return QuestionService.GetQuestionById(question_id);
        }

        // 獲得全部問題
        [HttpGet]
        [Route("AllQuestion")]
        public IActionResult AllQuestion([FromQuery]string search,[FromQuery]int type = 0,[FromQuery]int page = 1){
            AllQuestionViewModel data = new(){
                forpaging = new Forpaging(page),
                search = search
            };
            int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
            data.question = QuestionService.GetQuestionList(userId,type,data.search,data.forpaging);
            if(data.question.Count==0){
                return Ok(new Response(){
                    status_code = 204,
                    message = "無資料，請新增題目"
                });
            }
            return Ok(new Response(){
                status_code = 200,
                message = "讀取成功",
                data = data
            });
        }
        #endregion
        #region 修改題目
        // [HttpPut]
        // public IActionResult UpdateQuestion(){
            
        // }
        #endregion
        #region 刪除題目
        // [HttpDelete]
        // public IActionResult DeleteQuestion([FromQuery]int questionId){
            
        // }
        #endregion
    }

}