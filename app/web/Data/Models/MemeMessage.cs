using System;

namespace LangBot.Web
{
    public class MemeMessage
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string DeleteReason { get; set; }
        public string TeamId { get; set; }
        public string TeamDomain { get; set; }
        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
        public ChannelType ChannelType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string TemplateId { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAnonymous { get; set; }
        public int UpVoteCount { get; set; }
        public int FlagCount { get; set; }

        public MessageState MessageState
        {
            get
            {
                if (DeleteDate.HasValue) return MessageState.Deleted;
                if (PublishDate.HasValue) return MessageState.Published;
                return MessageState.Preview;
            }
        }
    }
}
