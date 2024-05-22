using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;

namespace BrainBoost_V2.ViewModels
{
    public class RoomViewModel
    {
        public Room room{ get; set; }
        public List<Question> questionList { get; set; }
    }
}