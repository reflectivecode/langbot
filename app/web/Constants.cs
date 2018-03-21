namespace LangBot.Web
{
    public static class Constants
    {
        public static class Commands
        {
            public const string Lang = "/lang";
        }

        public static class CallbackIds
        {
            public const string Meme = "meme";
            public const string Edit = "edit";
        }

        public static class DialogElements
        {
            public const string Anonymous = "anonymous";
            public const string Text = "text";
        }

        public static class ActionNames
        {
            public const string UpVote = "up-vote";
            public const string Flag = "flag";
            public const string Edit = "edit";
            public const string Cancel = "cancel";
            public const string Submit = "submit";
            public const string Switch = "switch";
            public const string Raw = "raw";
        }

        public static class Reactions
        {
            public const string UpVote = "up-vote";
            public const string Flag = "flag";
        }
    }
}
