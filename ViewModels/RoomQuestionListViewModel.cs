using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;
using BrainBoost_V2.ViewModels;

namespace Back.ViewModels
{
    public class RoomQuestionListViewModel
    {
        public RoomClassViewModel room{get;set;}
        public List<Question> questionList{get;set;}
    }
}