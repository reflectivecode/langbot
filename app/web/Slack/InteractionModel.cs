﻿namespace LangBot.Web.Slack
{
    public class InteractionModel
    {
        public InteractionPayload Payload { get; }
        public string CallbackId { get => Payload.CallbackId; }
        public string ActionName { get => Payload.Actions[0].Name; }
        public string ActionValue { get => Payload.Actions[0].GetValue(); }

        public InteractionModel(InteractionPayload payload)
        {
            Payload = payload;
        }
    }
}
