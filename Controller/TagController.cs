// using System.Data;
// using BrainBoost.Parameter;
// using BrainBoost_V2.Models;
// using BrainBoost_V2.Parameter;
// using BrainBoost_V2.Service;
// using BrainBoost_V2.ViewModels;
// using Microsoft.AspNetCore.Mvc;

// namespace BrainBoost_V2.Controller
// {
//     [Route("[controller]")]
//     public class TagController(IWebHostEnvironment _evn,QuestionService _questionService,UserService _userService) : ControllerBase
//     {
//         readonly QuestionService QuestionService = _questionService;
//         readonly UserService UserService = _userService;
//         readonly IWebHostEnvironment evn = _evn;
//         public class ExcelForm{
//             public int subjectId{get;set;}
//             public IFormFile ExcelFile{get;set;}
//             public IFormFile? optionA{get;set;}
//             public IFormFile? optionB{get;set;}
//             public IFormFile? optionC{get;set;}
//             public IFormFile? optionD{get;set;}
//         }
//         public class GetQuestion_Excel
//         {
//             public Question questionData { get; set; } = new();
//             public Subject subjectData{get;set;} = new();
            
//             public List<options> options { get; set; }

//             public Answer answerData { get; set; } = new();

//             public Tag tagData{ get; set; } = new();
//         }
//         public class options{
//             public string content{get;set;}
//             public string route{get;set;}
//         }
//         #region 選擇題 檔案匯入
//         [HttpPost("[Action]")]
//         public IActionResult Excel_MultipleChoice([FromForm]ExcelForm FormData)
//         {
//             // 檔案處理
//             DataTable dataTable = QuestionService.FileDataPrecess(FormData.ExcelFile);
//             var wwwroot = evn.ContentRootPath + @"\wwwroot\images\";
//             int userId;
//             // 將dataTable資料匯入資料庫
//             if(User.Identity.Name != null){
//                 userId = UserService.GetDataByAccount(User.Identity.Name).userId;
//             }
//             else return BadRequest(new Response(){status_code = 400,message = "請先登入"});
//             // 將dataTable資料匯入資料庫
//             if(dataTable != null){
//                 List<GetQuestion_Excel> AllQuestion = [];
//                 foreach (DataRow dataRow in dataTable.Rows)
//                 {
//                     GetQuestion_Excel getQuestion = new();
//                     getQuestion.tagData.tagContent = dataRow["Tag"].ToString();
//                         getQuestion.subjectData= new(){
//                                                         userId = userId,
//                                                         subjectId = FormData.subjectId
//                                                     };
//                     // 題目敘述
//                     getQuestion.questionData = new Question(){
//                         userId = userId,
//                         typeId = 2,
//                         questionLevel = Convert.ToInt32(dataRow["Level"]),
//                         questionContent = dataRow["Question"].ToString()
//                     };
//                     //處理圖片
//                     // if(Data.file.Length > 0){
//                     //     var imgname = User.Identity.Name + ".jpg";
//                     //     var img_path = wwwroot + imgname;
//                     //     using var stream = System.IO.File.Create(img_path);
//                     //     Data.file.CopyTo(stream);
//                     //     user.userPhoto = img_path;
//                     // }
//                     // 題目選項
//                     getQuestion.options.Add(new options{content = dataRow["OptionA"].ToString(), route = });
//                     // content=dataRow["OptionA"].ToString(),
//                     // dataRow["OptionB"].ToString(),
//                     // dataRow["OptionC"].ToString(),
//                     // dataRow["OptionD"].ToString()
                    

//                     getQuestion.answerData = new Answer()
//                     {
//                         answerContent = dataRow["Answer"].ToString(),
//                         parse = dataRow["Parse"].ToString()
//                     };

//                     try
//                     {
//                         QuestionService.InsertQuestion(getQuestion);
//                         getQuestion.answerData.questionId = getQuestion.questionData.questionId;
//                         AllQuestion = [.. AllQuestion, getQuestion];
//                     }
//                     catch (Exception e)
//                     {
//                         return BadRequest(new Response(){
//                             status_code = Response.StatusCode,
//                             message = $"發生錯誤:  {e}"
//                         });
//                     }
//                 }
//                 return Ok(new Response(){
//                     status_code = Response.StatusCode,
//                     message = "匯入選擇題成功",
//                     data = AllQuestion
//                 });
//             }
//             else return BadRequest(new Response(){
//                                                     status_code = 400,
//                                                     message = "資料表無資料",
//                                                 });
//         }
//         #endregion
//     }
// }