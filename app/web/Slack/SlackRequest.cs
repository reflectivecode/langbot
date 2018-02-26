using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace LangBot.Web.Slack
{
    public class SlackRequest
    {
        [DataMember(Name = "token"), Required] public string Token { get; set; }
    }
}
