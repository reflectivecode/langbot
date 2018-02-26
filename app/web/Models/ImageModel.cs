using System.Collections.Generic;
using System.Drawing;

namespace LangBot.Web.Models
{
    public class ImageModel
    {
        public string ImageId { get; set; }
        public IList<Box> Boxes { get; set; } = new List<Box>();

        public class Box
        {
            public string Text { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public Alignment Vertical { get; set; } = Alignment.Center;
            public Color FillColor { get; set; } = Color.White;
            public Color LineColor { get; set; } = Color.Black;
        }

        public enum Alignment
        {
            Top,
            Center,
            Bottom,
        }
    }
}
