namespace BrainBoost_V2.Services
{
    public class Forpaging
    {
        public int NowPage { get; set; }
        public int MaxPage { get; set; }
        public int Item
        {
            get
            {
                return 5;
            }
        }
        public Forpaging()
        {
            this.NowPage = 1;
        }
        public Forpaging(int Page)
        {
            this.NowPage = Page;
        }
        public void SetRightPage()
        {
            if (this.NowPage < 1)
            {
                this.NowPage = 1;
            }
            else if(this.NowPage > this.MaxPage)
            {
                this.NowPage = this.MaxPage;
            }
            if(this.MaxPage < 1)
            {
                this.MaxPage = 1;
            }
        }
    }
}