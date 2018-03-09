using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LangBot.Web.Enums;
using LangBot.Web.Models;
using LangBot.Web.Slack;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Brushes;
using SixLabors.ImageSharp.Drawing.Pens;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
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

        public async Task<(byte[] data, string mimeType)> Render(ImageModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var config = await _templateService.GetTemplates();
            var template = config.Templates.FirstOrDefault(x => x.Id == model.ImageId) ?? throw new SlackException($"Template id not found: {model.ImageId}");
            var imagePath = _templateService.GetTemplatePath(template);
            var fontPath = _templateService.GetFontPath();
            var font = new FontCollection().Install(fontPath).CreateFont(10, FontStyle.Regular);
            var boxes = model.Boxes.Prepend(template.Watermark ?? config.TemplateDefaults.Watermark).ToList();
            var encoder = GetEncoder(template, config);
            var mimeType = GetMimeType(template, config);

            using (var image = Image.Load(imagePath))
            {
                DrawTextBoxes(image, font, boxes);

                using (var stream = new MemoryStream())
                {
                    image.Save(stream, encoder);
                    return (stream.ToArray(), mimeType);
                }
            }
        }
        public static void DrawTextBoxes<TPixel>(Image<TPixel> image, Font font, IEnumerable<TextBox> boxes) where TPixel : struct, IPixel<TPixel>
        {
            image.Mutate(context =>
            {
                foreach (var box in boxes)
                {
                    var boxBounds = new RectangleF(
                        x: (float)(image.Width * box.X / 100),
                        y: (float)(image.Height * box.Y / 100),
                        width: (float)(image.Width * box.Width / 100),
                        height: (float)(image.Height * box.Height / 100)
                    );
                    var scaledFont = ScaleFont(new Font(font, image.Height / 8), box.Text, boxBounds.Width - 2 * outlineSize, boxBounds.Height - 2 * outlineSize);
                    var wrapWidth = ScaleWidth(scaledFont, box.Text, boxBounds.Width - 2 * outlineSize);
                    var lineColor = ColorBuilder<TPixel>.FromRGBA(box.LineColor.R, box.LineColor.G, box.LineColor.B, box.LineColor.A);
                    var fillColor = ColorBuilder<TPixel>.FromRGBA(box.FillColor.R, box.FillColor.G, box.FillColor.B, box.FillColor.A);
                    var pen = Pens.Solid(lineColor, 3f);
                    var brush = Brushes.Solid(fillColor);

                    var textBounds = TextMeasurer.MeasureBounds(box.Text, new RendererOptions(scaledFont)
                    {
                        WrappingWidth = wrapWidth,
                        HorizontalAlignment = ConvertHorizontalAlignment(box.Horizontal),
                    });

                    var drawLocation = GetLocation(box, boxBounds, textBounds);

                    // draw outline
                    context.DrawText(box.Text, scaledFont, pen, drawLocation, new TextGraphicsOptions(true)
                    {
                        WrapTextWidth = wrapWidth,
                        HorizontalAlignment = ConvertHorizontalAlignment(box.Horizontal),
                    });

                    // draw fill
                    context.DrawText(box.Text, scaledFont, brush, drawLocation, new TextGraphicsOptions(true)
                    {
                        WrapTextWidth = wrapWidth,
                        HorizontalAlignment = ConvertHorizontalAlignment(box.Horizontal),
                    });
                }
            });
        }

        private static PointF GetLocation(TextBox box, RectangleF boxBounds, RectangleF textBounds)
        {
            return new PointF(
                x: GetLocationX(box.Horizontal, boxBounds, textBounds),
                y: GetLocationY(box.Vertical, boxBounds, textBounds)
            );
        }

        private static float GetLocationX(AlignmentH alignment, RectangleF boxBounds, RectangleF textBounds)
        {
            switch (alignment)
            {
                case AlignmentH.Left: return boxBounds.X - textBounds.X;
                case AlignmentH.Right: return boxBounds.X - textBounds.X + boxBounds.Width - textBounds.Width;
                case AlignmentH.Center: return boxBounds.X - textBounds.X + boxBounds.Width / 2 - textBounds.Width / 2;
                default: throw new ArgumentOutOfRangeException(nameof(alignment));
            }
        }

        private static float GetLocationY(AlignmentV alignment, RectangleF boxBounds, RectangleF textBounds)
        {
            switch (alignment)
            {
                case AlignmentV.Top: return boxBounds.Y - textBounds.Y;
                case AlignmentV.Bottom: return boxBounds.Y - textBounds.Y + boxBounds.Height - textBounds.Height;
                case AlignmentV.Center: return boxBounds.Y - textBounds.Y + boxBounds.Height / 2 - textBounds.Height / 2;
                default: throw new ArgumentOutOfRangeException(nameof(alignment));
            }
        }

        private static HorizontalAlignment ConvertHorizontalAlignment(AlignmentH alignment)
        {
            switch (alignment)
            {
                case AlignmentH.Left: return HorizontalAlignment.Left;
                case AlignmentH.Right: return HorizontalAlignment.Right;
                case AlignmentH.Center: return HorizontalAlignment.Center;
                default: throw new ArgumentOutOfRangeException(nameof(alignment));
            }
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

        public static float ScaleWidth(Font font, string text, float width)
        {
            var initialHeight = TextMeasurer.MeasureBounds(text, new RendererOptions(font) { WrappingWidth = width }).Height;
            var singleLineHeight = TextMeasurer.MeasureBounds(text, new RendererOptions(font)).Height;
            if (initialHeight == singleLineHeight) return width;

            var maxWidth = width;
            var minWidth = 0f;

            while (maxWidth - minWidth > 0.1f)
            {
                var actual = TextMeasurer.MeasureBounds(text, new RendererOptions(font) { WrappingWidth = width });

                if (actual.Width > width || actual.Height > initialHeight + singleLineHeight / 10f)
                    minWidth = width;
                else
                    maxWidth = width;

                width = (maxWidth + minWidth) / 2f;
            }

            return maxWidth;
        }

        private static IImageEncoder GetEncoder(TemplateConfig.Template template, TemplateConfig config)
        {
            switch ((template.Format ?? config.TemplateDefaults.Format).ToUpperInvariant())
            {
                case "JPG":
                case "JPEG":
                    return new JpegEncoder()
                    {
                        IgnoreMetadata = true,
                        Quality = template.Quality ?? config.TemplateDefaults.Quality ?? 90
                    };
                case "PNG":
                    return new PngEncoder()
                    {
                        IgnoreMetadata = true
                    };
                case "BMP":
                    return new BmpEncoder();
                case "GIF":
                    return new GifEncoder()
                    {
                        IgnoreMetadata = true
                    };
                default:
                    throw new ArgumentOutOfRangeException("format");
            }
        }

        private static string GetMimeType(TemplateConfig.Template template, TemplateConfig config)
        {
            switch ((template.Format ?? config.TemplateDefaults.Format).ToUpperInvariant())
            {
                case "JPG":
                case "JPEG":
                    return "image/jpeg";
                case "PNG":
                    return "image/png";
                case "BMP":
                    return "image/bmp";
                case "GIF":
                    return "image/gif";
                default:
                    throw new ArgumentOutOfRangeException("format");
            }
        }
    }
}
