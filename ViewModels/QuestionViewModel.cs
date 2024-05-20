using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;

namespace back.ViewModels
{
    public class QuestionViewModel
    {
        public int questionId { get; set; }
        public int userId { get; set; }
        public int typeId { get; set; }
        public string questionContent { get; set; }
        public string questionPicture { get; set; }
        public int questionLevel { get; set; }
        public string answerContent { get; set; }
        public string parse { get; set; }
        public List<OptionDto> options { get; set; } = [];
    }

    public class OptionDto
    {
        public string? optionContent { get; set; }
        public string? optionPicture { get; set; }
    }
}