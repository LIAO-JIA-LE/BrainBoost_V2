using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost_V2.Models;

namespace BrainBoost_V2.ViewModels
{
    public class SubjectTagViewModel
    {
        public Subject subject{get;set;}
        public User user{get;set;}
        public List<Tag> tagList{get;set;}

    }
}