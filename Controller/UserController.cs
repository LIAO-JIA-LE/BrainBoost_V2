using BrainBoost_V2.Services;
using BrainBoost_V2.Models;
using BrainBoost_V2.Service;
using BrainBoost_V2.Parameter;
using Microsoft.AspNetCore.Mvc;
using BrainBoost_V2.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace BrainBoost_V2.Controller
{
    [Route("BrainBoost/[controller]")]
    public class UserController(IWebHostEnvironment _evn,UserService _userService,MailService _mailservice, JwtHelpers _jwtHelpers,Forpaging _forpaging,RoleService _roleservice) : ControllerBase
    {
        #region 呼叫函式
        readonly UserService UserService = _userService;
        readonly MailService MailService = _mailservice;
        readonly JwtHelpers JwtHelpers = _jwtHelpers;
        readonly Forpaging Forpaging = _forpaging;
        readonly RoleService RoleService = _roleservice;
        readonly IWebHostEnvironment evn = _evn;

        #endregion

        #region 註冊
        // 註冊 
        [HttpPost("[Action]")]
        public IActionResult Register([FromBody]UserRegister RegisterData)
        {
            if (ModelState.IsValid)
            {
                var validatestr = UserService.RegisterCheck(RegisterData.userAccount,RegisterData.userEmail);
                if (!string.IsNullOrEmpty(validatestr)){
                    return Ok(new Response(){
                        status_code = 400,
                        message = validatestr
                    });
                }
                var wwwroot = evn.ContentRootPath + @"\wwwroot\images\";
                User User = new()
                {
                    userName = RegisterData.userName,
                    userPhoto = wwwroot + "default.jpg",
                    userAccount = RegisterData.userAccount,
                    userEmail = RegisterData.userEmail,
                    userPassword = UserService.HashPassword(RegisterData.userPassword),
                    userAuthCode = MailService.GenerateAuthCode()
                };

                var path = Directory.GetCurrentDirectory() + "/Verificationletter/RegisterTempMail.html";
                string TempMail = System.IO.File.ReadAllText(path);

                var querystr =  $@"Account={User.userAccount}&AuthCode={User.userAuthCode}";

                var request = HttpContext.Request;
                UriBuilder ValidateUrl = new()
                {
                    Scheme = request.Scheme, // 使用請求的協議 (http/https)
                    Host = request.Host.Host, // 使用請求的主機名
                    Port = request.Host.Port ?? 80, // 使用請求的端口，如果未指定則默認使用80
                    Path = "/BrainBoost/User/MailValidate?" + querystr
                };
                string finalUrl = ValidateUrl.ToString().Replace("%3F","?");

                string MailBody = MailService.GetMailBody(TempMail, User.userName, finalUrl);
                MailService.SendMail(MailBody, User.userEmail);
                string str = "寄信成功，請收信。";
                
                UserService.Register(User);
                return Ok(new Response(){
                                    status_code = 200,
                                    message = str
                                });
            }
            return BadRequest(new Response(){
                                        status_code = 400,
                                        message = "請完整輸入資料"
                                    });
        }

        // 郵件驗證
        [HttpGet("[Action]")]
        public IActionResult MailValidate(string Account,string AuthCode)
        {
            if (UserService.MailValidate(Account, AuthCode))
                return Ok(new Response(){
                    message = "已驗證成功",
                    status_code = 200,
                    data = User.Identity.Name
                });
            else
                return Ok(new Response(){
                    message = "請重新確認或重新註冊",
                    status_code = 400,
                    data = User.Identity.Name
                });
        }
        #endregion
        #region 登入
        // 登入
        [HttpPost("[Action]")]
        public IActionResult Login([FromBody]UserLogin User)
        {
            string ValidateStr = UserService.LoginCheck(User.userAccount, User.userPassword);
            if (!string.IsNullOrWhiteSpace(ValidateStr)){
                Response result = new(){
                    message = ValidateStr,
                    status_code = 400
                };
                return Ok(result);
            }
            else
            {
                int Role = UserService.GetRole(User.userAccount);
                var jwt = JwtHelpers.GenerateToken(User.userAccount,Role);
                Response result = new(){ 
                    message = "登入成功",
                    status_code = 200,
                    data = jwt 
                };
                return Ok(result);
            }
        }
        #endregion
        #region 修改個人資料
        //修改個人資料
        [HttpPut]
        [Route("")]
        public IActionResult UpdateUserData(UserUpdate Data){
            try{
                if(User.Identity.Name == null){
                    return BadRequest(new Response(){
                        status_code = 400,
                        message = "請先登入"
                    });
                }
                UserUpdate user = new(){
                    userId = UserService.GetDataByAccount(User.Identity.Name).userId,
                    userName = Data.userName
                };
                //處理圖片
                var wwwroot = evn.ContentRootPath + @"\wwwroot\images\";
                if(Data.file.Length > 0){
                    var imgname = User.Identity.Name + ".jpg";
                    var img_path = wwwroot + imgname;
                    using var stream = System.IO.File.Create(img_path);
                    Data.file.CopyTo(stream);
                    user.userPhoto = img_path;
                }
                else{
                    user.userPhoto = wwwroot + "default.jpg";
                }
                UserService.UpdateUserData(user);
                var userData = UserService.GetDataByAccount(User.Identity.Name);
                userData.userPassword = string.Empty;
                Response result = new(){
                    status_code = 200,
                    message = "修改成功",
                    data = userData
                };
                return Ok(result);
            }
            catch(Exception ex){
                Response result = new(){
                    status_code = 400,
                    message = ex.Message
                };
                return Ok(result);
            }
        }
        #endregion
        #region 後台管理者
        //取得目前所有使用者
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("AllUser")]
        public IActionResult AllUser([FromQuery]string? Search,[FromQuery]int page = 1){
            UserViewModel data = new(){
                forpaging = new Forpaging(page)
            };
            data.user = UserService.GetAllMemberList(Search,data.forpaging);
            data.search = Search;
            return Ok(new Response(){
                status_code = 200,
                message = "讀取成功",
                data = data
            });
        }

        // 取得單一使用者(帳號)
        // 未來可加任課科目&上課科目
        [HttpGet]
        [Route("User")]
        public IActionResult UserByAcc([FromQuery]string account){
            try{
                User data = UserService.GetDataByAccount(account);
                data.userPassword = string.Empty;
                return Ok(new Response(){
                    status_code = 200,
                    data = data
                });
            }
            catch (Exception e){
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
            }
        }
        #endregion

        #region 忘記密碼
        // 輸入Email後寄驗證信
        [HttpPost]
        [Route("[Action]")]
        public IActionResult ForgetPassword([FromBody]ForgetPassword Email)
        {
            // 看有沒有Email的資料
            User Data = UserService.GetDataByEmail(Email.Email);
            // 有就寄驗證信，沒有則回傳「查無用戶」
            if (Data != null)
            {
                Data.userAuthCode = MailService.GenerateAuthCode();
                // 製作AuthCode
                UserService.ChangeAuthCode(Data.userAuthCode, Email.Email);
                // 寄驗證信
                var path = Directory.GetCurrentDirectory() + "/Verificationletter/ForgetPasswordTempMail.html";
                string TempMail = System.IO.File.ReadAllText(path);
                string MailBody = MailService.GetMailBody(TempMail, Data.userName, Data.userAuthCode);
                MailService.SendForgetMail(MailBody, Email.Email);
                string str = "寄信成功，請收信。";
                return Ok(new Response(){
                    status_code = 200,
                    message = str
                });
            }
            else
                return BadRequest(new Response(){
                    status_code = 400,
                    message = "查無此戶"
                });
        }

        // 檢查驗證碼
        [HttpPost]
        [Route("[Action]")]
        public IActionResult CheckForgetPasswordCode([FromBody]CheckForgetPasswordAuthCode Data)
        {
            // 取得此Email的會員資訊
            User user = UserService.GetDataByEmail(Data.Email);
            // 判斷驗證碼是否正確
            if (user.userAuthCode == Data.AuthCode)
            {
                RoleService.SetMemberRole_ForgetPassword(user.userId);
                int Role = UserService.GetRole(user.userAccount);
                var jwt = JwtHelpers.GenerateToken(user.userAccount, Role);
                Response result = new()
                {
                    message = "驗證成功",
                    status_code = 200,
                    data = jwt
                };
                // 回傳成功
                return Ok(result);
            }
            else
            {
                // 回傳失敗
                return Ok(new Response()
                {
                    message = "驗證碼錯誤",
                    status_code = 400
                });
            }
        }

        // 修改密碼
        [HttpPost]
        [Route("ChangePasswordByForget")]
        [Authorize(Roles = "ForgetPassword")]
        public IActionResult ChangePassword([FromBody]CheckForgetPassword Data)
        {
            // 取得此Email的會員資訊
            if (User.IsInRole("ForgetPassword"))
            {
                User user = UserService.GetDataByEmail(Data.Email);
                if(User.Identity.Name != user.userAccount || user == null)
                    return BadRequest(new Response(){status_code = 400, message = "電子郵件不符，請重新輸入"});
                UserService.ClearAuthCode(Data.Email);
                UserService.ChangePasswordByForget(Data);
                return Ok(new Response(){
                    status_code = 200,
                    message = "修改密碼成功！請再次登入！"
                });
            }
            else
            {
                // 用戶未獲得足夠的權限
                return BadRequest(new Response(){
                    status_code = 400,
                    message = "您無權執行此操作。"
                });
            }

        }
        #endregion

    }
}