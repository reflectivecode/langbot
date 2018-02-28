using System.Collections.Generic;

namespace LangBot.Web.Models
{
    public class ImageModel
    {
        public string ImageId { get; set; }
        public IList<TextBox> Boxes { get; set; } = new List<TextBox>();
    }
}
