using System.Collections.Generic;

namespace LangBot.Web.Models
{
    public class EditOpenModel
    {
        public bool Anonymous { get; set; }
        public string ResponseUrl { get; set; }
        public IList<string> TextLines { get; set; }
        public string TemplateId { get; set; }
        public string UserId { get; set; }
    }
}

