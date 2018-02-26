using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LangBot.Web.Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Brushes;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;

namespace LangBot.Web.Services
{
    public class RenderService
    {
        private const float outlineSize = 1.0f;

        private readonly TemplateService _templateService;

        public RenderService(TemplateService templateService)
        {
            _templateService = templateService;
        }

        public async Task<byte[]> Render(ImageModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var imagePath = await _templateService.GetTemplatePath(model.ImageId);
            var fontPath = _templateService.GetFontPath();
            var font = new FontCollection().Install(fontPath).CreateFont(10, FontStyle.Regular);

            var boxes = model.Boxes.ToList();
            boxes.Add(new ImageModel.Box
            {
                Text = "LangBot",
                X = .5,
                Y = 98,
                Height = 2,
                Width = 10,
                Vertical = ImageModel.Alignment.Bottom,
            });

            using (var image = Image.Load(imagePath))
            {
                DrawTextBoxes(image, font, boxes);

                using (var stream = new MemoryStream())
                {
                    image.SaveAsJpeg(stream, new JpegEncoder() { Quality = 80 });
                    return stream.ToArray();
                }
            }
        }

        public static void DrawTextBoxes<TPixel>(Image<TPixel> image, Font font, IEnumerable<ImageModel.Box> boxes) where TPixel : struct, IPixel<TPixel>
        {
            image.Mutate(context =>
            {
                foreach (var box in boxes)
                {
                    var boxX = (float)(image.Width * box.X / 100);
                    var boxY = (float)(image.Height * box.Y / 100);
                    var boxWidth = (float)(image.Width * box.Width / 100);
                    var boxHeight = (float)(image.Height * box.Height / 100);
                    var scaledFont = ScaleFont(new Font(font, image.Height / 8), box.Text, boxWidth - 2 * outlineSize, boxHeight - 2 * outlineSize);
                    var lineColor = ColorBuilder<TPixel>.FromRGBA(box.LineColor.R, box.LineColor.G, box.LineColor.B, box.LineColor.A);
                    var fillColor = ColorBuilder<TPixel>.FromRGBA(box.FillColor.R, box.FillColor.G, box.FillColor.B, box.FillColor.A);

                    var textBounds = TextMeasurer.MeasureBounds(box.Text, new RendererOptions(scaledFont)
                    {
                        WrappingWidth = boxWidth,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    });

                    var center = new PointF(
                        boxX,
                        box.Vertical == ImageModel.Alignment.Top ? boxY - textBounds.Top :
                        box.Vertical == ImageModel.Alignment.Bottom ? boxY - textBounds.Top + boxHeight - textBounds.Height :
                        boxY - textBounds.Top + boxHeight / 2 - textBounds.Height / 2
                    );

                    var outlines = new[]
                    {
                        new PointF(center.X + outlineSize, center.Y + outlineSize),
                        new PointF(center.X + outlineSize, center.Y - outlineSize),
                        new PointF(center.X + outlineSize, center.Y),
                        new PointF(center.X - outlineSize, center.Y + outlineSize),
                        new PointF(center.X - outlineSize, center.Y - outlineSize),
                        new PointF(center.X - outlineSize, center.Y),
                        new PointF(center.X, center.Y - outlineSize),
                        new PointF(center.X, center.Y + outlineSize),
                    };

                    foreach (var outline in outlines)
                    {
                        context.DrawText(box.Text, scaledFont, lineColor, outline, new TextGraphicsOptions(true)
                        {
                            WrapTextWidth = boxWidth,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        });
                    }

                    context.DrawText(box.Text, scaledFont, fillColor, center, new TextGraphicsOptions(true)
                    {
                        WrapTextWidth = boxWidth,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    });
                }
            });
        }

        public static Font ScaleFont(Font font, string text, float targetWidth, float targetHeight)
        {
            var maxFontSize = font.Size;
            var minFontSize = 0f;

            while (maxFontSize - minFontSize > 0.1f)
            {
                var actual = TextMeasurer.MeasureBounds(text, new RendererOptions(font) { WrappingWidth = targetWidth });

                if (actual.Height > targetHeight || actual.Width > targetWidth)
                    maxFontSize = font.Size;
                else
                    minFontSize = font.Size;

                font = new Font(font, (maxFontSize + minFontSize) / 2);
            }

            return new Font(font, minFontSize);
        }
    }
}
