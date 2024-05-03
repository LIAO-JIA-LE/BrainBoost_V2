using Microsoft.AspNetCore.Mvc;
using BrainBoost_V2.Models;
using BrainBoost_V2.Parameter;
using BrainBoost_V2.ViewModels;
using BrainBoost_V2.Service;
using NPOI.SS.Formula.Functions;

namespace BrainBoost_V2.Controller
{
    [Route("BrainBoost/[controller]")]
    public class ImportController(QuestionService _questionService, UserService _userService) : ControllerBase
    {
        #region 呼叫函式

        readonly QuestionService QuestionService = _questionService;
        readonly UserService UserService = _userService;
        
        #endregion

        #region 是非題 手動匯入

        [HttpPost("[Action]")]
        public IActionResult TrueOrFalse([FromQuery]int subjectId, [FromBody]TureorFalse question){
            GetQuestion getQuestion = new();
            getQuestion.tagData.tagContent = question.tagContent;
            // 題目敘述
            getQuestion.questionData = new Question(){
                typeId = 1,
                subjectId = subjectId,
                questionLevel = question.questionLevel,
                questionContent = question.questionContent
            };

            // 題目答案
            getQuestion.answerData = new Answer(){
                answerContent = question.isAnswer ? "是" : "否",
                parse = question.parse
            };

            try
            {
                getQuestion.questionData.userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                getQuestion.questionData.questionId =  QuestionService.InsertQuestion(getQuestion);
            }
            catch (Exception e)
            {
                return BadRequest(new Response(){
                        status_code = Response.StatusCode,
                        message = $"發生錯誤:  {e}"
                    });
            }
            return Ok(new Response(){
                status_code = Response.StatusCode,
                message = "新增是非題成功",
                data = getQuestion
            });
        }

        #endregion

        #region 選擇題 手動匯入

        [HttpPost("[Action]")]
        public IActionResult MultipleChoice([FromQuery]int subjectId, [FromBody]MultipleChoice question){
            GetQuestion getQuestion = new();
            getQuestion.tagData.tagContent = question.tagContent;
            // 題目敘述
            getQuestion.questionData = new Question(){
                typeId = 2,
                subjectId = subjectId,
                questionLevel = question.questionLevel,
                questionContent = question.questionContent
            };

            // 題目選項
            getQuestion.options = new List<string>(){
                question.optionA.ToString(),
                question.optionB.ToString(),
                question.optionC.ToString(),
                question.optionD.ToString(),
            };

            // 題目答案
            getQuestion.answerData = new Answer(){
                answerContent = question.answerContent,
                parse = question.parse
            };

            try
            {
                getQuestion.questionData.userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                getQuestion.questionData.questionId =  QuestionService.InsertQuestion(getQuestion);
            }
            catch (Exception e)
            {
                return BadRequest(new Response(){
                        status_code = Response.StatusCode,
                        message = $"發生錯誤:  {e}"
                    });
            }
            return Ok(new Response(){
                status_code = Response.StatusCode,
                message = "新增是非題成功",
                data = getQuestion
            });
        }

        #endregion
        
    }
}