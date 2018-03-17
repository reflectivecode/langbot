using System;

namespace LangBot.Web
{
    public class Reaction
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string Type { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public string Message { get; set; }
    }
}
