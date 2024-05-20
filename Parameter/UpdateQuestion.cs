using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Parameter
{
    public class UpdateQuestion
    {
        public int questionId {get;set;}
        public int userId {get;set;}
        public int typeId {get;set;}
        [Required(ErrorMessage = "請輸入題目內容")]
        public required string questionContent {get;set;}
        [Required(ErrorMessage = "請輸入題目等級")]
        public int questionLevel {get;set;}
        // public string tagContent{get;set;}
        [Required(ErrorMessage = "請輸入答案內容")]
        public required string answerContent{get;set;}
        public string? parse{get;set;}
        // public bool isPhoto {get;set;}
        
        public List<string>? optionContent{get;set;}
        public List<IFormFile>? optionPicture{get;set;}
    }
}