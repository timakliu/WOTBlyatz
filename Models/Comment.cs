namespace WOTBlyatz.Models
{
    public class Comment
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int ModId { get; set; } // ID мода, к которому относится комментарий
    }
}
