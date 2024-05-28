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
    public class ImportController(QuestionService _questionService, UserService _userService, SubjectService _subjectService) : ControllerBase
    {
        #region 呼叫函式

        readonly SubjectService SubjectService = _subjectService;
        readonly QuestionService QuestionService = _questionService;
        readonly UserService UserService = _userService;

        #endregion

        #region 是非題 手動匯入

        [HttpPost("[Action]")]
        public IActionResult TrueOrFalse([FromQuery] int subjectId, [FromBody] TureorFalse question)
        {
            GetQuestion getQuestion = new();
            getQuestion.tagData.tagContent = question.tagContent;
            // 使用者防呆
            int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
            if (SubjectService.GetSubject(userId, subjectId) == null)
                return BadRequest(new Response
                {
                    status_code = 400,
                    message = "您非此科目擁有者"
                });
            getQuestion.subjectData.subjectId = subjectId;
            // 題目敘述
            getQuestion.questionData = new Question()
            {
                typeId = 1,
                questionLevel = question.questionLevel,
                questionContent = question.questionContent
            };

            // 題目答案
            getQuestion.answerData = new Answer()
            {
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
                return BadRequest(new Response()
                {
                    status_code = 400,
                    message = $"發生錯誤:  {e}"
                });
            }
            return Ok(new Response()
            {
                status_code = 200,
                message = "新增是非題成功",
                data = getQuestion
            });
        }

        #endregion

        #region 選擇題 手動匯入

        [HttpPost("[Action]")]
        public IActionResult MultipleChoice([FromQuery] int subjectId, [FromBody] MultipleChoice question)
        {
            GetQuestion getQuestion = new();
            getQuestion.tagData.tagContent = question.tagContent;
            getQuestion.subjectData.subjectId = subjectId;
            // 題目敘述
            getQuestion.questionData = new Question()
            {
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
            getQuestion.answerData = new Answer()
            {
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
                return BadRequest(new Response()
                {
                    status_code = Response.StatusCode,
                    message = $"發生錯誤:  {e}"
                });
            }
            return Ok(new Response()
            {
                status_code = Response.StatusCode,
                message = "新增選擇題成功",
                data = getQuestion
            });
        }

        #endregion

        #region 是非題 檔案匯入
        [HttpPost("[Action]")]
        public IActionResult Excel_TrueOrFalse([FromForm] int subjectId, [FromForm] List<IFormFile> files)
        {

            List<GetQuestion> AllQuestion = [];
            int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
            // 防呆：是否已登入
            if (userId == null)
                return BadRequest(new Response() { status_code = 400, message = "請先登入" });
                
            foreach (var file in files)
            {
                if (file.FileName.ToUpper().EndsWith("XLSX"))
                {
                    // 檔案處理
                    DataTable dataTable = QuestionService.FileDataPrecess(file);

                    // 將dataTable資料匯入資料庫
                    if (dataTable != null)
                    {
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            GetQuestion getQuestion = new();
                            getQuestion.tagData.tagContent = dataRow["Tag"].ToString();
                            getQuestion.subjectData = new()
                            {
                                userId = userId,
                                subjectId = subjectId
                            };

                            // 題目敘述
                            getQuestion.questionData = new Question()
                            {
                                userId = getQuestion.subjectData.userId,
                                typeId = 1,
                                questionLevel = Convert.ToInt32(dataRow["Level"]),
                                questionContent = dataRow["Question"].ToString(),
                                // questionPicture = dataRow["QuestionImg"].ToString()
                            };

                            // 題目答案
                            getQuestion.answerData = new Answer()
                            {
                                answerContent = dataRow["Answer"].ToString(),
                                parse = dataRow["Parse"].ToString()
                            };

                            try
                            {
                                QuestionService.InsertQuestion(getQuestion);
                                getQuestion.answerData.questionId = getQuestion.questionData.questionId;
                                AllQuestion = [.. AllQuestion, getQuestion];
                            }
                            catch (Exception e)
                            {
                                return BadRequest(new Response()
                                {
                                    status_code = Response.StatusCode,
                                    message = $"發生錯誤:  {e}"
                                });
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(new Response()
                        {
                            status_code = 400,
                            message = "資料表無資料",
                        });
                    }
                }
                else
                {
                    return BadRequest(new Response()
                    {
                        status_code = Response.StatusCode,
                        message = "檔案格式應為xlsx，請重新匯入" + file.FileName + "！"
                    });
                }
            }
            return Ok(new Response()
            {
                status_code = Response.StatusCode,
                message = "新增是非題成功",
                data = AllQuestion
            });
        }
        #endregion

        #region 選擇題 檔案匯入
        [HttpPost("[Action]")]
        public IActionResult Excel_MultipleChoice([FromForm] int subjectId, [FromForm] List<IFormFile> files)
        {
            List<GetQuestion> AllQuestion = [];
            int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
            // 防呆：是否已登入
            if (userId == null)
                return BadRequest(new Response() { status_code = 400, message = "請先登入" });

            foreach (var file in files)
            {
                if (file.FileName.ToUpper().EndsWith("XLSX"))
                {
                    // 檔案處理
                    DataTable dataTable = QuestionService.FileDataPrecess(file);

                    // 將dataTable資料匯入資料庫
                    if (dataTable != null)
                    {

                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            GetQuestion getQuestion = new();
                            getQuestion.tagData.tagContent = dataRow["Tag"].ToString();
                            getQuestion.subjectData = new()
                            {
                                userId = userId,
                                subjectId = subjectId
                            };
                            // 題目敘述
                            getQuestion.questionData = new Question()
                            {
                                userId = getQuestion.subjectData.userId,
                                typeId = 2,
                                questionLevel = Convert.ToInt32(dataRow["Level"]),
                                questionContent = dataRow["Question"].ToString(),
                                // questionPicture = dataRow["QuestionImg"].ToString()
                            };

                            // 題目選項
                            getQuestion.options = new List<string>(){
                                dataRow["OptionA"].ToString(),
                                dataRow["OptionB"].ToString(),
                                dataRow["OptionC"].ToString(),
                                dataRow["OptionD"].ToString()
                            };

                            // getQuestion.optionImg = new List<string>(){
                            //     dataRow["OptionA_Img"].ToString(),
                            //     dataRow["OptionB_Img"].ToString(),
                            //     dataRow["OptionC_Img"].ToString(),
                            //     dataRow["OptionD_Img"].ToString()
                            // };

                            getQuestion.answerData = new Answer()
                            {
                                answerContent = dataRow["Answer"].ToString(),
                                parse = dataRow["Parse"].ToString()
                            };

                            try
                            {
                                QuestionService.InsertQuestion(getQuestion);
                                getQuestion.answerData.questionId = getQuestion.questionData.questionId;
                                AllQuestion = [.. AllQuestion, getQuestion];
                            }

                            catch (Exception e)
                            {
                                return BadRequest(new Response()
                                {
                                    status_code = Response.StatusCode,
                                    message = $"發生錯誤:  {e}"
                                });
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(new Response()
                        {
                            status_code = 400,
                            message = "資料表無資料",
                        });
                    }
                }
                else
                {
                    return BadRequest(new Response()
                    {
                        status_code = Response.StatusCode,
                        message = "檔案格式應為xlsx，請重新匯入" + file.FileName + "！"
                    });
                }

            }
            return Ok(new Response()
            {
                status_code = Response.StatusCode,
                message = "匯入選擇題成功",
                data = AllQuestion
            });
        }
        #endregion
    }
}