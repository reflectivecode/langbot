using System.Collections.Generic;

namespace LangBot.Web.Models
{
    public class TemplateConfig
    {
        public IList<Template> Templates { get; set; }

        public class Template
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string File { get; set; }
        }
    }
}
