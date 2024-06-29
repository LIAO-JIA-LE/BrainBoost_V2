namespace BrainBoost_V2.Models{
    public class Guest{
        public int guestId{get;set;}

        public string guestName{get;set;}

        public int roomId{get;set;}

        public bool isJoined{get;set;}
        public DateTime visitTime{get;set;}
    }
}