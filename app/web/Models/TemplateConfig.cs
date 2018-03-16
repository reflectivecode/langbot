using System.Collections.Generic;

namespace LangBot.Web.Models
{
    public class TemplateConfig
    {
        public Template TemplateDefaults { get; set; } = new Template();
        public IList<Template> Templates { get; set; } = new List<Template>();
        public IList<string> Privileged { get; set; } = new List<string>();

        public class Template
        {
            public IList<TextBox> Boxes { get; set; }
            public bool? Default { get; set; }
            public string File { get; set; }
            public string Format { get; set; }
            public string Id { get; set; }
            public bool Privileged { get; set; }
            public string Name { get; set; }
            public int? Quality { get; set; }
            public TextBox Watermark { get; set; }
            public int? GifPaletteSize { get; set; }
            public string GifQuantizer { get; set; }
        }
    }
}
