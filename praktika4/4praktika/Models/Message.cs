namespace _4praktika.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public string MessageSubject { get; set; }
        public string MessageText { get; set; }
        public DateTime SendDate { get; set; }
        public string Status { get; set; }
    }
}
