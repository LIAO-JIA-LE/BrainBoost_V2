using BrainBoost_V2.Models;
using BrainBoost_V2.Service;

namespace BrainBoost_V2.ViewModels
{
    public class UserViewModel
    {
        public string search {get;set;}
        public Forpaging forpaging {get;set;} = new();
        public List<User> user {get;set;} = [];
    }
}