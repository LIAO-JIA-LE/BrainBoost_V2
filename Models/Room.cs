using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost_V2.Models
{
    public class Room
    {
        // 搶答室編號
        public int roomId { get; set; }

        // 會員編號
        public int userId{get;set;}

        // 搶答室名稱
        public string roomName { get; set; }

        public int classId { get; set; }

        // 創建時間
        public DateTime createTime { get; set; }

        // 驗證碼
        public string roomPinCode { get; set; }

        // 搶答室模式
        public bool roomFunction{get;set;} 

        // 公開
        public bool roomPublic{ get; set; }

        // 假刪除
        public bool isDelete{get;set;}

        //是否開啟狀態
        public bool isOpen{get;set;}
        
        // 限時
        public int timeLimit{get;set;}
    }

}