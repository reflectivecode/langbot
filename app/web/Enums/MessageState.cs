using System;

namespace LangBot.Web
{
    [Flags]
    public enum MessageState
    {
        None = 0,
        Preview = 1,
        Published = 2,
        Deleted = 4,
    }
}
