using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace LangBot.Web.Slack
{
    [DataContract]
    public class InteractionRequest
    {
        [DataMember(Name = "payload"), Required] public string Payload { get; set; }
    }
}
