using System.Drawing;

namespace LangBot.Web.Models
{
    public class TextBox
    {
        public string Label { get; set; }
        public string Text { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public AlignmentV Vertical { get; set; } = AlignmentV.Center;
        public AlignmentH Horizontal { get; set; } = AlignmentH.Center;
        public Color FillColor { get; set; } = Color.White;
        public Color LineColor { get; set; } = Color.Black;
    }
}
