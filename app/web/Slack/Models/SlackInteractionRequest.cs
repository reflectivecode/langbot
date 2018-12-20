using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LangBot.Web.Slack
{
    public class SlackInteractionRequest
    {
        [FromForm(Name = "payload"), Required] public string Payload { get; set; }
    }
}
