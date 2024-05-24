using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BrainBoost_V2.Controller
{
    [Route("[controller]")]
    public class DownloadController : ControllerBase
    {
        #region 宣告函式
        // readonly DownloadService DownloadService = _downloadService;
        private readonly string _trueOrFalseTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "是非題.xlsx");
        private readonly string _multipleChoiceTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "選擇題.xlsx");
        #endregion

        #region 下載是非題範本
        [HttpGet("[Action]")]
        public IActionResult DownloadTrueOrFalseTemplate()
        {
            return DownloadTemplate(_trueOrFalseTemplatePath, "是非題.xlsx");
        }
        #endregion

        #region 下載選擇題範本
        [HttpGet("[Action]")]
        public IActionResult DownloadMultipleChoiceTemplate()
        {
            return DownloadTemplate(_multipleChoiceTemplatePath, "選擇題.xlsx");
        }
        #endregion

        private IActionResult DownloadTemplate(string filePath, string fileName)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "找不到檔案" });
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fileBytes, contentType, fileName);
        }
    }
}