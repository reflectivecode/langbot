using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LangBot.Web.Slack
{
    public class CommandRequest : ISlackRequest
    {
        [FromForm(Name = "command"), Required] public string Command { get; set; }
        [FromForm(Name = "text")] public string Text { get; set; } = "";
        [FromForm(Name = "token"), Required] public string Token { get; set; }
        [FromForm(Name = "user_id"), Required] public string UserId { get; set; }
    }
}
