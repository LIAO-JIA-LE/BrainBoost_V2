using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Parameter
{
    public class UpdateRoomQuestion
    {
        public int userId { get; set; }
        public int roomId { get; set; }
        public List<int> questionId { get; set; }
    }
}