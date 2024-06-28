using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;

namespace BrainBoost_V2.ViewModels
{
    public class RoomQuestionViewModel
    {
        public Question question{get;set;}
        public List<Option> options{get;set;}
    }
}