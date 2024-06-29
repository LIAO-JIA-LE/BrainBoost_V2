using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Back.Parameter
{
    public class GuestList
    {
        // 搶答室id
        public int roomId{get;set;}

        // 訪客名稱 列表
        public List<string> guestNameList{get;set;}

        // 嫁入搶答室總人數
        public int guestCount{get;set;}
    }
}