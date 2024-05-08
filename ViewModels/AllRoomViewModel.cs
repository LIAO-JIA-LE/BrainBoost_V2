using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;
using BrainBoost_V2.Services;

namespace BrainBoost_V2.ViewModels
{
    public class AllRoomViewModel
    {
        public int userId{get;set;}
        public Forpaging forpaging{get;set;}
        public string search{get;set;}
        public List<Room> roomList{get;set;}
    }
}