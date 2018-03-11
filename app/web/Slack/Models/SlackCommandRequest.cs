using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LangBot.Web.Slack
{
    public class SlackCommandRequest : ISlackRequest
    {
        [FromForm(Name = "command"), Required] public string Command { get; set; }
        [FromForm(Name = "text")] public string Text { get; set; } = "";
        [FromForm(Name = "token"), Required] public string Token { get; set; }
        [FromForm(Name = "team_id"), Required] public string TeamId { get; set; }
        [FromForm(Name = "team_domain"), Required] public string TeamDomain { get; set; }
        [FromForm(Name = "channel_id"), Required] public string ChannelId { get; set; }
        [FromForm(Name = "channel_name"), Required] public string ChannelName { get; set; }
        [FromForm(Name = "user_id"), Required] public string UserId { get; set; }
        [FromForm(Name = "user_name"), Required] public string UserName { get; set; }
    }
}
