using System;

namespace LangBot.Web
{
    public class Response
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string Guid { get; set; }
        public DateTime CreateDate { get; set; }
        public string ResponseUrl { get; set; }
        public string TeamId { get; set; }
        public string TeamDomain { get; set; }
        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
