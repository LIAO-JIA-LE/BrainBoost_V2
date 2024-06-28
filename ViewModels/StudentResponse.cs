using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.ViewModels
{
    public class StudentResponse
    {
        // 搶答室編號
        public int roomId{get;set;}

        // 題目編號
        public int questionId{get;set;}

        // 會員編號
        public int guestId{get;set;}

        // 限時時間
        public float timeLimit{get;set;}

        // 答題時間
        public float timeCose{get;set;}

        // 學生答案
        public string answer{get;set;}

        // 學生成績
        public decimal score{get;set;}
    }
}