using BrainBoost_V2.Service;
using BrainBoost_V2.Models;
using BrainBoost_V2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrainBoost_V2.Parameter;
using back.ViewModels;

namespace BrainBoost_V2.Controller
{
    [Route("BrainBoost/[controller]")]
    public class QuestionController : ControllerBase
    {
        #region 呼叫函示
        private readonly QuestionService QuestionService;
        readonly UserService UserService;
        readonly IWebHostEnvironment evn;

        public QuestionController(QuestionService _questionService,UserService _UserService,IWebHostEnvironment _evn)
        {
            QuestionService = _questionService;
            UserService = _UserService;
            evn = _evn;
        }
        #endregion

        #region 顯示問題
        // 獲得 單一問題
        [HttpGet]
        public IActionResult Question([FromQuery]int questionId){
            // return QuestionService.GetQuestionById(question_id);
            try
            {
                if(User.Identity.Name == null ) 
                    return BadRequest(new Response{status_code = 400 , message = "請先登入"});
                int userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                QuestionViewModel question = QuestionService.GetQuestionViewModelById(userId,questionId);
                if(question == null) return BadRequest(new Response{status_code = 400, message = "您無權查看此題或無資料"});
                return Ok(new Response{
                    status_code = 200,
                    message = "讀取成功",
                    data = question
                });
            }
            catch (Exception e)
            {
                return BadRequest(new Response{status_code = 400, message = e.Message});
            }
        }

        // 獲得全部問題
        [HttpGet]
        [Route("AllQuestion")]
        public IActionResult AllQuestion([FromQuery]string search,[FromQuery]searchQuestion searchQuestion,[FromQuery]int type = 0,[FromQuery]int page = 1){
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

        //新增搶答室的題庫列表
        public class searchQuestion{
            public int? userId { get; set; }
            public string? search{ get; set; }
            public int? typeId{ get; set; }
            public int? questionLevel{ get; set; }
            public int? tagId{ get; set; }
            public int? subjectId{ get; set; }
        }
        [HttpGet]
        [Route("QuestionList")]
        public IActionResult QuestionList([FromQuery]searchQuestion searchQuestion){
            try
            {
                if(User.Identity.Name == null) return BadRequest(new Response{status_code = 400, message = "請先登入"});
                if(searchQuestion == null)  return BadRequest(new Response{status_code = 400, message = "請輸入篩選內容"});
                searchQuestion.userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                List<Question> questionList = QuestionService.GetQuestionList(searchQuestion);
                if(questionList == null) return Ok(new Response{status_code = 204, message = "查無資料", data = questionList});
                return Ok(new Response{status_code = 200, message = "讀取成功", data = questionList});
            }
            catch (Exception e)
            {
                return BadRequest(new Response{status_code = 400 , message = e.Message});
            }
        }
        #endregion
        #region 修改題目
        [HttpPut]
        public IActionResult UpdateQuestion([FromForm]UpdateQuestion UpdateData){
            try
            {
                // if(User.Identity.Name == null ) 
                //     return BadRequest(new Response{status_code = 400 , message = "請先登入"});
                // else{
                    // UpdateData.userId = UserService.GetDataByAccount(User.Identity.Name).userId;
                    Question question = QuestionService.GetQuestionById(UpdateData.questionId);
                    if(question == null || question.userId != UpdateData.userId) 
                        return BadRequest(new Response{status_code=400 , message = "您無權更改此題或無此題"});
                    QuestionViewModel questionViewModel = new(){
                        questionId = UpdateData.questionId,
                        userId  = UpdateData.userId ,
                        typeId  = UpdateData.typeId ,
                        questionContent  = UpdateData.questionContent ,
                        questionLevel  = UpdateData.questionLevel ,
                        answerContent  = UpdateData.answerContent ,
                        parse  = UpdateData.parse ?? "",
                        // options = []
                    };
                    // 選項陣列處理
                    List<Option> options = QuestionService.GetQuestionOptionByqId(questionViewModel.questionId);
                    // 處理圖片
                    var wwwroot = evn.ContentRootPath + @"\wwwroot\images\option\" + UpdateData.questionId.ToString() + "\\";
                    if (!Directory.Exists(wwwroot))
                        Directory.CreateDirectory(wwwroot);
                    
                    if( UpdateData.typeId == 2 && UpdateData.optionPicture != null ){
                        int i = 0;
                        foreach(var optionPicture in UpdateData.optionPicture){
                            var imgname = options[i].optionId.ToString() + ".jpg";//
                            var img_path = wwwroot + imgname;
                            using var stream = System.IO.File.Create(img_path);
                            optionPicture.CopyTo(stream);
                            questionViewModel.options.Add(new(){optionContent = UpdateData.optionContent[i],optionPicture = img_path});
                            i++;
                        }
                    }
                    else
                       foreach(var optionContent in UpdateData.optionContent){
                            questionViewModel.options.Add(new(){optionContent = optionContent});
                        }
                       
                    QuestionService.UpdateQuestion(questionViewModel);
                    //包Response資料
                    return Ok(new Response{status_code = 200, message = "修改成功",data = questionViewModel});
                // }
            }
            catch (System.Exception e)
            {
                
                return BadRequest(new Response{
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        #endregion
        #region 刪除題目
        // [HttpDelete]
        // public IActionResult DeleteQuestion([FromQuery]int questionId){
        //     try
        //     {
        //         QuestionService.DeleteQuestion(questionId);
        //     }
        //     catch (Exception e)
        //     {
        //         return BadRequest(new Response{
        //                                     status_code = 400 , 
        //                                     message = e.Message
        //                                     });
        //     }
        // }
        #endregion
    }

}