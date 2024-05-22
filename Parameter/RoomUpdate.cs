using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Parameter
{
    public class RoomUpdate
    {
        // 搶答室編號
        public int roomId { get; set; }
        // 搶答室名稱
        public string roomName { get; set; }

        // 搶答室模式
        public bool roomFunction{get;set;} 

        // 公開
        public bool roomPublic{ get; set; }

        // 限時
        public int timeLimit{get;set;}
    }
}