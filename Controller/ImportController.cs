using Microsoft.AspNetCore.Mvc;
using BrainBoost_V2.Models;
using BrainBoost_V2.Parameter;
using BrainBoost_V2.ViewModels;
using BrainBoost_V2.Service;
using System.Data;

namespace BrainBoost_V2.Controller
{
    [Route("BrainBoost/[controller]")]
    [ApiController]
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
            getQuestion.subjectData.subjectId = subjectId;
            // 題目敘述
            getQuestion.questionData = new Question(){
                typeId = 1,
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
                QuestionService.InsertQuestion(getQuestion);
                getQuestion.answerData.questionId = getQuestion.questionData.questionId;
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
            getQuestion.subjectData.subjectId = subjectId;
            // 題目敘述
            getQuestion.questionData = new Question(){
                typeId = 2,
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
                QuestionService.InsertQuestion(getQuestion);
                getQuestion.answerData.questionId = getQuestion.questionData.questionId;
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
                message = "新增選擇題成功",
                data = getQuestion
            });
        }

        #endregion

        #region 是非題 檔案匯入
        [HttpPost("[Action]")]
        public IActionResult Excel_TrueOrFalse([FromQuery]int subjectId, List<IFormFile> files){
            if (files == null || files.Count == 0){
                return BadRequest(new Response(){
                        status_code = Response.StatusCode,
                        message = "檔案無資料",
                    });
            }
            else{
                List<GetQuestion> processedQuestions = new List<GetQuestion>();

                foreach(var file in files){
                    if (file.Length == 0) continue;
                    // 檔案處理
                    DataTable dataTable = QuestionService.FileDataPrecess(file);
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        GetQuestion getQuestion = new();
                        getQuestion.tagData.tagContent = dataRow["Tag"].ToString();
                        getQuestion.subjectData.subjectId = subjectId;
                        // 題目敘述
                        getQuestion.questionData = new Question(){
                            typeId = 1,
                            questionPicture = dataRow["QuestionImg"] as byte[],
                            questionLevel = Convert.ToInt32(dataRow["Level"]),
                            questionContent = dataRow["Question"].ToString()
                        };

                        // 題目答案
                        getQuestion.answerData = new Answer(){
                            answerContent = dataRow["Answer"].ToString(),
                            parse = dataRow["Parse"].ToString()
                        };

                        try
                        {
                            int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                            QuestionService.InsertQuestion(getQuestion);
                            getQuestion.answerData.questionId = getQuestion.questionData.questionId;
                            processedQuestions.Add(getQuestion);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"發生錯誤:  {e}");
                        }
                    }
                    if (processedQuestions.Count > 0)
                    {
                        return Ok(new Response()
                        {
                            status_code = Response.StatusCode,
                            message = $"{processedQuestions.Count} 道是非題已成功新增",
                            data = processedQuestions
                        });
                    }
                    else
                    {
                        return BadRequest(new Response()
                        {
                            status_code = Response.StatusCode,
                            message = "檔案無資料或發生錯誤",
                        });
                    }
                }
                }
            return Ok(new Response(){
                        status_code = Response.StatusCode,
                        message = "檔案無資料",
                    });
        }
        #endregion

        #region 選擇題 檔案匯入
        [HttpPost("[Action]")]
        public IActionResult Excel_MultipleChoice([FromQuery]int subjectId, IFormFile file)
        {
            // 檔案處理
            DataTable dataTable = QuestionService.FileDataPrecess(file);
            // 將dataTable資料匯入資料庫
            foreach (DataRow dataRow in dataTable.Rows)
            {
                GetQuestion getQuestion = new();
                getQuestion.tagData.tagContent = dataRow["Tag"].ToString();
                    getQuestion.subjectData.subjectId = subjectId;
                // 題目敘述
                getQuestion.questionData = new Question(){
                    typeId = 2,
                    questionLevel = Convert.ToInt32(dataRow["Level"]),
                    questionContent = dataRow["Question"].ToString()
                };

                // 題目選項
                getQuestion.options = new List<string>(){
                    dataRow["OptionA"].ToString(),
                    dataRow["OptionB"].ToString(),
                    dataRow["OptionC"].ToString(),
                    dataRow["OptionD"].ToString()
                };

                getQuestion.answerData = new Answer()
                {
                    answerContent = dataRow["Answer"].ToString(),
                    parse = dataRow["Parse"].ToString()
                };

                try
                {
                    int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                    QuestionService.InsertQuestion(getQuestion);
                    getQuestion.answerData.questionId = getQuestion.questionData.questionId;
                }
                catch (Exception e)
                {
                    return BadRequest(new Response(){
                        status_code = Response.StatusCode,
                        message = $"發生錯誤:  {e}"
                    });
                }
            }
            return Ok(new Response(){
                status_code = Response.StatusCode,
                message = "匯入選擇題成功"
            });    
        }
        #endregion
        
    }
}